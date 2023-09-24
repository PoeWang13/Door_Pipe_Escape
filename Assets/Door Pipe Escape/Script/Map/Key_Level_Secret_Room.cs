using UnityEngine;

public class Key_Level_Secret_Room : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.Instance.TakedLevelSecretRoom();
            Canvas_Manager.Instance.HasLevelSecretRoomKey(true);
            Destroy(gameObject);
        }
    }
}