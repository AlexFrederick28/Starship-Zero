using Unity.VisualScripting;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{

    [SerializeField] private float bulletSpeed;
    [SerializeField] private float projectileLifeTime;
    [SerializeField] private Rigidbody2D RB2D;

    public WeaponBase baseWeapon;

    private void Start()
    {
        MoveProjectile();
        Destroy(gameObject, projectileLifeTime);
    }

    protected void MoveProjectile() // push projectile forward // TODO - dont add velocity to attacks only projectiles
    {
        RB2D = GetComponent<Rigidbody2D>();
        RB2D.linearVelocity = transform.right * bulletSpeed;

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        //Debug.Log("BP trigger");

        if (collision.GetComponent<BulletProjectile>() == true)
        {
            //Debug.Log("BP hit self");
            return; // hit projectile dupe
        }

        if (collision.GetComponent<EnemyBase>() == true)
        {
            //Debug.Log("BP enemy hit");
            baseWeapon.ProjectileDealDamage(collision);
        }

        baseWeapon.DestroyProjectile(gameObject);

    }

}
