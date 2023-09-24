using DG.Tweening;
using StarterAssets;
using System.Collections;
using UnityEngine;

public class Player : Stat_Manager
{
    private static Player instance;
    public static Player Instance { get { return instance; } }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [SerializeField] private GameObject view;
    [SerializeField] private Knife prefabKnife;
    [Space]
    [SerializeField] private ThirdPersonController controller;
    [SerializeField] private StarterAssetsInputs input;

    private int knifeAmount = 0;
    private int direction = 1;
    private int elixirTime = 0;
    private int elixirTimeLimit = 10;
    private bool hasLevelFinishKey;
    private bool hasLevelSecretRoomKey;
    private bool findedMapItem;
    private Transform mapItem;
    public bool HasLevelFinishKey { get { return hasLevelFinishKey; } }

    private void Start()
    {
        Canvas_Manager.Instance.OnLevelLost += Instance_OnLevelLost;
        Canvas_Manager.Instance.OnLevelWin += Instance_OnLevelWin;
    }
    private void Instance_OnLevelLost(object sender, System.EventArgs e)
    {
        controller.enabled = false;
        hasLevelFinishKey = false;
        hasLevelSecretRoomKey = false;
    }
    private void Instance_OnLevelWin(object sender, System.EventArgs e)
    {
        controller.enabled = false;
        hasLevelFinishKey = false;
        hasLevelSecretRoomKey = false;
    }
    public void LevelStart(Vector3 playerStartPos)
    {
        transform.position = playerStartPos;
        controller.enabled = true;
    }
    public void TakedLevelFinishKey()
    {
        hasLevelFinishKey = true;
    }
    public void TakedLevelSecretRoom()
    {
        hasLevelSecretRoomKey = true;
    }
    private void Update()
    {
        Handle();
        UseMapItem();
    }
    private void Handle()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
    }
    private void UseMapItem()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!findedMapItem)
            {
                // We couldn't find a Map Item.
                return;
            }
            if (Vector3.SqrMagnitude(transform.position - mapItem.position) > 2.5f)
            {
                // Player is far.
                return;
            }
            controller.canMove = false;
            // Player decided to use Map Item.
            mapItem.GetComponent<IMapItem>().UseMapItem(transform);
        }
    }
    private void Attack()
    {
        if (knifeAmount > 0)
        {
            knifeAmount--;
            Vector3 direction1;

            Knife knife = Instantiate(prefabKnife, transform.position + Vector3.up, Quaternion.identity);
            direction = (int)transform.eulerAngles.y % 360;
            if (direction < 180 && direction > 0)
            {
                direction = 90;
                direction1 = Vector3.right;
                knife.transform.GetChild(0).localEulerAngles = Vector3.up * 180;
            }
            else
            {
                direction = 270;
                direction1 = Vector3.left;
            }
            transform.eulerAngles = Vector3.up * direction;
            knife.SetSpeed(direction1, "Player");
        }
    }
    public void IncreaseKnife()
    {
        knifeAmount++;
    }
    public void TakedElixir()
    {
        elixirTime = 0;
        SetViewObject();
    }
    private void SetViewObject()
    {
        view.SetActive(false);
        DOTween.To(value => { }, startValue: 0, endValue: 1, duration: 0.25f).SetEase(Ease.Linear).
            OnComplete(() =>
            {
                view.SetActive(true);
                DOTween.To(value => { }, startValue: 0, endValue: 1, duration: 0.25f).SetEase(Ease.Linear).
                     OnComplete(() =>
                     {
                         elixirTime++;
                         if (elixirTime != elixirTimeLimit)
                         {
                             SetViewObject();
                         }
                     });
            });
    }
    public void FoundSecretRoom()
    {
        if (hasLevelSecretRoomKey)
        {
            // Give the player the power to walk in 3D
            input.Set3DWalking();
            Vector3 playerNewPos = transform.position;
            playerNewPos.z = 2.5f;
            transform.DOMove(playerNewPos, 0.5f).OnComplete(() =>
            {
                // Adjust the angle of the camera according to the walk.
                Camera_Manager.Instance.Set3DWalking();
            });
            transform.DORotate(Vector3.zero, 0.5f);
        }
    }
    public void PlayerCanWalk()
    {
        input.PlayerCanWalk();
    }
    public void ExitSecretRoom()
    {
        // Give the player the power to walk on the platform
        input.SetPlatformWalking();
        // Adjust the angle of the camera according to the walk.
        Camera_Manager.Instance.SetPlatformWalking();

        StartCoroutine(SetPlatform());
    }
    IEnumerator SetPlatform()
    {
        controller.exitingSecretRoom = true;
        yield return new WaitForSeconds(0.25f);
        Vector3 playerNewPos = transform.position;
        playerNewPos.z = 0;
        transform.DOMove(playerNewPos, 0.25f);
        // Set player position to correct platform position
        yield return new WaitForSeconds(0.5f);
        controller.exitingSecretRoom = false;
    }
    public bool PlayerWalkingOnPlatform()
    {
        return input.WalkingOnPlatform();
    }
    public override void Dead()
    {
        Canvas_Manager.Instance.LevelLost();
    }
    public override void UsingMapItem()
    {
        controller.canMove = false;
    }
    public override void ExitingMapItem()
    {
        controller.canMove = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MapItem"))
        {
            findedMapItem = true;
            mapItem = other.transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MapItem"))
        {
            findedMapItem = false;
            mapItem = null;
        }
    }
}