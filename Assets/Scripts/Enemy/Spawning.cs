using JetBrains.Annotations;
using System;
using System.Collections.Generic;
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
    //[SerializeField] private List<GameObject> easyEnemiesToSpawn;
    [SerializeField] private List<GameObject> easyEnemiesToSpawn;
    [SerializeField] private List<GameObject> mediumEnemiesToSpawn;
    [SerializeField] private List<GameObject> hardEnemiesToSpawn;
    [SerializeField] private List<GameObject> bossEnemiesToSpawn;
    [Space]
    [Header("Positions")]
    [SerializeField] private float spawnMinDistance;
    [SerializeField] private float spawnMaxDistance;
    [Header("Pool")]
    [SerializeField] private int totalEnemyPoolCount;
    [SerializeField] private int easyEnemyPoolCount;
    [SerializeField] private int mediumEnemyPoolCount;
    [SerializeField] private int hardEnemyPoolCount;
    [SerializeField] private int bossEnemyPoolCount;
    [SerializeField] private int amountCurrentlyInPool;
    [SerializeField] private int activeInPool;
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

    private void SpawnChance()
    {
        currentEasySpawnChance = easySpawnChance - amountOfEnemiesToSpawn;
        currentMediumSpawnChance = mediumSpawnChance + amountOfEnemiesToSpawn;
        currentHardSpawnChance = hardSpawnChance + amountOfEnemiesToSpawn;
        currentBossSpawnChance = bossSpawnChance + amountOfEnemiesToSpawn;
    }

    private void SpawnNewEnemy()
    {
        if (player != null)
        {
            if (enemyPool != null && enemyPool.Count != 0)
            {
                if (amountCurrentlyInPool > 0)
                {
                    if (amountOfEnemiesToSpawn > activeInPool)
                    {
                        if (currentHardSpawnChance > 0)
                        {
                            int randomNumb = Random.Range(1, 100);
                            if (randomNumb <= currentHardSpawnChance)
                            {
                                if (enemyPool[hardIndexNumb].GetComponent<EnemyBase>().currentDifficultyType == EnemyBase.DifficultyType.Hard)
                                {
                                    enemyPool[hardIndexNumb].gameObject.SetActive(true);
                                    Debug.Log("Spawned EASY Enemy");
                                    amountCurrentlyInPool--;
                                    activeInPool++;
                                    enemyPool.RemoveAt(0);
                                    hardIndexNumb = 0;
                                }
                                else
                                {
                                    hardIndexNumb++;
                                }
                            }
                        }
                        if (currentBossSpawnChance > 0)
                        {
                            int randomNumb = Random.Range(1, 100);
                            if (randomNumb <= currentBossSpawnChance)
                            {
                                if (enemyPool[bossIndexNumb].GetComponent<EnemyBase>().currentDifficultyType == EnemyBase.DifficultyType.Boss)
                                {
                                    enemyPool[bossIndexNumb].gameObject.SetActive(true);
                                    Debug.Log("Spawned EASY Enemy");
                                    amountCurrentlyInPool--;
                                    activeInPool++;
                                    enemyPool.RemoveAt(0);
                                    bossIndexNumb = 0;
                                }
                                else
                                {
                                    bossIndexNumb++;
                                }
                            }
                        }
                        if (currentMediumSpawnChance > 0)
                        {
                            int randomNumb = Random.Range(1, 100);
                            if (randomNumb <= currentMediumSpawnChance)
                            {
                                if (enemyPool[mediumIndexNumb].GetComponent<EnemyBase>().currentDifficultyType == EnemyBase.DifficultyType.Medium)
                                {
                                    enemyPool[mediumIndexNumb].gameObject.SetActive(true);
                                    Debug.Log("Spawned EASY Enemy");
                                    amountCurrentlyInPool--;
                                    activeInPool++;
                                    enemyPool.RemoveAt(0);
                                    mediumIndexNumb = 0;
                                }
                                else
                                {
                                    mediumIndexNumb++;
                                }
                            }
                        }
                        if (currentEasySpawnChance > 0)
                        {
                            int randomNumb = Random.Range(1, 100);
                            if (randomNumb <= currentEasySpawnChance)
                            {
                                if (enemyPool[easyIndexNumb].GetComponent<EnemyBase>().currentDifficultyType == EnemyBase.DifficultyType.Easy)
                                {
                                    enemyPool[easyIndexNumb].gameObject.SetActive(true);
                                    Debug.Log("Spawned EASY Enemy");
                                    amountCurrentlyInPool--;
                                    activeInPool++;
                                    enemyPool.RemoveAt(0);
                                    easyIndexNumb = 0;
                                }
                                else
                                {
                                    easyIndexNumb++;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
