using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{

    // base weapon class - has all the functionality for weapons 

    [Header("Base Settings")]
    public string weaponName;
    public string weaponDescription;
    public int weaponLevel = 0;

    // TODO - add base and modified stats to weapon
    [SerializeField] private float baseDamage;
    [SerializeField] private float baseFireRate;
    [SerializeField] private float baseCritChance;
    [SerializeField] private float baseCritDamage;

    [SerializeField] public float projectileSpeed;
    [SerializeField] private GameObject projectileToFire;
    public WeaponScriptableObject weaponType;

    [Header("Modified Stats")]
    public float damage;
    public float fireRate;
    public float critChance;
    public float critDamage;

    // to track what a weapons stats are before entering an infested room
    private float recordedDamage;
    private float recordedFireRate;
    private float recordedCritChance;
    private float recordedCritDamage;

    [Header("Other")]
    [SerializeField] private float fireTime;
    [SerializeField] private float detectionRadius = 5f; // default 5, subject to change
    [SerializeField] private Transform closestTarget;
    [SerializeField] private bool canWeaponFire;

    public InventoryItemPackage.Rarity rarity;

    // bullets from weapon
    [SerializeField] protected GameObject newBullet; // newest bullet
    [SerializeField] public List<GameObject> currentBullets; // all bullets

    [Header("References (item stuff for now)")]

    [SerializeField] public CardManager cardManager;

    protected void OnEnable()
    {
        if (GameState.instance != null)
        {
            GameState.instance.OnPlayerLevelUp += PauseWeapon;

            GameState.instance.OnEnteringInfestedRoom += RandomiseWeaponFireTime;
            GameState.instance.OnEnteringInfestedRoom += ResetWeaponStats;
            GameState.instance.OnEnteringInfestedRoom += RecordWeaponStats;

            GameState.instance.OnCompletedInfestedClear += ResetToRecordedWeaponStats;

            GameState.instance.OnPlayerRetry += RandomiseWeaponFireTime;
            GameState.instance.OnPlayerRetry += PauseWeapon;
            GameState.instance.OnPlayerRetry += ResetToRecordedWeaponStats;

            GameState.instance.OnPlayerRespawn += PauseWeapon;
            GameState.instance.OnPlayerRespawn += ResetToRecordedWeaponStats;
        }

        PlayerBase.OnPlayerDeath += PauseWeapon;
    }

    protected void OnDisable()
    {
        GameState.instance.OnPlayerLevelUp -= PauseWeapon;

        GameState.instance.OnEnteringInfestedRoom -= RandomiseWeaponFireTime;
            GameState.instance.OnEnteringInfestedRoom -= ResetWeaponStats;
        GameState.instance.OnEnteringInfestedRoom -= RecordWeaponStats;

        GameState.instance.OnCompletedInfestedClear -= ResetToRecordedWeaponStats;

        GameState.instance.OnPlayerRetry -= RandomiseWeaponFireTime;
        GameState.instance.OnPlayerRetry -= PauseWeapon;
        GameState.instance.OnPlayerRetry -= ResetToRecordedWeaponStats;

        GameState.instance.OnPlayerRespawn -= PauseWeapon;
        GameState.instance.OnPlayerRespawn -= ResetToRecordedWeaponStats;

        PlayerBase.OnPlayerDeath -= PauseWeapon;
    }

    protected virtual void Start()
    {
        if (cardManager == null)
        {
            cardManager = FindFirstObjectByType<CardManager>();
        }

        AssignWeaponStats();
    }

    protected virtual void Update()
    {
        if (GameState.instance.currentState != GameState.States.RoomClear) { return; }
        FireTimer();
    }

    private void FixedUpdate()
    {
        FindClosetTarget();
    }

    public void PauseWeapon()
    {
        if (canWeaponFire == true)
        {
            canWeaponFire = false;
        }
        else if (canWeaponFire == false)
        {
            canWeaponFire = true;
        }
    }

    public void RecordWeaponStats()
    {
        recordedDamage = damage;
        recordedFireRate = fireRate;
        recordedCritChance = critChance;
        recordedCritDamage = critDamage;
    }

    public void ResetWeaponStats()
    {
        damage = baseDamage;
        fireRate = baseFireRate;
        critChance = baseCritChance;
        critDamage = baseCritDamage;
    }

    public void ResetToRecordedWeaponStats()
    {
        damage = recordedDamage;
        fireRate = recordedFireRate;
        critChance = recordedCritChance;
        critDamage = recordedCritDamage;
        weaponLevel = 0;
    }

    //public void ItemScaling()
    //{
    //    // this function is scaling weapon damage, crit chance, crit damage, and fire rate based off of a set in stone item positions 
    //    if (cardManager == null)
    //    {
    //        cardManager = FindFirstObjectByType<CardManager>();
    //    }

    //    // (regions in order of the item manager list, 0 = first in list)
    //    #region Damage 
    //    if (cardManager.itemCountGO[0] > 0f) // if at least 1 itemas
    //    {
    //        // convert to float for decimal calculation
    //        float amount = cardManager.itemCountGO[0];
    //        float scale = cardManager.itemScalingGO[0];

    //        //Debug.Log("ItemCount [" + amount + "], Item Scaling [" + scale + "]");
    //        float itemModifier = amount * scale; // amount to modify by
    //        //Debug.Log("Item: Damage Increase [" + itemModifier + "%]");

    //        damage = baseDamage * (1f + itemModifier / 100f); // weapons damage
    //    }
    //    else // if no items (0 or less damage
    //    {
    //        //Debug.Log("else = base damage");
    //        damage = baseDamage;
    //    }

    //    //Debug.Log(weaponName + " Damage = [" + damage + "]");
    //    #endregion

    //    #region Crit Chance
    //    if (cardManager.itemCountGO[1] > 0f)
    //    {
    //        // convert
    //        float amount = cardManager.itemCountGO[1];
    //        float scale = cardManager.itemScalingGO[1];

    //        //Debug.Log("ItemCount [" + amount + "], Item Scaling [" + scale + "]");
    //        float itemModifier = amount * scale;

    //        critChance = baseCritChance + itemModifier;

    //    }
    //    else
    //    {
    //        critChance = baseCritChance;
    //    }

    //    //Debug.Log(weaponName + " Crit Chance = [" + critChance + "]");
    //    #endregion

    //    #region Crit Damage
    //    if (cardManager.itemCountGO[2] > 0f)
    //    {
    //        // convert
    //        float amount = cardManager.itemCountGO[2];
    //        float scale = cardManager.itemScalingGO[2];

    //        //Debug.Log("ItemCount [" + amount + "], Item Scaling [" + scale + "]");
    //        float itemModifier = amount * scale;

    //        critDamage = baseCritDamage + itemModifier;

    //    }
    //    else
    //    {
    //        critDamage = baseCritDamage;
    //    }

    //    //Debug.Log(weaponName + " Crit Damage = [" + critDamage + "]");
    //    #endregion

    //    #region Fire Rate
    //    if (cardManager.itemCountGO[3] > 0f) // if at least 1 itemas
    //    {
    //        // convert to float for decimal calculation
    //        float amount = cardManager.itemCountGO[3];
    //        float scale = cardManager.itemScalingGO[3];

    //        //Debug.Log("ItemCount [" + amount + "], Item Scaling [" + scale + "]");
    //        float itemModifier = amount * scale; // amount to modify by
    //        //Debug.Log("Item: Attack Speed Increase [" + itemModifier + "%]");

    //        fireRate = baseFireRate * (1f + itemModifier / 100f); // weapons damage
    //    }
    //    else // if no items (0 or less damage
    //    {
    //        fireRate = baseFireRate;
    //    }

    //    //Debug.Log(weaponName + " Attack Speed = [" + fireRate + "]");
    //    #endregion


    //}

    //protected void ModifyWeaponStats(float mod)
    //{
    //    damage = baseDamage * (1f + mod / 100f);
    //    critChance = baseCritChance + mod;
    //    critDamage = baseCritDamage + mod;
    //    fireRate = baseFireRate * (1f + mod / 100f);
    //}

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

        EnemyBase enemy = collision.gameObject.GetComponentInParent<EnemyBase>();
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

    // offset weapon firerate to not be the same
    public void RandomiseWeaponFireTime()
    {
        float randomTime = Random.Range(0f, 0.9f);
        //Debug.Log(randomTime + "ccccccccccc");

        fireTime = randomTime;
    }


    protected void FindClosetTarget() // closet enemy for weapon to fire at
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        float shortestDistance = detectionRadius;
        closestTarget = null; // reset

        foreach (var hit in hits)
        {
            if (hit.gameObject.GetComponentInParent<EnemyBase>() == true)
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

    public void ChangeWeaponType(WeaponScriptableObject type)
    {
        weaponType = type;
        AssignWeaponStats();
    }

    public void RemoveWeapon()
    {
        weaponType = null;
        AssignWeaponStats();
    }

    // gives the weapon its base stats
    public void AssignWeaponStats() 
    {
        if (weaponType == null) // no scriptable can be found
        {
            weaponName = string.Empty;
            weaponDescription = string.Empty;
            baseDamage = 0;
            baseFireRate = 0;
            baseCritChance = 0;
            baseCritDamage = 0;
            projectileSpeed = 0;
            projectileToFire = null;
        }
        else // found, set its base stats
        {
            weaponName = weaponType.weaponName;
            weaponDescription = weaponType.weaponDescription;
            baseDamage = weaponType.damage;
            baseFireRate = weaponType.fireRate;
            baseCritChance = weaponType.critChance;
            baseCritDamage = weaponType.critDamage;
            projectileSpeed = weaponType.projectileSpeed;
            projectileToFire = weaponType.projectileToFire;
        }
    }

    // visual for attack range radius of the weapon
    //void OnDrawGizmos() 
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(transform.position, detectionRadius);
    //}
}
