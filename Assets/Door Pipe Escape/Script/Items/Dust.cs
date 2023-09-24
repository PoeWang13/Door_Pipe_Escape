using UnityEngine;

public class Dust : MonoBehaviour
{
    [SerializeField] private DustBag dustBag;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dustBag.Drop();
            Destroy(gameObject);
        }
    }
}