using UnityEngine;
using DG.Tweening;

public class DustBag : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Canvas_Manager.Instance.Dusted();
            DOTween.Kill(transform);
            Destroy(gameObject);
        }
    }
    public void Drop()
    {
        Vector3 stopPozition = transform.position + Vector3.down * 2.5f;
        transform.SetParent(null);
        transform.DOMove(stopPozition, 1.5f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}