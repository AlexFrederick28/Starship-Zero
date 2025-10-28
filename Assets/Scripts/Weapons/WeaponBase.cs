using UnityEngine;

public class WeaponBase : MonoBehaviour
{

    // base weapon class

    [Header("Stats")]
    [SerializeField] private string weaponName;
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private float critChance;
    [SerializeField] private float critDamage;
    [SerializeField] private GameObject projectileToFire;

    [SerializeField] private WeaponScriptableObject weaponType;


    void Start()
    {
        weaponName = weaponType.weaponName;
        damage = weaponType.damage;
        fireRate = weaponType.fireRate;
        critChance = weaponType.critChance;
        critDamage = weaponType.critDamage;
        projectileToFire = weaponType.projectileToFire;
    }

    void Update()
    {
        
    }

    protected void ItemScaling()
    {
        // scale weapon with players items
    }

    protected void FireProjectile()
    {
        // weapon fires projectile
    }

    protected void ProjectileDealDamage()
    {
        // weapon projectile deals damage
    }

    protected void DestroyProjectile()
    {
        // destroy weapon projectile
    }

}
