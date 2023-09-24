using UnityEngine;
using DG.Tweening;

public class Wall : MonoBehaviour
{
    [SerializeField] private Transform wallView;
    private Vector3 orjPos;
    private int hasSomeOne;
    private void Start()
    {
        orjPos = wallView.position;
    }
    // Assigned to Activator_Manager in parent.
    public void ActiveWall()
    {
        DOTween.To(value => { }, startValue: 0, endValue: 1, duration: 0.1f).OnComplete(
            () =>
            {
                NoEnterWallLine();
            });
    }
    // Assigned to Activator_Manager in parent.
    public void DeActiveWall()
    {
        wallView.DOMove(orjPos, 0.25f).OnComplete(() =>
        {
            hasSomeOne = 0;
            gameObject.SetActive(false);
        });
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasSomeOne++;
        }
        else if (other.CompareTag("Enemy"))
        {
            hasSomeOne++;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ExitWall();
        }
        else if (other.CompareTag("Enemy"))
        {
            ExitWall();
        }
    }
    private void ExitWall()
    {
        hasSomeOne--;
        NoEnterWallLine();
    }
    private void NoEnterWallLine()
    {
        if (hasSomeOne == 0)
        {
            wallView.DOMove(transform.position, 0.25f);
        }
    }
}