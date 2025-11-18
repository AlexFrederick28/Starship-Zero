using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using System.Collections.Generic;

public class WeaponBase : MonoBehaviour
{

    // base weapon class - has all the functionality for weapons 

    [Header("Base Settings")]
    [SerializeField] private string weaponName;

    // TODO - add base and modified stats to weapon
    [SerializeField] private float baseDamage;
    [SerializeField] private float baseFireRate;
    [SerializeField] private float baseCritChance;
    [SerializeField] private float baseCritDamage;

    [SerializeField] public float projectileSpeed;
    [SerializeField] private GameObject projectileToFire;
    [SerializeField] private WeaponScriptableObject weaponType;

    [Header("Modified Stats")]
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private float critChance;
    [SerializeField] private float critDamage;

    [Header("Other")]
    [SerializeField] private float fireTime;
    [SerializeField] private float detectionRadius = 5f; // default 5, subject to change
    [SerializeField] private Transform closestTarget;
    [SerializeField] private bool canWeaponFire;

    // bullets from weapon
    [SerializeField] protected GameObject newBullet; // newest bullet
    [SerializeField] public List<GameObject> currentBullets; // all bullets

    [Header("References (item stuff for now)")]

    [SerializeField] public ItemManager itemManager;

    protected virtual void Start()
    {
        if (itemManager == null)
        {
            itemManager = FindFirstObjectByType<ItemManager>();
        }

        weaponName = weaponType.weaponName;
        baseDamage = weaponType.damage;
        baseFireRate = weaponType.fireRate;
        baseCritChance = weaponType.critChance;
        baseCritDamage = weaponType.critDamage;
        projectileSpeed = weaponType.projectileSpeed;
        projectileToFire = weaponType.projectileToFire;    
    }

    protected virtual void Update()
    {
        FireTimer();
    }

    private void FixedUpdate()
    {
        FindClosetTarget();
    }

    public void ItemScaling() // scale weapon stats with players items - TODO
    {
        // (regions in order of the item manager list)
        #region Damage 
        if (itemManager.itemCountGO[0] > 0f) // if at least 1 itemas
        {
            // convert to float for decimal calculation
            float amount = itemManager.itemCountGO[0];
            float scale = itemManager.itemScalingGO[0];

            //Debug.Log("ItemCount [" + amount + "], Item Scaling [" + scale + "]");
            float itemModifier = amount * scale; // amount to modify by
            //Debug.Log("Item: Damage Increase [" + itemModifier + "%]");

            damage = baseDamage * (1f + itemModifier / 100f); // weapons damage
        }
        else // if no items (0 or less damage
        {
            damage = baseDamage;
        }

        //Debug.Log(weaponName + " Damage = [" + damage + "]");
        #endregion

        #region Crit Chance
        // TODO
        #endregion

        #region Crit Damage
        // TODO
        #endregion

        #region Fire Rate
        if (itemManager.itemCountGO[3] > 0f) // if at least 1 itemas
        {
            // convert to float for decimal calculation
            float amount = itemManager.itemCountGO[3];
            float scale = itemManager.itemScalingGO[3];

            //Debug.Log("ItemCount [" + amount + "], Item Scaling [" + scale + "]");
            float itemModifier = amount * scale; // amount to modify by
            //Debug.Log("Item: Attack Speed Increase [" + itemModifier + "%]");

            fireRate = baseFireRate * (1f + itemModifier / 100f); // weapons damage
        }
        else // if no items (0 or less damage
        {
            fireRate = baseFireRate;
        }

        //Debug.Log(weaponName + " Attack Speed = [" + fireRate + "]");
        #endregion


    }

    protected virtual void FireProjectile() // weapon fires projectile
    {
        if (closestTarget != null)
        {
            // direction to fire forward
            Vector3 vectorToTarget = closestTarget.position - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            newBullet = Instantiate(projectileToFire, transform.position, rotation); // create bullet

            BulletProjectile bullet = newBullet.GetComponent<BulletProjectile>();
            currentBullets.Add(newBullet);
            if (bullet != null)
            {
                bullet.baseWeapon = this; // owner that created bullet
            }
        }

        else
        {
            //Debug.Log("No target detected to fire");
        }
    }

    //protected virtual void FireAttack() // variation ^^^ 
    //{
    //    newBullet = Instantiate(projectileToFire, transform.position, Quaternion.identity);

    //    BulletProjectile bullet = newBullet.GetComponent<BulletProjectile>();
    //    currentBullets.Add(newBullet);
    //    if (bullet != null)
    //    {
    //        bullet.baseWeapon = this;
    //    }
    //}

    public void ProjectileDealDamage(Collider2D collision) // weapon projectile deals damage
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
        enemy.TakeDamage(finalDamage); // final damage

    }

    public void DestroyProjectile(GameObject projectileGO) // destroy weapon projectile
    {    
        currentBullets.Remove(projectileGO);
        Destroy(projectileGO, 0.01f); // small note - if not 0.01s destruction sometimes code above wont play
    }

    protected virtual void FireTimer() // controls the fire rate of each weapon
    {
        if (canWeaponFire == true)
        {
            fireTime += Time.deltaTime * fireRate; // fire speed
            if (fireTime >= 1f)
            {
                fireTime = 0f; // reset timer

                FireProjectile(); // fire

                //if (isProjectile == false) // variation
                //{
                //    FireAttack();
                //}

            }
        }   
    }

    protected void FindClosetTarget() // closet enemy for weapon to fire at
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        float shortestDistance = detectionRadius;
        closestTarget = null; // reset

        foreach (var hit in hits)
        {
            if (hit.gameObject.GetComponent<EnemyBase>() == true)
            {
                float distanceToTarget = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToTarget < shortestDistance) // if closest
                {
                    shortestDistance = distanceToTarget;
                    closestTarget = hit.transform; // set as target
                    //Debug.Log(closestTarget.name);
                }
            }
        }
    }

    //void OnDrawGizmos() // visual for attack range radius of the weapon
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(transform.position, detectionRadius);
    //}


}
