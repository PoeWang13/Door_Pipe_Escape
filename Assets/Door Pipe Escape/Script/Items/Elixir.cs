using UnityEngine;

public class Elixir : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.Instance.TakedElixir();
            Destroy(gameObject);
        }
    }
}