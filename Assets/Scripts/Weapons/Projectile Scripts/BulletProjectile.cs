using System.Collections;
using System.Collections.Generic;
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
    public LayerMask enemyLayerMask;

    public Transform lastEnemyHit;

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
            // basic projectile
            if (projectileInfo.projectileEffectType == ProjectileScriptableObject.ProjectileBulletEffect.Basic)
            {
                //Debug.Log("basic projectile");


                baseWeapon.ProjectileDealDamage(collision, false, 0); // damage

                DestroyProjectile(); // destroy
            }

            // piercing projectile
            else if (projectileInfo.projectileEffectType == ProjectileScriptableObject.ProjectileBulletEffect.Piercing)
            {
                //Debug.Log("piercing projectile");

                // CHECK FOR 0 FIRST
                if (projectileEffectCount <= 0) // out of pierce
                {
                   // Debug.Log("no pierce left");
                    baseWeapon.ProjectileDealDamage(collision, false, 0);
                    DestroyProjectile();
                }

                if (projectileEffectCount > 0) // has pierce left
                {
                    projectileEffectCount--;

                    //Debug.Log("pierce now remaining: " + projectileEffectCount);
                    baseWeapon.ProjectileDealDamage(collision, false, 0);

                }
            }

            //explosive projectile
            else if (projectileInfo.projectileEffectType == ProjectileScriptableObject.ProjectileBulletEffect.Explosive)
            {
                //Debug.Log("Bullet Type: " + projectileInfo.projectileEffectType);

                Collider2D[] enemyHits = Physics2D.OverlapCircleAll(transform.position, projectileEffectCount, enemyLayerMask); // find all enemies in explosive radius
                //Debug.Log("BOOOOM");

                // deal damage to each one
                foreach (var enemyHit in enemyHits)
                {
                    //Debug.Log("BAAAANG");

                    baseWeapon.ProjectileDealDamage(enemyHit, true, 0.5f);

                }

                DestroyProjectile();
            }

            // ricochet projectile
            else if (projectileInfo.projectileEffectType == ProjectileScriptableObject.ProjectileBulletEffect.Ricochet)
            {
                baseWeapon.ProjectileDealDamage(collision, false, 0);

                projectileEffectCount--;

                if (projectileEffectCount <= 0)
                {
                    DestroyProjectile();
                }

                lastEnemyHit = collision.transform;

                Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, 5f, enemyLayerMask); // find enemies in range


                Transform nextTarget = null; // reset target

                float closestDistance = Mathf.Infinity; // dist

                foreach (var enemy in nearbyEnemies)
                {
                    if (enemy.transform == lastEnemyHit)
                        continue;

                    float distance = Vector2.Distance(transform.position, enemy.transform.position); // get distance from this bullet to enemy

                    if (distance < closestDistance) // find the closet enemy
                    {
                        closestDistance = distance;
                        nextTarget = enemy.transform;
                    }
                }

                // if target is found
                if (nextTarget != null)
                {
                    Vector2 direction = (nextTarget.position - transform.position).normalized; // normalized needed

                    // aim at next enemy and go
                    Rigidbody2D rb = GetComponent<Rigidbody2D>();
                    rb.linearVelocity = direction * rb.linearVelocity.magnitude;
                }
                else
                {
                    Debug.Log("no target found for ricochet");
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
