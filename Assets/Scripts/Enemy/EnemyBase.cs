using System.Collections;
using Unity.Multiplayer.Center.Common;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBase : MonoBehaviour 
{
    [Header("Base Settings")]
    [SerializeField] private GameObject view;
    [SerializeField] private GameObject[] colliders;
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyScriptableObject enemyType;
    [SerializeField] private string enemyName;
    [SerializeField] private float currentHealth;
    [SerializeField] protected float maxHealth;
    private float animationSpeed;

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
            if (value <= 0)
            {
                value = 0;
            }

            currentHealth = value;
            Death();
        }
    }

    [SerializeField] protected float damage;
    [SerializeField] protected float speed;
    [SerializeField] protected int level;
    public int experienceAdditive;

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

    [Space]
    [Header("Drops")]
    //[SerializeField] protected InventoryItem[] specimens = new InventoryItem[0];
    [SerializeField] protected InventoryItemPackage[] specimens = new InventoryItemPackage[0];
    private int totalSpecimenWeight;
    [Tooltip("Out of 100")]
    public int dropFrequencyPercentChance;
    [SerializeField] protected int recordedDropFrequencyPercentChance;

    [Space]
    [Header("Audio")]
    [SerializeField] protected float volume;
    [SerializeField] protected float minPitch;
    [SerializeField] protected float maxPitch;
    [SerializeField] protected AudioClip enemyHurtClip;

    private bool enemyPaused = false;

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

        animationSpeed = animator.speed;

        CalculateTotalSpecimenWeight();
    }

    protected virtual void OnEnable()
    {
        Spawning.instance.OnInfestedRoomReset += AddEnemyBackToSpawnPool;

        if (EnemyBrain.instance != null)
        {
            EnemyBrain.instance.MoveToPlayer += MoveToPlayer;
        }
        if (GameState.instance != null)
        {
            GameState.instance.OnPlayerLevelUp += PauseAndUnpauseEnemy;
            LevelUpManager.OnCardChosen += PauseAndUnpauseEnemy;

            GameState.instance.OnEnteringInfestedRoom += RecordVariablesOnRoomStart;
            GameState.instance.OnCompletedInfestedClear += ResetVariablesToRecorded;
            GameState.instance.OnPlayerRespawn += ResetVariablesToRecorded;
            GameState.instance.OnPlayerRetry += ResetVariablesToRecorded;
        }

        PlayerBase.OnPlayerDeath += PauseAndUnpauseEnemy;

        if (scaledStats == false)
        {
            // only scales the stats once per mite - this will have to be updated if the spawn pools are changed in future
            LevelScale();
            scaledStats = true;
        }

        cooldownTimer = attackCooldown;
        readyToAttack = true;

        // reset health on spawn to ensure max health (Incase the room is being re-attempted by the player)
        currentHealth = maxHealth;
    }

    protected virtual void OnDisable()
    {
        Spawning.instance.OnInfestedRoomReset -= AddEnemyBackToSpawnPool;

        if (EnemyBrain.instance != null)
        {
            EnemyBrain.instance.MoveToPlayer -= MoveToPlayer;
        }

        GameState.instance.OnPlayerLevelUp -= PauseAndUnpauseEnemy;
        LevelUpManager.OnCardChosen -= PauseAndUnpauseEnemy;

        GameState.instance.OnEnteringInfestedRoom -= RecordVariablesOnRoomStart;
        GameState.instance.OnCompletedInfestedClear -= ResetVariablesToRecorded;
        GameState.instance.OnPlayerRespawn -= ResetVariablesToRecorded;
        GameState.instance.OnPlayerRetry -= ResetVariablesToRecorded;

        PlayerBase.OnPlayerDeath -= PauseAndUnpauseEnemy;
    }

    protected virtual void Update()
    {
        if (enemyPaused == true) { return; }
        AttackCooldown();
    }

    public void RecordVariablesOnRoomStart()
    {
        recordedDropFrequencyPercentChance = dropFrequencyPercentChance;
    }

    public void ResetVariablesToRecorded()
    {
        dropFrequencyPercentChance = recordedDropFrequencyPercentChance;
        experienceAdditive = 0;
    }

    /// <summary>
    /// Makes the drop chance directly equal to the plugged in amount
    /// </summary>
    /// <param name="amount"></param>
    public void IncreaseDropChance(int amount)
    {
        dropFrequencyPercentChance = amount;
    }

    protected void LevelScale()
    {
        // scales enemy stats with levels to adjust game difficulty
        if (Spawning.instance != null)
        {
            if (Spawning.instance.entryQuestActivated == true)
            {
                level = (int)Spawning.instance.questLevel.x;
            }
            else
            {
                level = GameState.instance.player.Level;
            }

            maxHealth += (healthScale * level);
            Health += (healthScale * level);
            damage += (damageScale * level);
            speed += (speedScale * level);
        }
    }

    protected virtual void Attack()
    {
        // enemy attack player - mostly used for animations
        if (enemyPaused == true) { return; }
        Debug.Log("Enemy used Attack!");
        cooldownTimer = 0f;
        readyToAttack = false;
    }

    public void TakeDamage(float damage)
    {
        // enemy take damage from player
        if (enemyPaused == true) { return; }
        Health -= damage;
        SoundManager.instance.PlaySoundClip(enemyHurtClip, transform, volume, false, true, minPitch, maxPitch);
        Debug.Log(name + " took " + damage + " damage!");
    }

    protected void Death()
    {
        // enemy death

        if (Health == 0)
        {
            ExperiencePoint exp = Spawning.instance.experience.experiencePool[0];
            Spawning.instance.experience.RemoveFromPool(exp, currentDifficultyType, transform);
            exp.AddExperienceAdditive(experienceAdditive); // THIS KEEPS RETURNING NULL FML
            DropRandomSpecimen();
            Spawning.instance.AddToPool(gameObject);
            Spawning.instance.EnemyDeath(currentDifficultyType);
            ResetVariablesToRecorded();
            Health = maxHealth;
        }
    }

    protected void MoveToPlayer(Transform playerTransform)
    {
        // locate player and move directly to them at a constant speed
        if (enemyPaused == true) { return; }
        Vector3 targetPosition = (playerTransform.position - transform.position).normalized;
        transform.position += targetPosition * speed * Time.deltaTime;
        if (transform.position.x > playerTransform.position.x)
        {
            Quaternion newRotation = new Quaternion(0, 0, 0, 0);
            view.transform.rotation = newRotation;

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].transform.rotation = newRotation;
            }
        }
        else
        {
            Quaternion newRotation = new Quaternion(0, 180, 0, 0);
            view.transform.rotation = newRotation;

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].transform.rotation = newRotation;
            }
        }
    }

    protected void AttackCooldown()
    {
        if (enemyPaused == true) { return; }
        if (readyToAttack == false)
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer > attackCooldown)
            {
                readyToAttack = true;
            }
        }
    }

    protected void AddEnemyBackToSpawnPool()
    {
        if (enemyPaused == true)
        {
            // unpause the enemy for the next time they spawn 
            PauseAndUnpauseEnemy();
        }
        Spawning.instance.AddToPool(gameObject);
    }

    protected void PauseAndUnpauseEnemy()
    {
        if (enemyPaused == false)
        {
            animator.speed = 0.0f;
            enemyPaused = true;
        }
        else
        {
            animator.speed = animationSpeed;
            enemyPaused = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (readyToAttack == true)
        {
            if (collision.gameObject.GetComponentInParent<PlayerBase>())
            {
                if (collision.gameObject.GetComponentInParent<PlayerBase>().Health != 0)
                {
                    Attack();
                    collision.gameObject.GetComponentInParent<PlayerBase>().TakeDamage(damage);
                }
            }
        }
    }
    public void CalculateTotalSpecimenWeight()
    {
        for (int i = 0; i < specimens.Length; i++)
        {
            totalSpecimenWeight += specimens[i].dropChance;
        }
    }

    public void DropRandomSpecimen()
    {
        int rand = Random.Range(0, 100);

        if (rand < dropFrequencyPercentChance)
        {
            int randomNumb = Random.Range(0, totalSpecimenWeight);
            for (int i = 0; i < specimens.Length; i++)
            {
                if (randomNumb < specimens[i].dropChance)
                {
                    Spawning.instance.specimenPool.RemoveFromPool(Spawning.instance.specimenPool.selectedSpecimen, specimens[i], transform);
                    Debug.Log("Spawned new specimen");
                    return;
                }
                else
                {
                    randomNumb -= specimens[i].dropChance;
                    continue;
                }
            }
        }
    }
}
