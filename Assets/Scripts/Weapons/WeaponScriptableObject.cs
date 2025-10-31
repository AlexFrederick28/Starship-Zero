using UnityEngine;


[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "Scriptable Objects/WeaponScriptableObject")]
public class WeaponScriptableObject : ScriptableObject
{
    public string weaponName;
    public float damage;
    public float fireRate;
    public float critChance;
    public float critDamage;
    public bool isProjectile;
    public GameObject projectileToFire;

}
