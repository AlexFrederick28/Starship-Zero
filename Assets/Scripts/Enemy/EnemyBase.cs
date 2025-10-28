using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private EnemyScriptableObject enemyType;
    [SerializeField] private string enemyName;
    [SerializeField] private float health;
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private int level;
    [SerializeField] private float attackCooldown;

    private void Start()
    {
        enemyName = enemyType.enemyName;
        health = enemyType.health;
        damage = enemyType.damage;
        speed = enemyType.speed;
        level = enemyType.level;

        // need to set enemy level to match player level
        // stats scale based off player level (refer to enemy data table in Starship Zero document)

        LevelScale();
    }

    private void LevelScale()
    {
        // scales enemy stats with levels to adjust game difficulty

    }

    protected void Attack()
    {
        // enemy attack player

    }

    protected void TakeDamage(float damage)
    {
        // enemy take damage from player

    }

    protected void Death()
    {
        // enemy death

    }

    protected void MoveToPlayer()
    {
        // locate player and move directly to them at a constant speed
    }
}
