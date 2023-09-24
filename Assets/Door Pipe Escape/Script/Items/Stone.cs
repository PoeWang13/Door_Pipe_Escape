using UnityEngine;
using DG.Tweening;

public class Stone : MonoBehaviour
{
    private float pushTime = 0.05f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Push();
        }
        else if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage();
            DOTween.Kill(transform);
            Destroy(gameObject);
        }
    }
    public void Push()
    {
        Vector3 tr = Vector3.zero;
        Vector3 pl = Vector3.zero;
        tr.x = transform.position.x;
        pl.x = Player.Instance.transform.position.x;

        Vector3 direction = (tr - pl).normalized;
        transform.DOMove(direction + transform.position, pushTime);
    }
}