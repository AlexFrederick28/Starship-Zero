using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using System.Collections.Generic;

public class WeaponBase : MonoBehaviour
{

    // base weapon class

    [SerializeField] private float fireTime;
    [SerializeField] private bool canWeaponFire;

    [Header("Stats")]
    [SerializeField] private string weaponName;
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private float critChance;
    [SerializeField] private float critDamage;

    [Space] 

    [SerializeField] private GameObject projectileToFire;

    [SerializeField] private WeaponScriptableObject weaponType;

    [SerializeField] private Transform closestTarget;

    [SerializeField] private float detectionRadius = 5f; // default 5, subject to change

    [SerializeField] protected GameObject newBullet; // list?
    [SerializeField] protected List<GameObject> currentBullets; // list?


    protected virtual void Start()
    {
        weaponName = weaponType.weaponName;
        damage = weaponType.damage;
        fireRate = weaponType.fireRate;
        critChance = weaponType.critChance;
        critDamage = weaponType.critDamage;
        projectileToFire = weaponType.projectileToFire;

        ItemScaling();

        
    }

    protected virtual void Update()
    {
        FireTimer();
    }

    private void FixedUpdate()
    {
        FindClosetTarget();
    }

    protected void ItemScaling() // scale weapon stats with players items
    {
        
    }

    protected virtual void FireProjectile() // weapon fires projectile
    {
        if (closestTarget != null)
        {
            Vector3 vectorToTarget = closestTarget.position - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            newBullet = Instantiate(projectileToFire, transform.position, Quaternion.identity); // half list?
            currentBullets.Add(newBullet); // list?
        }

        else
        {
            Debug.Log("No Target detected on fire");
        }
    }

    protected virtual void FireAttack() // variation ^^^ - weapons that DONT want a rotation e.g. sword attack 
    {
        newBullet = Instantiate(projectileToFire, transform.position, Quaternion.identity);
        currentBullets.Add(newBullet);
    }

    protected void ProjectileDealDamage(Collider2D collision) // weapon projectile deals damage
    {

        float critRoll = Random.Range(0f, 100f);
        float finalDamage = 0f;

        if (critRoll <= critChance) // crit
        {
            finalDamage += damage * (1 + critDamage/100);
        }

        else // no crit
        {
            finalDamage += damage;
        }   


        EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
        enemy.TakeDamage(finalDamage); 

        currentBullets.Remove(newBullet); // list?
        DestroyProjectile();
    }

    protected void DestroyProjectile() // destroy weapon projectile
    {
        Destroy(gameObject);
        
    }

    protected virtual void FireTimer()
    {
        if (canWeaponFire == true)
        {
            fireTime += Time.deltaTime * 2f; // TODO - x times player attack speed? x 2f is just to fire 2x faster for now
            if (fireTime >= fireRate)
            {
                fireTime -= fireRate;
                FireProjectile();
            }
        }   
    }

    protected void FindClosetTarget()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        float shortestDistance = detectionRadius;
        closestTarget = null; // reset

        foreach (var hit in hits)
        {
            if (hit.gameObject.GetComponent<EnemyBase>() == true)
            {
                float distanceToTarget = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToTarget < shortestDistance)
                {
                    shortestDistance = distanceToTarget;
                    closestTarget = hit.transform;
                    Debug.Log(closestTarget);
                }
            }
        }
    }

    //void OnDrawGizmos() // visual for attack radius
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(transform.position, detectionRadius);
    //}


}
