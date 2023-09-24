using UnityEngine;

public class Knife : MonoBehaviour
{
    private string ownerTag;
    private int speed = 5;
    private float destroyTime = 30;
    private float destroyTimeNext;
    private bool sendKnife;
    private bool used;
    private Vector3 direction1 = Vector3.right;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ownerTag))
        {
            return;
        }
        if (other.CompareTag("Enemy"))
        {
            if (!used)
            {
                used = true;
                other.GetComponent<Enemy>().TakeDamage();
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag("Player"))
        {
            if (!used)
            {
                used = true;
                other.GetComponent<Player>().TakeDamage();
                Destroy(gameObject);
            }
        }
    }
    public void SetSpeed(Vector3 direction, string ownerTag)
    {
        sendKnife = true;
        this.direction1 = direction;
        this.ownerTag = ownerTag;
    }
    private void Update()
    {
        transform.Translate(direction1 * speed * Time.deltaTime);
        if (sendKnife)
        {
            destroyTimeNext += Time.deltaTime;
            if (destroyTimeNext > destroyTime)
            {
                Destroy(gameObject);
            }
        }
    }
}