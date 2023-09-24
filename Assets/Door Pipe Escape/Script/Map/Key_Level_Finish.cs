using UnityEngine;

public class Key_Level_Finish : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.Instance.TakedLevelFinishKey();
            Canvas_Manager.Instance.HasLevelFinishKey(true);
            Destroy(gameObject);
        }
    }
}