using UnityEngine;

public class TempBullet : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyBase>())
        {
            collision.gameObject.GetComponent<EnemyBase>().TakeDamage(50f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyBase>())
        {
            collision.gameObject.GetComponent<EnemyBase>().TakeDamage(50f);
        }
    }
}
