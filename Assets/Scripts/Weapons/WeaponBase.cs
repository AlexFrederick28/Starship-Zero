using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class WeaponBase : MonoBehaviour
{

    // base weapon class - has all the functionality for weapons 

    [Header("Base Settings")]
    public string weaponName;
    public string weaponDescription;
    public int weaponLevel = 0;

    [SerializeField] private float baseDamage;
    [SerializeField] private float baseFireRate;
    [SerializeField] private float baseCritChance;
    [SerializeField] private float baseCritDamage;

    [SerializeField] public float projectileSpeed;
    public GameObject projectileToFire;
    public WeaponScriptableObject weaponType;

    [Header("Modified Stats")]
    public float damage;
    public float fireRate;
    public float critChance;
    public float critDamage;
    public int bulletEffectCount;

    // to track what a weapons stats are before entering an infested room
    private float recordedDamage;
    private float recordedFireRate;
    private float recordedCritChance;
    private float recordedCritDamage;
    public int recordedBulletEffectCount;
    private bool addedPermanentStatsToWeapon = false;

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

    public static Action<float, EnemyBase> OnDamagingEnemy;

    public ProjectileScriptableObject projectileSOInfo;

    protected void OnEnable()
    {
        if (GameState.instance != null)
        {
            GameState.instance.OnPlayerLevelUp += PauseWeapon;
            LevelUpManager.OnCardChosen += PauseWeapon;

            GameState.instance.OnEnteringInfestedRoom += RandomiseWeaponFireTime;
            GameState.instance.OnEnteringInfestedRoom += ResetWeaponStats;
            GameState.instance.OnEnteringInfestedRoom += AddStatsFromPlayerPermanentUpgrades;
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
        LevelUpManager.OnCardChosen -= PauseWeapon;

        GameState.instance.OnEnteringInfestedRoom -= RandomiseWeaponFireTime;
        GameState.instance.OnEnteringInfestedRoom -= ResetWeaponStats;
        GameState.instance.OnEnteringInfestedRoom -= AddStatsFromPlayerPermanentUpgrades;
        GameState.instance.OnEnteringInfestedRoom -= RecordWeaponStats;

        GameState.instance.OnCompletedInfestedClear -= ResetToRecordedWeaponStats;

        GameState.instance.OnPlayerRetry -= RandomiseWeaponFireTime;
        GameState.instance.OnPlayerRetry -= PauseWeapon;
        GameState.instance.OnPlayerRetry -= ResetToRecordedWeaponStats;

        GameState.instance.OnPlayerRespawn -= PauseWeapon;
        GameState.instance.OnPlayerRespawn -= ResetToRecordedWeaponStats;

        PlayerBase.OnPlayerDeath -= PauseWeapon;

        ResetWeaponStats();
        addedPermanentStatsToWeapon = false;
    }

    protected virtual void Start()
    {
        if (cardManager == null)
        {
            cardManager = FindFirstObjectByType<CardManager>();
        }

        //bulletEffectCount = projectileSOInfo.effectCount;
        AssignBaseWeaponStats();
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

    public void AddStatsFromPlayerPermanentUpgrades()
    {
        if (addedPermanentStatsToWeapon == true ) { return; }

        if (GameState.instance.player.damage > 0)
        {
            // percent damage increase 
            float damageIncrease = (GameState.instance.player.damage / 100) * damage;
            if (damageIncrease < 1) { damageIncrease = 1; }
            damage += (int)damageIncrease;
        }

        if (GameState.instance.player.critChance > 0)
        {
            // percent crit chance increase 
            float critChanceIncrease = (GameState.instance.player.critChance / 100) * critChance;
            if (critChanceIncrease < 1) { critChanceIncrease = 1; }
            critChance += (int)critChanceIncrease;
        }

        if (GameState.instance.player.critDamage > 0)
        {
            // percent crit damage increase 
            float critDamageIncrease = (GameState.instance.player.critDamage / 100) * critDamage;
            if (critDamageIncrease < 1) { critDamageIncrease = 1; }
            critDamage += (int)critDamageIncrease;
        }

        if (GameState.instance.player.fireRate > 0)
        {
            // percent fire rate increase 
            float fireRateIncrease = (GameState.instance.player.fireRate / 100) * fireRate;
            if (fireRateIncrease < 1) { fireRateIncrease = 1; }
            fireRate += (int)fireRateIncrease;
        }

        addedPermanentStatsToWeapon = true;
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
        recordedBulletEffectCount = bulletEffectCount;
}

    public void ResetWeaponStats()
    {
        damage = baseDamage;
        fireRate = baseFireRate;
        critChance = baseCritChance;
        critDamage = baseCritDamage;
        bulletEffectCount = 1;
        addedPermanentStatsToWeapon = false;

        //AddStatsFromPlayerPermanentUpgrades();
    }

    public void ResetToRecordedWeaponStats()
    {
        damage = recordedDamage;
        fireRate = recordedFireRate;
        critChance = recordedCritChance;
        critDamage = recordedCritDamage;
        bulletEffectCount = recordedBulletEffectCount;
        weaponLevel = 0;
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

    // weapon projectile deals damage, cut damage by amount 0-1 e.g. 0.8 = 80% damage
    public void ProjectileDealDamage(Collider2D collision, bool cutDamage, float amountToCut) 
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

        if (cutDamage == true)
        {
            finalDamage = finalDamage * amountToCut;
        }

        EnemyBase enemy = collision.gameObject.GetComponentInParent<EnemyBase>();
        // a damage event that is used by a CardBase (a physical card object) to add its affect from this hit
        OnDamagingEnemy?.Invoke(finalDamage, enemy);
        
        Debug.Log(enemy + " enemy target");
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
        AssignBaseWeaponStats();
        ResetWeaponStats();
    }

    public void RemoveWeapon()
    {
        weaponType = null;
        AssignBaseWeaponStats();
    }

    // gives the weapon its base stats
    public void AssignBaseWeaponStats() 
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

    public void ChangeProjectileType(GameObject projectile)
    {
        projectileToFire = projectile;
    }

    // visual for attack range radius of the weapon
    //void OnDrawGizmos() 
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(transform.position, detectionRadius);
    //}
}
