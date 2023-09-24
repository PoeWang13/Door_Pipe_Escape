using UnityEngine;
using DG.Tweening;
public class Camera_Manager : Singletion<Camera_Manager>
{
    [SerializeField] private Vector2 levelLimit;
    private Transform player;
    private int playerOffsetX = 12;
    private float playerOffsetY = 6.5f;
    private bool isPlatformWalking = true;
    private bool cameraChanging = false;
    private void Start()
    {
        player = Player.Instance.transform;
    }
    private void LateUpdate()
    {
        if (cameraChanging)
        {
            return;
        }
        if (isPlatformWalking)
        {
            Vector3 playerPos = player.position + Vector3.back * 10;
            if (playerPos.x < -levelLimit.x + playerOffsetX)
            {
                playerPos.x = -levelLimit.x + playerOffsetX;
            }
            else if (playerPos.x > levelLimit.x - playerOffsetX)
            {
                playerPos.x = levelLimit.x - playerOffsetX;
            }
            if (playerPos.y < -levelLimit.y + playerOffsetY)
            {
                playerPos.y = -levelLimit.y + playerOffsetY;
            }
            else if (playerPos.y > levelLimit.y - playerOffsetY)
            {
                playerPos.y = levelLimit.y - playerOffsetY;
            }
            transform.position = playerPos;
        }
        else
        {
            Vector3 playerPos = player.position + Vector3.up * 6;
            transform.position = playerPos;
        }
    }
    public void SetCamera(Vector2 level)
    {
        levelLimit = level;
    }
    [ContextMenu("Set 3D Walking")]
    public void Set3DWalking()
    {
        cameraChanging = true;
        // Adjust the angle of the camera according to your walk.
        Vector3 playerNewPos = player.position + new Vector3(-2.5f, 6, 0);
        Vector3 playerNewAngle = Vector3.right * 35;
        transform.DOMove(playerNewPos, 1).OnComplete(() =>
        {
            cameraChanging = false;
            isPlatformWalking = false;
            Player.Instance.PlayerCanWalk();
        });
        transform.DORotate(playerNewAngle, 1);
    }
    [ContextMenu("Set Platform Walking")]
    public void SetPlatformWalking()
    {
        cameraChanging = true;
        // Set the camera to its original state.
        Vector3 playerNewPos = player.position + Vector3.back * 10;
        Vector3 playerNewAngle = Vector3.zero;
        transform.DOMove(playerNewPos, 1).OnComplete(() =>
        {
            cameraChanging = false;
            isPlatformWalking = true;
            Player.Instance.PlayerCanWalk();
        });
        transform.DORotate(playerNewAngle, 1);
    }
}