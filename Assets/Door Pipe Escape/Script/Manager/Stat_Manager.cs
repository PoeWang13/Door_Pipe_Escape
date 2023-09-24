using UnityEngine;

public class Stat_Manager : MonoBehaviour
{
    [SerializeField] private int life = 1;
    public void TakeDamage()
    {
        life--;
        if (life <= 0)
        {
            Dead();
        }
    }
    public virtual void Dead()
    {
    }
    public virtual void UsingMapItem()
    {

    }
    public virtual void ExitingMapItem()
    {

    }
}