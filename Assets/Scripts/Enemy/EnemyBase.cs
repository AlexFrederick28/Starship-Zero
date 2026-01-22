using Unity.Multiplayer.Center.Common;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private EnemyScriptableObject enemyType;
    [SerializeField] private string enemyName;
    [SerializeField] private float currentHealth;
    [SerializeField] protected float maxHealth;

    public float Health
    {
        get
        {
            return currentHealth;
        }
        set
        {
            if (value > maxHealth)
            {
                value = maxHealth;
            }
            if (value < 0)
            {
                value = 0;
                Death();
            }

            currentHealth = value;
        }
    }

    [SerializeField] protected float damage;
    [SerializeField] protected float speed;
    [SerializeField] protected int level;

    [Space]
    [Header("Stat Scaling")]
    [SerializeField] protected float healthScale;
    [SerializeField] protected float damageScale;
    [SerializeField] protected float speedScale;
    private bool scaledStats = false;

    public enum DifficultyType { Easy, Medium, Hard, Boss }
    public DifficultyType currentDifficultyType;

    [Space]
    [Header("Attack")]
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float cooldownTimer;
    [SerializeField] protected bool readyToAttack;

    protected virtual void Awake()
    {
        enemyName = enemyType.enemyName;
        Health = enemyType.health;
        maxHealth = enemyType.health;
        damage = enemyType.damage;
        speed = enemyType.speed;
        level = enemyType.level;
        healthScale = enemyType.healthScaling;
        damageScale = enemyType.damageScaling;
        speedScale = enemyType.speedScaling;
    }

    protected virtual void OnEnable()
    {
        if (EnemyBrain.instance != null)
        {
            EnemyBrain.instance.MoveToPlayer += MoveToPlayer;
        }

        if (scaledStats == false)
        {
            // only scales the stats once per mite - this will have to be updated if the spawn pools are changed in future
            LevelScale();
            scaledStats = true;
        }

        cooldownTimer = attackCooldown;
        readyToAttack = true;
    }

    protected virtual void OnDisable()
    {
        if (EnemyBrain.instance != null)
        {
            EnemyBrain.instance.MoveToPlayer -= MoveToPlayer;
        }
    }

    protected virtual void Update()
    {
        AttackCooldown();
    }

    protected void LevelScale()
    {
        // scales enemy stats with levels to adjust game difficulty
        if (Spawning.instance != null)
        {
            for (int i = 0; i < QuestManager.instance.activeQuests.Count; i++)
            {
                if (QuestManager.instance.activeQuests[i].prerequisite.id == Spawning.instance.questID)
                {
                    // checking the spawn ID matches an active quest and sets the enemies level
                    Debug.Log("Set new enemy level to quest level");
                    level = QuestManager.instance.activeQuests[i].prerequisite.level;
                    maxHealth += (healthScale * level);
                    Health += (healthScale * level);
                    damage += (damageScale * level);
                    speed += (speedScale * level);
                }
            }
        }
    }

    protected virtual void Attack()
    {
        // enemy attack player - mostly used for animations

        Debug.Log("Enemy used Attack!");
        cooldownTimer = 0f;
        readyToAttack = false;
    }

    public void TakeDamage(float damage)
    {
        // enemy take damage from player

        Health -= damage;
        Debug.Log(name + " took " + damage + " damage!");
    }

    protected void Death()
    {
        // enemy death

        if (Health == 0)
        {
            Spawning.instance.GetComponent<Experience>().RemoveFromPool(Spawning.instance.GetComponent<Experience>().selectedExperiencePoint, currentDifficultyType, transform);
            Spawning.instance.AddToPool(gameObject);
            Spawning.instance.EnemyDeath(currentDifficultyType);
            Health = maxHealth;
        }
    }

    protected void MoveToPlayer(Transform playerTransform)
    {
        // locate player and move directly to them at a constant speed

        Vector3 targetPosition = (playerTransform.position - transform.position).normalized;
        transform.position += targetPosition * speed * Time.deltaTime;
    }

    protected void AttackCooldown()
    {
        if (readyToAttack == false)
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer > attackCooldown)
            {
                readyToAttack = true;
            }
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (readyToAttack == true)
        {
            if (collision.gameObject.GetComponent<PlayerBase>())
            {
                if (collision.gameObject.GetComponent<PlayerBase>().Health != 0)
                {
                    Attack();
                    collision.gameObject.GetComponent<PlayerBase>().TakeDamage(damage);
                }
            }
        }
        }
    }
