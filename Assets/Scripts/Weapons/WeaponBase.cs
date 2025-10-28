using UnityEngine;

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
    [SerializeField] private GameObject projectileToFire;

    [SerializeField] private WeaponScriptableObject weaponType;



    protected virtual void Start()
    {
        weaponName = weaponType.weaponName;
        damage = weaponType.damage;
        fireRate = weaponType.fireRate;
        critChance = weaponType.critChance;
        critDamage = weaponType.critDamage;
        projectileToFire = weaponType.projectileToFire;
    }

    protected virtual void Update()
    {
        FireTimer();
    }

    protected void ItemScaling()
    {
        // scale weapon with players items
    }

    protected virtual void FireProjectile()
    {
        // weapon fires projectile

        Instantiate(projectileToFire, transform.position, Quaternion.identity); // TODO - fix when aiming is implemented

    }

    protected void ProjectileDealDamage()
    {
        // weapon projectile deals damage
    }

    protected void DestroyProjectile()
    {
        // destroy weapon projectile
    }

    protected void FireTimer()
    {
        if (canWeaponFire == true)
        {
            fireTime += Time.deltaTime * 2f; // TODO - x times player attack speed?
            if (fireTime >= fireRate)
            {
                fireTime -= fireRate;
                FireProjectile();
            }
        }   
    }

}
