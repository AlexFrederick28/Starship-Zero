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
            }

            currentHealth = value;
        }
    }

    [SerializeField] protected float damage;
    [SerializeField] protected float speed;
    [SerializeField] protected int level;
    public enum DifficultyType { Easy, Medium, Hard, Boss }
    public DifficultyType currentDifficultyType;

    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float cooldownTimer;
    [SerializeField] protected bool readyToAttack;

    protected virtual void Start()
    {
        enemyName = enemyType.enemyName;
        Health = enemyType.health;
        damage = enemyType.damage;
        speed = enemyType.speed;
        level = enemyType.level;

        // need to set enemy level to match player level
        // stats scale based off player level (refer to enemy data table in Starship Zero document)

        LevelScale();
    }

    protected virtual void OnEnable()
    {
        if (EnemyBrain.instance != null)
        {
            EnemyBrain.instance.MoveToPlayer += MoveToPlayer;
        }
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
        }
        if (cooldownTimer > attackCooldown)
        {
            readyToAttack = true;
        }

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerBase>())
        {
            if (collision.gameObject.GetComponent<PlayerBase>().Health != 0)
            {
                if (readyToAttack)
                {
                    Attack();
                    collision.gameObject.GetComponent<PlayerBase>().TakeDamage(damage);
                }
            }
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerBase>())
        {
            if (collision.gameObject.GetComponent<PlayerBase>().Health != 0)
            {
                if (readyToAttack)
                {
                    Attack();
                    collision.gameObject.GetComponent<PlayerBase>().TakeDamage(damage);
                }
            }
        }
    }
}
