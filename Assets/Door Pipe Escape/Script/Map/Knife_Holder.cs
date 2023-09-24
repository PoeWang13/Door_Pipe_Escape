using UnityEngine;

public class Knife_Holder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.Instance.IncreaseKnife();
            Destroy(gameObject);
        }
    }
}