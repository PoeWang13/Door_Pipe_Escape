using UnityEngine;
using DG.Tweening;

public class Saw : MonoBehaviour
{
    [SerializeField] private bool limitless;
    private bool isActivited;
    private Vector3 orjPos;
    private Vector3 attackPos;
    private void Start()
    {
        orjPos = transform.position;
        attackPos = new Vector3(orjPos.x, orjPos.y, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.Instance.IncreaseKnife();
        }
        else if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage();
        }
    }
    // Assigned to Activator_Manager in parent.
    public void ShowView()
    {
        isActivited = true;
        SendSaw();
    }
    // Assigned to Activator_Manager in parent.
    public void HideView()
    {
        isActivited = false;
    }
    private void SendSaw()
    {
        transform.DOMove(attackPos, 0.25f).OnComplete(() =>
        {
            DOTween.To(value => { }, startValue: 0, endValue: 1, duration: 0.25f).OnComplete(
                () =>
                {
                    transform.DOMove(orjPos, 0.25f).OnComplete(
                    () =>
                    {
                        if (limitless && isActivited)
                        {
                            DOTween.To(value => { }, startValue: 0, endValue: 1, duration: 0.25f).OnComplete(
                                () =>
                                {
                                    ShowView();
                                });
                        }
                    });
                });
        });
    }

}