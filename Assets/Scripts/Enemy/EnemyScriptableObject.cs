using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "Scriptable Objects/EnemyScriptableObject")]
public class EnemyScriptableObject : ScriptableObject
{
    public string enemyName;
    public float health;
    public float damage;
    public float speed;
    public int level;
    public float healthScaling;
    public float damageScaling;
    public float speedScaling;
}
