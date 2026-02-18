using UnityEngine;


[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "Scriptable Objects/WeaponScriptableObject")]
public class WeaponScriptableObject : ScriptableObject
{
    public string weaponName;
    public string weaponDescription;
    public Sprite weaponSprite;
    public float damage;
    public float fireRate;
    public float critChance;
    public float critDamage;
    public float projectileSpeed;
    public GameObject projectileToFire;

}
