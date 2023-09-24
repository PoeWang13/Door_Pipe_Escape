using UnityEngine;

public class Thrower : Enemy
{
    [SerializeField] private float attackCoolDown = 3;
    [SerializeField] private Knife prefabKnife;

    private float attackCoolDownMax;
    private Transform player;

    public override void Start()
    {
        base.Start();
        player = Player.Instance.transform;
    }
    public override void SpecialFunctionForEnemy()
    {
        attackCoolDownMax += Time.deltaTime;
        if (attackCoolDownMax < attackCoolDown)
        {
            // It's not time to shoot again.
            return;
        }
        if (Vector3.SqrMagnitude(transform.position - player.transform.position) > 25)
        {
            // Player is far.
            return;
        }
        if (Mathf.Abs(transform.position.y - player.transform.position.y) > 1)
        {
            // We are not the same height as the Player.
            return;
        }
        if (View.localEulerAngles.y == 180)
        {
            if (transform.position.x < player.transform.position.x)
            {
                // Enemy looks left. The player is on our right side, so the player is in the back.
                return;
            }
        }
        else if (View.localEulerAngles.y == 0)
        {
            if (transform.position.x > player.transform.position.x)
            {
                // Enemy looks to the right. The player is on our left side, so the player is in the back.
                return;
            }
        }
        Vector3 startPos = transform.position + Vector3.up;
        // For Fire Direction
        Ray rayFireDirection = new Ray(startPos, Vector3.right);
        Debug.DrawRay(startPos, Vector3.right * 5 * (View.localEulerAngles.y == 180 ? -1 : 1), Color.magenta);
        if (Physics.Raycast(rayFireDirection, 5, WallMask))
        {
            // Player remained behind the wall.
            return;
        }
        // Player is shootable.
        Fire();
    }
    private void Fire()
    {
        attackCoolDownMax = 0;
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Knife knife = Instantiate(prefabKnife, transform.position + Vector3.up, Quaternion.identity);
        if (View.localEulerAngles.y == 0)
        {
            knife.transform.GetChild(0).localEulerAngles = Vector3.up * 180;
        }
        knife.SetSpeed(direction, "Enemy");
    }
}