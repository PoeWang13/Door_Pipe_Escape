using UnityEngine;

public class Door_Level_Secret_Room : MonoBehaviour
{
    [SerializeField] private GameObject secret_Room;
    private bool hasPlayer;
    private bool playerInSecretRoom;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasPlayer = true;
            if (other.TryGetComponent<Player>(out Player player))
            {
                // Is the player on the platform or in the secret room?
                playerInSecretRoom = !player.PlayerWalkingOnPlatform();
                if (playerInSecretRoom)
                {
                    secret_Room.SetActive(false);
                    // If the player is inside the Secret Room, it means the player came out from inside.
                    Player.Instance.ExitSecretRoom();
                    Destroy(gameObject);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasPlayer = false;
        }
    }
    private void Update()
    {
        if (!hasPlayer)
        {
            return;
        }
        if (playerInSecretRoom)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            secret_Room.SetActive(true);
            Player.Instance.FoundSecretRoom();
        }
    }
}