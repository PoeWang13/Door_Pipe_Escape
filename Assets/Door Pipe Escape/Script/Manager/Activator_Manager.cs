using UnityEngine;
using UnityEngine.Events;

public class Activator_Manager : MonoBehaviour
{
    [SerializeField] private UnityEvent buttonActive;
    [SerializeField] private UnityEvent buttonRelease;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            buttonActive?.Invoke();
        }
        else if (other.CompareTag("Enemy"))
        {
            buttonActive?.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            buttonRelease?.Invoke();
        }
        else if (other.CompareTag("Enemy"))
        {
            buttonRelease?.Invoke();
        }
    }
}