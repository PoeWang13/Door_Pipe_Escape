using DG.Tweening;
using UnityEngine;

public class Enemy : Stat_Manager
{
    public enum DecideForUsingMapItem
    {
        None, UsedMapItem, DontUseMapItem
    }
    private const float SIN25 = 0.42262f;
    private const float GROUND_DISTANCE_OFFSET = 0.1f;

    [Header("General Value")]
    [SerializeField] private int speed = 1;
    [SerializeField] private Transform view;
    [Header("Layer Masks")]
    [SerializeField] private LayerMask stairMask;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private LayerMask playerMask;
    [Header("Colliders")]
    [SerializeField] private Collider myCollider;
    [SerializeField] private CharacterController controller;

    private float wallDistance = 0.5f;
    private float groundDistance = 0.0f;
    private bool usingStair = false;
    private bool passingStair = false;
    private bool checkedStair = false;
    private bool findedMapItem;
    private bool usingMapItem;
    private bool killedPlayer;
    private Transform mapItem;
    private Collider stairCollider;
    private Vector3 translateDirection = new Vector3(1, 0, 0);
    private DecideForUsingMapItem decideForUsingMapItem;

    public Transform View { get { return view; } }
    public LayerMask WallMask { get { return wallMask; } }

    #region Genel Fonksions
    public virtual void Start()
    {
        translateDirection = view.localEulerAngles.y == 180 ? Vector3.left : Vector3.right;
        Canvas_Manager.Instance.OnLevelLost += Instance_OnLevelLost;
        Canvas_Manager.Instance.OnLevelWin += Instance_OnLevelWin;
    }
    private void Instance_OnLevelLost(object sender, System.EventArgs e)
    {
        speed = 0;
    }
    private void Instance_OnLevelWin(object sender, System.EventArgs e)
    {
        speed = 0;
    }
    public override void Dead()
    {
        Canvas_Manager.Instance.AddScore(100);
        Destroy(gameObject);
    }
    private void Update()
    {
        if (usingMapItem)
        {
            // We ar using a Map Item.
            return;
        }
        CheckingEndOfTheGround();
        CheckingWall();
        CheckingStair();
        SpecialFunctionForEnemy();
        FindedMapItem();
        controller.Move(translateDirection * speed * Time.deltaTime);
    }
    #endregion

