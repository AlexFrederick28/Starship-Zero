using JetBrains.Annotations;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawning : Difficulty
{
    [Space]
    [Header("Spawn Settings")]
    [Header("Percent Chance")]
    [SerializeField] private float currentEasySpawnChance;
    [SerializeField] private float currentMediumSpawnChance;
    [SerializeField] private float currentHardSpawnChance;
    [SerializeField] private float currentBossSpawnChance;
    [Header("Chance Multiplier")]
    [Tooltip("Default 100% (Spawn chance lowers as the difficulty progresses)")]
    [SerializeField] private float easySpawnChance;
    [Tooltip("Default -25% (Spawn chance increases as the difficulty progresses)")]
    [SerializeField] private float mediumSpawnChance;
    [Tooltip("Default -50% (Spawn chance increases as the difficulty progresses)")]
    [SerializeField] private float hardSpawnChance;
    [Tooltip("Default 50 (Amount of enemies to spawn DIVIDED BY Boss spawn chance)")]
    [SerializeField] private float bossSpawnChance;
    [Header("Amount")]
    [SerializeField] private float amountOfEnemiesToSpawn;
    [SerializeField] private float spawnAmountMulitplier;
    [SerializeField] private List<GameObject> easyEnemiesToSpawn;
    [SerializeField] private List<GameObject> mediumEnemiesToSpawn;
    [SerializeField] private List<GameObject> hardEnemiesToSpawn;
    [SerializeField] private List<GameObject> bossEnemiesToSpawn;
    [Space]
    [Header("Positions")]
    [SerializeField] private float spawnMinDistance;
    [SerializeField] private float spawnMaxDistance;
    [Header("Pool")]
    [SerializeField] private int totalPoolCount;
    [SerializeField] private int amountCurrentlyInPool;
    [SerializeField] private GameObject enemyBasePrefabToSpawn;
    [SerializeField] private GameObject poolParentToSpawn;
    [SerializeField] private GameObject poolParent;
    [SerializeField] private Stack<GameObject> enemyBasePool = new();

    private PlayerBase player;

    private void Start()
    {
        SpawnPool();
    }

    override public void Update()
    {
        base.Update();
        SpawnChance();

        if (player == null)
        {
            player = FindAnyObjectByType<PlayerBase>();
        }

        amountOfEnemiesToSpawn = (currentDifficulty / scalingSegments) * spawnAmountMulitplier;

        // here for testing - this should only start spawning enemies when entering a room
        SpawnNewEnemy();
    }

    private void SpawnPool()
    {
        if (poolParent == null)
        {
            poolParent = Instantiate(poolParentToSpawn, transform.position, Quaternion.identity);
        }
        for (int i = 0; i < totalPoolCount; i++)
        {
            if (poolParent != null)
            {
                if (enemyBasePool.Count != totalPoolCount)
                {
                    GameObject newEnemy = Instantiate(enemyBasePrefabToSpawn, transform.position, Quaternion.identity);
                    enemyBasePool.Push(newEnemy);
                    newEnemy.transform.SetParent(poolParent.transform);
                    newEnemy.SetActive(false);
                    amountCurrentlyInPool++;
                }
            }
        }
    }

    private void SpawnChance()
    {
        currentEasySpawnChance = easySpawnChance - amountOfEnemiesToSpawn;
        currentMediumSpawnChance = mediumSpawnChance + amountOfEnemiesToSpawn;
        currentHardSpawnChance = hardSpawnChance + amountOfEnemiesToSpawn;
        currentBossSpawnChance = amountOfEnemiesToSpawn / bossSpawnChance;
    }

    private void SpawnNewEnemy()
    {
        if (player != null)
        {
            if (enemyBasePool != null && enemyBasePool.Count != 0)
            {
                if (amountCurrentlyInPool > 0)
                {
                    if (amountOfEnemiesToSpawn > totalPoolCount - amountCurrentlyInPool)
                    {
                        if (currentEasySpawnChance > 0)
                        {
                            int randomNumb = Random.Range(1, 100);
                            if (randomNumb <= currentEasySpawnChance)
                            {
                                Debug.Log("Spawned EASY Enemy");
                                int randomEnemy = Random.Range(easyEnemiesToSpawn.Count, 0);
                                enemyBasePool.Pop();
                                Debug.Log(enemyBasePool.Count);
                                amountCurrentlyInPool--;
                            }
                        }
                        if (currentMediumSpawnChance > 0)
                        {
                            int randomNumb = Random.Range(1, 100);
                            if (randomNumb <= currentMediumSpawnChance)
                            {
                                Debug.Log("Spawned MEDIUM Enemy");
                                int randomEnemy = Random.Range(mediumEnemiesToSpawn.Count, 0);
                                enemyBasePool.Pop();
                                Debug.Log(enemyBasePool.Count);
                                amountCurrentlyInPool--;
                            }
                        }
                        if (currentHardSpawnChance > 0)
                        {
                            int randomNumb = Random.Range(1, 100);
                            if (randomNumb <= currentHardSpawnChance)
                            {
                                Debug.Log("Spawned HARD Enemy");
                                int randomEnemy = Random.Range(hardEnemiesToSpawn.Count, 0);
                                enemyBasePool.Pop();
                                Debug.Log(enemyBasePool.Count);
                                amountCurrentlyInPool--;
                            }
                        }
                        if (currentBossSpawnChance > 0)
                        {
                            int randomNumb = Random.Range(1, 100);
                            if (randomNumb <= currentBossSpawnChance)
                            {
                                Debug.Log("Spawned BOSS Enemy");
                                int randomEnemy = Random.Range(bossEnemiesToSpawn.Count, 0);
                                enemyBasePool.Pop();
                                Debug.Log(enemyBasePool.Count);
                                amountCurrentlyInPool--;
                            }
                        }
                    }
                }
            }
        }
    }
}
