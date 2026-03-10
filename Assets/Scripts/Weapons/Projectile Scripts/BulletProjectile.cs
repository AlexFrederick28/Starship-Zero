using Unity.VisualScripting;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{

    // projectile - functionality of the bullet that is fired from weapons

    [SerializeField] private float projectileLifeTime; // self destruct time
    [SerializeField] private Rigidbody2D RB2D;

    public WeaponBase baseWeapon; // owner of this bullet created (used to refer back to it when dealing damage)

    public ProjectileScriptableObject projectileInfo;

    public int projectileEffectCount;

    private void Start()
    {
        projectileEffectCount = baseWeapon.bulletEffectCount;
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
            if (projectileInfo.projectileEffectType == ProjectileScriptableObject.ProjectileBulletEffect.Basic)
            {
                //Debug.Log("basic projectile");


                baseWeapon.ProjectileDealDamage(collision); // damage

                DestroyProjectile(); // destroy
            }

            else if (projectileInfo.projectileEffectType == ProjectileScriptableObject.ProjectileBulletEffect.Piercing)
            {
                //Debug.Log("piercing projectile");

                // CHECK FOR 0 FIRST
                if (projectileEffectCount <= 0) // out of pierce
                {
                   // Debug.Log("no pierce left");
                    baseWeapon.ProjectileDealDamage(collision);
                    DestroyProjectile();
                }

                if (projectileEffectCount > 0) // has pierce left
                {
                    projectileEffectCount--;

                    //Debug.Log("pierce now remaining: " + projectileEffectCount);
                    baseWeapon.ProjectileDealDamage(collision);
                }
            }
            //Debug.Log("BP enemy hit");
            //baseWeapon.ProjectileDealDamage(collision);
        }

    }

    public void DestroyProjectile()
    {
        baseWeapon.DestroyProjectile(gameObject);
    }
}
