using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;

public class Spawning : Difficulty
{
    [Space]
    [Header("Spawn Settings")]
    [Header("Percent Chance")]
    [SerializeField] private float currentEasySpawnChance;
    private float CurrentEasySpawnChance
    {
        get { return currentEasySpawnChance; }
        set
        {
            if (value > 100)
            {
                value = 100;
            }

            currentEasySpawnChance = value;
        }
    }
    [SerializeField] private float currentMediumSpawnChance;
    private float CurrentMediumSpawnChance
    {
        get { return currentMediumSpawnChance; }
        set
        {
            if (value > 100)
            {
                value = 100;
            }

            currentMediumSpawnChance = value;
        }
    }
    [SerializeField] private float currentHardSpawnChance;
    private float CurrentHardSpawnChance
    {
        get { return currentHardSpawnChance; }
        set
        {
            if (value > 100)
            {
                value = 100;
            }

            currentHardSpawnChance = value;
        }
    }
    [SerializeField] private float currentBossSpawnChance;
    private float CurrentBossSpawnChance
    {
        get { return currentBossSpawnChance; }
        set
        {
            if (value > 100)
            {
                value = 100;
            }

            currentBossSpawnChance = value;
        }
    }
    [Header("Chance Multiplier")]
    [Tooltip("Default 100% (Spawn chance lowers as the difficulty progresses)")]
    [SerializeField] private float easySpawnWeight;
    [Tooltip("Default -25% (Spawn chance increases as the difficulty progresses)")]
    [SerializeField] private float mediumSpawnWeight;
    [Tooltip("Default -50% (Spawn chance increases as the difficulty progresses)")]
    [SerializeField] private float hardSpawnWeight;
    [Tooltip("Default 50 (Amount of enemies to spawn DIVIDED BY Boss spawn chance)")]
    [SerializeField] private float bossSpawnWeight;
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
    [SerializeField] private GameObject spawnBoundOne;
    [SerializeField] private GameObject spawnBoundTwo;
    [Header("Pool")]
    [SerializeField] private int totalEnemyPoolCount;
    [SerializeField] private int easyEnemyPoolCount;
    [SerializeField] private int easyEnemiesSpawned;
    [SerializeField] private int mediumEnemyPoolCount;
    [SerializeField] private int mediumEnemiesSpawned;
    [SerializeField] private int hardEnemyPoolCount;
    [SerializeField] private int hardEnemiesSpawned;
    [SerializeField] private int bossEnemyPoolCount;
    [SerializeField] private int bossEnemiesSpawned;
    [SerializeField] private int amountCurrentlyInPool;
    [SerializeField] private int totalEnemiesSpawned;
    [SerializeField] private GameObject enemyBasePrefabToSpawn;
    [SerializeField] private GameObject poolParentToSpawn;
    [SerializeField] private GameObject poolParent;
    [SerializeField] private List<GameObject> enemyPool;
    public List<GameObject> allEnemies;

    private PlayerBase player;

    private int easyIndexNumb;
    private int mediumIndexNumb;
    private int hardIndexNumb;
    private int bossIndexNumb;

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

