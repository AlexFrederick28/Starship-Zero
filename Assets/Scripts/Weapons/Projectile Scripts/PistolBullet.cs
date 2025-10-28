using UnityEngine;

public class PistolBullet : MonoBehaviour
{

    [SerializeField] private float bulletSpeed;
    [SerializeField] private float projectileLifeTime;
    [SerializeField] private Rigidbody2D RB2D;

    void Start()
    {
        RB2D = GetComponent<Rigidbody2D>();
        RB2D.linearVelocity = transform.right * bulletSpeed;
        Destroy(gameObject, projectileLifeTime);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }

}
