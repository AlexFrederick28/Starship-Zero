using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileScriptableObject", menuName = "Scriptable Objects/ProjectileScriptableObject")]
public class ProjectileScriptableObject : ScriptableObject
{
    // projectile scriptable object - used for projectile info

    public string projectileName; // name to be displayed
    public string projectileDescription; // description to explain the projectile type
    public Sprite displaySprite; // image used to show the projectile
    public GameObject projectilePrefabToFire; // prefab to be fired
    public ProjectileBulletEffect projectileEffectType; // type

    public enum ProjectileBulletEffect
    {
        Basic,
        Piercing,
        // bounce
        // explosive
        // lightning/zap
    }
}
