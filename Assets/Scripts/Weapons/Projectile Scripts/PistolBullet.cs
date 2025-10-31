using UnityEngine;

public class PistolBullet : MonoBehaviour
{

    [SerializeField] private float bulletSpeed;
    [SerializeField] private float projectileLifeTime;
    [SerializeField] private Rigidbody2D RB2D;

    public WeaponBase baseWeapon;

    private void Start()
    {
        MoveProjectile();
    }

    protected virtual void MoveProjectile() // push projectile forward
    {
        RB2D = GetComponent<Rigidbody2D>();
        RB2D.linearVelocity = transform.right * bulletSpeed;
        Destroy(gameObject, projectileLifeTime);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<EnemyBase>() == true)
        {
            // TODO - add damage from specific weapon e.g. bullet fired by pistol should take in pistol stats
        }
    }

}