    #region Enemy Moving Behaviour
    public virtual void FindedMapItem()
    {
        if (!findedMapItem)
        {
            // We couldn't find a Map Item.
            return;
        }
        if (decideForUsingMapItem == DecideForUsingMapItem.DontUseMapItem)
        {
            // We decided not to use Map Item.
            return;
        }
        if (decideForUsingMapItem == DecideForUsingMapItem.None)
        {
            if (Random.value > 0.5f)
            {
                decideForUsingMapItem = DecideForUsingMapItem.DontUseMapItem;
                return;
            }
            // We decided what to do with Map Item.
            decideForUsingMapItem = DecideForUsingMapItem.UsedMapItem;
        }
        if (Vector3.SqrMagnitude(transform.position - mapItem.position) > 0.2f)
        {
            // Enemy is far.
            return;
        }
        if (!mapItem.GetComponent<IMapItem>().UseMapItem(transform))
        {
            // Another Enemy uses that Map Item.
            return;
        }
        // Enemy decided to use Map Item.
        UsingMapItem();
    }
    public override void UsingMapItem()
    {
        usingMapItem = true;
        speed = 0;
    }
    public override void ExitingMapItem()
    {
        decideForUsingMapItem = DecideForUsingMapItem.DontUseMapItem;
        usingMapItem = false;
        speed = 1;
    }
    private void CheckingEndOfTheGround()
    {
        if (usingStair)
        {
            return;
        }
       // Checking end of the ground
       Vector3 startPosForGround = transform.position + Vector3.up * 0.1f + Vector3.right * 0.4f * translateDirection.x;
        Ray rayCheckingGroundDownSide = new Ray(startPosForGround, Vector3.down);
        Debug.DrawRay(startPosForGround, Vector3.down * 0.25f, Color.yellow);
        if (!Physics.Raycast(rayCheckingGroundDownSide, 0.25f, groundMask))
        {
            // No Ground
            // Are there stairs?
            if (Physics.Raycast(rayCheckingGroundDownSide, 1, stairMask))
            {
                // There are stairs.
                if (Random.value > 0.5f)
                {
                    // Use stairs
                    usingStair = true;
                    CheckingStair(usingStair);
                    translateDirection += Vector3.down * 0.85f;
                }
                else
                {
                    // Turn back
                    ChanceViewDirection();
                }
                return;
            }
            // There are not stairs so Turn back
            ChanceViewDirection();
        }
    }
    private void ChanceViewDirection()
    {
        int newAngles = ((int)view.localEulerAngles.y + 180) % 360;
        view.localEulerAngles = new Vector3(0, newAngles, 0);
        translateDirection *= -1;
    }
    private void CheckingWall()
    {
        if (usingStair)
        {
            return;
        }
        // Checking wall
        Vector3 startPosForWall = transform.position + Vector3.up + Vector3.right * 0.1f * translateDirection.x;
        Ray rayCheckingWall = new Ray(startPosForWall, Vector3.right * translateDirection.x);
        Debug.DrawRay(startPosForWall, Vector3.right * translateDirection.x * wallDistance, Color.cyan);

        if (Physics.Raycast(rayCheckingWall, 1, playerMask))
        {
            if (!killedPlayer)
            {
                Canvas_Manager.Instance.LevelLost();
                killedPlayer = true;
            }
        }
        else if (Physics.Raycast(rayCheckingWall, wallDistance, wallMask))
        {
            // There are not grounds so Turn back
            ChanceViewDirection();
        }
    }
    private void CheckingStair()
    {
        if (!usingStair && !passingStair)
        {
            Vector3 startUp = transform.position + Vector3.up * 1.9f;
            Vector3 startDown = transform.position + Vector3.up * 0.1f;

            Ray rayUpSide = new Ray(startUp, translateDirection);
            Ray rayDownSide = new Ray(startDown, translateDirection);

            Debug.DrawRay(startUp, translateDirection * 0.6f, Color.red);
            Debug.DrawRay(startDown, translateDirection * 0.6f, Color.red);

            // Are there stairs in front of us?
            // There are stairs, are we behind the stairs?
            if (Physics.Raycast(rayUpSide, out RaycastHit hitStairObject, 0.6f, stairMask))
            {
                passingStair = true;
                stairCollider = hitStairObject.transform.GetComponent<Collider>();
                SetStairForUsingGroundUnderStair(passingStair);
                CheckingStair(passingStair);
            }
            // There are stairs, Are we in front of the stairs?
            else if (Physics.Raycast(rayDownSide, out RaycastHit hitStairObject2, 0.6f, stairMask))
            {
                stairCollider = hitStairObject2.transform.GetComponent<Collider>();
                Ray rayGroundUnderStair = new Ray(hitStairObject2.transform.position, Vector3.down);
                // Merdiven altında yürünebilir ground var mı?
                groundDistance = hitStairObject2.transform.localScale.x * SIN25 + GROUND_DISTANCE_OFFSET;
                if (Physics.Raycast(rayGroundUnderStair, groundDistance, groundMask))
                {
                    // If there are grounds
                    // Randomly use the stairs or go straight
                    if (Random.value > 0.5f)
                    {
                        passingStair = true;
                        // Use Ground
                        SetStairForUsingGroundUnderStair(passingStair);
                        CheckingStair(passingStair);
                    }
                    else
                    {
                        usingStair = true;
                        // Use stairs
                        CheckingStair(usingStair);
                    }
                }
                // Ther are not ground, use stairs
                else
                {
                    usingStair = true;
                    // Use stairs
                    CheckingStair(usingStair);
                }
            }
        }
        else
        {
            if (checkedStair)
            {
                if (usingStair)
                {
                    int k = 0;
                    Vector3 startPos = transform.position + Vector3.up * 0.1f + Vector3.right * 0.4f * translateDirection.x;
                    // For TranslateDirection
                    Ray rayDownSide = new Ray(startPos, Vector3.down);
                    Debug.DrawRay(startPos, Vector3.down, Color.blue);
                    if (!Physics.Raycast(rayDownSide, 2.0f, stairMask))
                    {
                        k++;
                    }
                    startPos += Vector3.left * 0.8f * translateDirection.x;
                    // For opposite TranslateDirection
                    rayDownSide = new Ray(startPos, Vector3.down);
                    Debug.DrawRay(startPos, Vector3.down, Color.blue);
                    if (!Physics.Raycast(rayDownSide, 2.0f, stairMask))
                    {
                        k++;
                    }
                    if (k == 2)
                    {
                        usingStair = false;
                        translateDirection = view.localEulerAngles.y == 180 ? Vector3.left : Vector3.right;
                        CheckingStair(usingStair);
                    }
                }
                else if (passingStair)
                {
                    int k = 0;
                    for (int e = 0; e < 3; e++)
                    {
                        Vector3 startPos = transform.position + Vector3.up * 0.1f + Vector3.right * 0.4f * translateDirection.x + Vector3.up * e * 0.9f;
                        // For TranslateDirection
                        Ray rayUpSide = new Ray(startPos, translateDirection);
                        Debug.DrawRay(startPos, translateDirection * 2, Color.green);
                        if (!Physics.Raycast(rayUpSide, 2, stairMask))
                        {
                            k++;
                        }
                        startPos += Vector3.left * 0.8f * translateDirection.x;
                        // For opposite TranslateDirection
                        rayUpSide = new Ray(startPos, -translateDirection);
                        Debug.DrawRay(startPos, -translateDirection * 2, Color.green);
                        if (!Physics.Raycast(rayUpSide, 2, stairMask))
                        {
                            k++;
                        }
                    }
                    if (k == 6)
                    {
                        passingStair = false;
                        CheckingStair(passingStair);
                        SetStairForUsingGroundUnderStair(passingStair);
                    }
                }
            }
        }
    }
    private void SetStairForUsingGroundUnderStair(bool ignoreCollider)
    {
        Physics.IgnoreCollision(myCollider, stairCollider, ignoreCollider);
    }
    private void CheckingStair(bool checking)
    {
        DOTween.To(value => { }, startValue: 0, endValue: 1, duration: 0.1f).OnComplete(() => checkedStair = checking);
    }
    public virtual void SpecialFunctionForEnemy()
    {

    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.Instance.TakeDamage();
        }
        else if (other.CompareTag("MapItem"))
        {
            findedMapItem = true;
            mapItem = other.transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.Instance.TakeDamage();
        }
        else if (other.CompareTag("MapItem"))
        {
            findedMapItem = false;
            decideForUsingMapItem = DecideForUsingMapItem.None;
            mapItem = null;
        }
    }
}