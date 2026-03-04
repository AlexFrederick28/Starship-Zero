using Unity.VisualScripting;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{

    // projectile - functionality of the bullet that is fired from weapons

    [SerializeField] private float projectileLifeTime; // self destruct time
    [SerializeField] private Rigidbody2D RB2D;

    public WeaponBase baseWeapon; // owner of this bullet created (used to refer back to it when dealing damage)

    private void Start()
    {
        MoveProjectile();
        Destroy(gameObject, projectileLifeTime);
    }

    protected void MoveProjectile() // single instanceto push 0 gravity projectile
    {
        RB2D = GetComponent<Rigidbody2D>();
        RB2D.linearVelocity = transform.right * baseWeapon.projectileSpeed;

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        //Debug.Log("BP trigger");

        if (collision.GetComponent<BulletProjectile>() == true)
        {
            //Debug.Log("BP hit self");
            return; // hit projectile dupe
        }

        if (collision.GetComponentInParent<EnemyBase>() == true)
        {
            //Debug.Log("BP enemy hit");
            baseWeapon.ProjectileDealDamage(collision);
        }

        baseWeapon.DestroyProjectile(gameObject);

    }

}
