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

    // if it is shotgun it uses different code for firing like a shotgun
    public bool isWeaponShotgun; // when shotgun shoot 5? projectiles

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
    private AudioClip onFireSound;

    [SerializeField] private float fireTime;
    [SerializeField] private float detectionRadius = 5f; // default 5, subject to change
    [SerializeField] private Transform targetToAttack;
    [SerializeField] private bool canWeaponFire;

    public InventoryItemPackage.Rarity rarity;

    // bullets from weapon
    [SerializeField] protected GameObject newBullet; // newest bullet
    [SerializeField] public List<GameObject> currentBullets; // all bullets

    public LayerMask enemiesLayerMask;

    [Header("References")]

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

            GameState.instance.OnCompletedInfestedClear += ResetToRecordedWeaponStatsOnInfestedRoomCompletion;

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

        GameState.instance.OnCompletedInfestedClear -= ResetToRecordedWeaponStatsOnInfestedRoomCompletion;

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

    public void ResetToRecordedWeaponStatsOnInfestedRoomCompletion(Room room)
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
        if (targetToAttack != null)
        {

            SoundManager.instance.PlaySoundClip(onFireSound, transform, SoundManager.instance.SoundVolume(), true, true, SoundManager.instance.defaultMinPitch, SoundManager.instance.defaultMaxPitch);

            if (isWeaponShotgun != true) // not a shotgun
            {
                // direction to fire forward
                Vector3 vectorToTarget = targetToAttack.position - transform.position;
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

            else if (isWeaponShotgun != false) // is a shotgun
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector3 vectorToTarget = targetToAttack.position - transform.position;
                    float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;

                    float randomSpread = Random.Range(-30f, 30f);  // 30 degree shotgun spread
                    Quaternion rotation = Quaternion.AngleAxis(angle + randomSpread, Vector3.forward);

                    newBullet = Instantiate(projectileToFire, transform.position, rotation); // create bullet

                    BulletProjectile bullet = newBullet.GetComponent<BulletProjectile>();
                    currentBullets.Add(newBullet);
                    if (bullet != null)
                    {
                        bullet.baseWeapon = this; // owner that created bullet
                    }
                }
                
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

                FindClosetTarget(); // find
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
        var hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemiesLayerMask);

        float firstClosestDistance = detectionRadius;
        float secondClosestDistance = detectionRadius;

        Transform closestTarget = null;
        Transform secondClosestTarget = null;

        targetToAttack = null; // reset

        // OLD
        //foreach (var hit in hits)
        //{
        //    if (hit.gameObject.GetComponentInParent<EnemyBase>() == true)
        //    {
        //        float distanceToTarget = Vector2.Distance(transform.position, hit.transform.position);

        //        if (distanceToTarget < firstClosestDistance) // if closest
        //        {
        //            firstClosestDistance = distanceToTarget;
        //            targetToAttack = hit.transform; // set as target
        //            //Debug.Log(closestTarget.name);
        //        }
        //    }
        //}

        foreach (var hit in hits)
        {
            if (hit.gameObject.GetComponentInParent<EnemyBase>() != null) // make sure it has an enemy base
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);

                if (distance < firstClosestDistance)
                {
                    // make first closest to second closest
                    secondClosestDistance = firstClosestDistance;
                    secondClosestTarget = closestTarget;

                    // new closeset enemy
                    firstClosestDistance = distance;
                    closestTarget = hit.transform;
                }
                else if (distance < secondClosestDistance)
                {
                    // second closest enemy
                    secondClosestDistance = distance;
                    secondClosestTarget = hit.transform;
                }
            }
        }


        float randomValue = Random.value;

        //Debug.Log(randomValue + "randomVal");


        // pick target - 50/50 chance for first or second
        if (randomValue < 0.5f && closestTarget != null) 
        {
            targetToAttack = closestTarget;
        }

        else if (secondClosestTarget != null)
        {
            targetToAttack = secondClosestTarget;
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
            isWeaponShotgun = false;
            onFireSound = null;
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
            isWeaponShotgun = weaponType.isShotgun;
            onFireSound = weaponType.fireSound;
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