        if (poolParent != null)
        {
            for (int i = 0; i < easyEnemyPoolCount; i++)
            {
                allEnemies.Add(easyEnemiesToSpawn[Random.Range(easyEnemiesToSpawn.Count - 1, 0)]);

                GameObject newEnemy = Instantiate(easyEnemiesToSpawn[Random.Range(easyEnemiesToSpawn.Count - 1, 0)], transform.position, Quaternion.identity);
                enemyPool.Add(newEnemy);
                newEnemy.transform.SetParent(poolParent.transform);
                newEnemy.SetActive(false);
                amountCurrentlyInPool++;
            }
            for (int i = 0; i < mediumEnemyPoolCount; i++)
            {
                allEnemies.Add(mediumEnemiesToSpawn[Random.Range(mediumEnemiesToSpawn.Count - 1, 0)]);

                GameObject newEnemy = Instantiate(mediumEnemiesToSpawn[Random.Range(mediumEnemiesToSpawn.Count - 1, 0)], transform.position, Quaternion.identity);
                enemyPool.Add(newEnemy);
                newEnemy.transform.SetParent(poolParent.transform);
                newEnemy.SetActive(false);
                amountCurrentlyInPool++;
            }
            for (int i = 0; i < hardEnemyPoolCount; i++)
            {
                allEnemies.Add(hardEnemiesToSpawn[Random.Range(hardEnemiesToSpawn.Count - 1, 0)]);

                GameObject newEnemy = Instantiate(hardEnemiesToSpawn[Random.Range(hardEnemiesToSpawn.Count - 1, 0)], transform.position, Quaternion.identity);
                enemyPool.Add(newEnemy);
                newEnemy.transform.SetParent(poolParent.transform);
                newEnemy.SetActive(false);
                amountCurrentlyInPool++;
            }
            for (int i = 0; i < bossEnemyPoolCount; i++)
            {
                allEnemies.Add(bossEnemiesToSpawn[Random.Range(bossEnemiesToSpawn.Count - 1, 0)]);

                GameObject newEnemy = Instantiate(bossEnemiesToSpawn[Random.Range(bossEnemiesToSpawn.Count - 1, 0)], transform.position, Quaternion.identity);
                enemyPool.Add(newEnemy);
                newEnemy.transform.SetParent(poolParent.transform);
                newEnemy.SetActive(false);
                amountCurrentlyInPool++;
            }

            totalEnemyPoolCount = easyEnemyPoolCount + mediumEnemyPoolCount + hardEnemyPoolCount + bossEnemyPoolCount;
        }
    }

    [SerializeField] private float collectiveSpawnChance = 0f;

    private void SpawnChance()
    {
        // Chat GPT helped me with the spawn chance formula :)
        // Apply difficulty scaling (example: exponential or linear)
        // As difficulty increases, easier enemies lose weight and harder ones gain
        float easyScaled = easySpawnWeight / currentDifficulty;
        float mediumScaled = mediumSpawnWeight * Mathf.Lerp(0f, 2f, currentDifficulty / scalingSegments);
        float hardScaled = hardSpawnWeight * Mathf.Lerp(0f, 2f, currentDifficulty / scalingSegments);
        float bossScaled = bossSpawnWeight * Mathf.Lerp(0f, 2f, currentDifficulty / scalingSegments);

        // Normalize so total = 100%
        float total = easyScaled + mediumScaled + hardScaled + bossScaled;

        CurrentEasySpawnChance = (easyScaled / total) * 100f;
        CurrentMediumSpawnChance = (mediumScaled / total) * 100f;
        CurrentHardSpawnChance = (hardScaled / total) * 100f;
        CurrentBossSpawnChance = (bossScaled / total) * 100f;
    }

    private bool TrySetNewSpawnPosition(Transform enemyTransform)
    {
        int spawnAttempts = 10;

        for (int i = 0; i < spawnAttempts; i++)
        {
            Vector3 newSpawnPosition = new Vector3(Random.Range(spawnBoundOne.transform.position.x, spawnBoundTwo.transform.position.x), Random.Range(spawnBoundOne.transform.position.y, spawnBoundTwo.transform.position.y), 0f);
            float distance = Vector3.Distance(newSpawnPosition, player.transform.position);

            if (distance < spawnMaxDistance && distance > spawnMinDistance)
            {
                //Debug.Log($"Spawned successfully on attempt {i + 1}");
                enemyTransform.position = newSpawnPosition;
                return true; 
            }
        }

        //Debug.Log("Spawn failed all attempts");
        return false; 
    }

    private void SpawnNewEnemy()
    {
        if (player != null)
        {
            if (enemyPool != null && enemyPool.Count != 0)
            {
                if (amountCurrentlyInPool > 0)
                {
                    if (amountOfEnemiesToSpawn > totalEnemiesSpawned)
                    {
                        int randomNumb = Random.Range(1, 100);
                        if (currentHardSpawnChance > 0 && hardEnemiesSpawned < hardEnemyPoolCount)
                        {
                            if (randomNumb <= CurrentHardSpawnChance)
                            {
                                int enemyToSpawn = SearchForEnemyTypeInPool(EnemyBase.DifficultyType.Hard);

                                if (enemyPool[enemyToSpawn].GetComponent<EnemyBase>().currentDifficultyType == EnemyBase.DifficultyType.Hard)
                                {
                                    bool success = TrySetNewSpawnPosition(enemyPool[enemyToSpawn].transform);

                                    if (success)
                                    {
                                        RemoveFromPool(enemyPool[enemyToSpawn].gameObject);
                                        hardEnemiesSpawned++;
                                    }
                                    else
                                    {
                                        SpawnFailAddBackToPool(enemyPool[enemyToSpawn].gameObject);
                                    }
                                }
                            }
                        }
                        if (currentBossSpawnChance > 0 && bossEnemiesSpawned < bossEnemyPoolCount)
                        {
                            if (randomNumb <= CurrentBossSpawnChance)
                            {
                                int enemyToSpawn = SearchForEnemyTypeInPool(EnemyBase.DifficultyType.Boss);

                                if (enemyPool[enemyToSpawn].GetComponent<EnemyBase>().currentDifficultyType == EnemyBase.DifficultyType.Boss)
                                {
                                    bool success = TrySetNewSpawnPosition(enemyPool[enemyToSpawn].transform);

                                    if (success)
                                    {
                                        RemoveFromPool(enemyPool[enemyToSpawn].gameObject);
                                        bossEnemiesSpawned++;
                                    }
                                    else
                                    {
                                        SpawnFailAddBackToPool(enemyPool[enemyToSpawn].gameObject);
                                    }
                                }
                            }
                        }
                        if (currentMediumSpawnChance > 0 && mediumEnemiesSpawned < mediumEnemyPoolCount)
                        {
                            if (randomNumb <= CurrentMediumSpawnChance)
                            {
                                int enemyToSpawn = SearchForEnemyTypeInPool(EnemyBase.DifficultyType.Medium);

                                if (enemyPool[enemyToSpawn].GetComponent<EnemyBase>().currentDifficultyType == EnemyBase.DifficultyType.Medium)
                                {
                                    bool success = TrySetNewSpawnPosition(enemyPool[enemyToSpawn].transform);

                                    if (success)
                                    {
                                        RemoveFromPool(enemyPool[enemyToSpawn].gameObject);
                                        mediumEnemiesSpawned++;
                                    }
                                    else
                                    {
                                        SpawnFailAddBackToPool(enemyPool[enemyToSpawn].gameObject);
                                    }
                                }
                            }
                        }
                        if (currentEasySpawnChance > 0 && easyEnemiesSpawned < easyEnemyPoolCount)
                        {
                            if (randomNumb <= CurrentEasySpawnChance)
                            {
                                int enemyToSpawn = SearchForEnemyTypeInPool(EnemyBase.DifficultyType.Easy);

                                if (enemyPool[enemyToSpawn].GetComponent<EnemyBase>().currentDifficultyType == EnemyBase.DifficultyType.Easy)
                                {
                                    bool success = TrySetNewSpawnPosition(enemyPool[enemyToSpawn].transform);

                                    if (success)
                                    {
                                        RemoveFromPool(enemyPool[enemyToSpawn].gameObject);
                                        easyEnemiesSpawned++;
                                    }
                                    else
                                    {
                                        SpawnFailAddBackToPool(enemyPool[enemyToSpawn].gameObject);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void SpawnFailAddBackToPool(GameObject go)
    {
        enemyPool.Add(go);
        go.transform.position = poolParent.transform.position;
        go.SetActive(false);
        amountCurrentlyInPool++;
    }

    public void AddToPool(GameObject go)
    {
        // used when the enemy dies and needs to be reset
        enemyPool.Add(go);
        go.transform.position = poolParent.transform.position;
        go.SetActive(false);
        amountCurrentlyInPool++;
        totalEnemiesSpawned--;
    }

    public void RemoveFromPool(GameObject go)
    {
        enemyPool.Remove(go);
        go.SetActive(true);
        amountCurrentlyInPool--;
        totalEnemiesSpawned++;
    }

    private int SearchForEnemyTypeInPool(EnemyBase.DifficultyType type)
    {
        for (int i = 0; i < enemyPool.Count; i++)
        {
            if (enemyPool[i].GetComponent<EnemyBase>().currentDifficultyType != type)
            {
                continue;
            }
            else
            {
                type = enemyPool[i].GetComponent<EnemyBase>().currentDifficultyType;
                return i;
            }
        }

        return 0;
    }
}
