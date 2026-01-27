using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;

public class Spawning : Difficulty
{
    [Space]
    [Header("Quest Level")]
    [Tooltip("If this room is attached to a quest, enter its ID here to obtain its level and dynamically changing the scaling of enemies")]
    public int questID;
    [Tooltip("Objects that will activate once the player has cleared the room")]
    public GameObject[] questObjects;

    [Space]
    [Header("Checkpoint")]
    public RespawnCheckpoint checkpoint;

    [Space]
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
    [SerializeField, ReadOnly] private float currentMediumSpawnChance;
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
    private float collectiveSpawnChance = 0f;
    [Header("Chance Multiplier")]
    [SerializeField] private float easySpawnWeight;
    [SerializeField] private float mediumSpawnWeight;
    [SerializeField] private float hardSpawnWeight;
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
    [Tooltip("How far the player has to be between two 'closest' rooms for enemies to spawn from both")]
    [SerializeField] private float splitSpawnDistance;
    [SerializeField] private Collider2D[] roomSpawnBounds;
    private Collider2D closestSpawn;
    private Collider2D secondClosestSpawn;
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
    [SerializeField] private int totalEnemiesActive;
    [SerializeField] private int totalEnemiesInactive;
    [SerializeField] private GameObject enemyBasePrefabToSpawn;
    [SerializeField] private GameObject poolParentToSpawn;
    [SerializeField] private GameObject poolParent;
    [Tooltip("")]
    [SerializeField] private List<GameObject> enemiesInactiveInPool;
    public List<GameObject> allEnemies;

    [Header("Experience")]
    [SerializeField] private Experience experience;

    public PlayerBase player { get; private set; }

    private int easyIndexNumb;
    private int mediumIndexNumb;
    private int hardIndexNumb;
    private int bossIndexNumb;

    public bool playerClearedRoom = false;
    private bool PlayerWinCondition()
    {
        if (timerReachedMaxLength == true && playerClearedRoom == false)
        {
            ResetInfestedRoom();
            playerClearedRoom = true;
            GameState.instance.ChangeStateToMain();
            foreach (GameObject go in questObjects)
            {
                if (go.GetComponent<QuestTaskBase>())
                {
                    go.GetComponent<QuestTaskBase>().enabled = true;
                    continue;
                }
            }
            GetComponentInParent<Room>().currentState = GetComponentInParent<Room>().clearedState;
            return true;
        }
        else
        {
            return false;
        }
    }

    // for the instance of a spawner to work for each room, the gameobject must only be active when the room is entered as to not have multiple instances destroyed when they're needed later
    public static Spawning instance;

    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // turns off duplicate instances if there are more than one enabled
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Start()
    {
        // pools get spawned when this script is turned on, by default it is off until the player interacts with the infested door
        SpawnPools();
        DisabledQuestObjects();
    }

    override public void Update()
    {
        if (GameState.instance.currentState == GameState.States.RoomClear && playerClearedRoom == false)
        {
            base.Update();
            SpawnChance();

            if (player == null)
            {
                player = GameState.instance.player;
            }

            amountOfEnemiesToSpawn = (currentDifficulty / scalingSegments) * spawnAmountMulitplier;

            SpawnNewEnemy();
            PlayerWinCondition();
        }
    }

    public void SpawnPools()
    {
        SpawnPool(); // enemy pool
        experience = GetComponent<Experience>();
        experience.enabled = true;
        experience.SpawnPool(); // experience pool
        experience.ScaleEntireExperiencePool(); // scale experience with quest level
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
                GameObject newEnemy = Instantiate(easyEnemiesToSpawn[Random.Range(easyEnemiesToSpawn.Count - 1, 0)], transform.position, Quaternion.identity);
                AddNewEnemyToPool(newEnemy);
            }
            for (int i = 0; i < mediumEnemyPoolCount; i++)
            {
                GameObject newEnemy = Instantiate(mediumEnemiesToSpawn[Random.Range(mediumEnemiesToSpawn.Count - 1, 0)], transform.position, Quaternion.identity);
                AddNewEnemyToPool(newEnemy);
            }
            for (int i = 0; i < hardEnemyPoolCount; i++)
            {
                GameObject newEnemy = Instantiate(hardEnemiesToSpawn[Random.Range(hardEnemiesToSpawn.Count - 1, 0)], transform.position, Quaternion.identity);
                AddNewEnemyToPool(newEnemy);
            }
            for (int i = 0; i < bossEnemyPoolCount; i++)
            {
                GameObject newEnemy = Instantiate(bossEnemiesToSpawn[Random.Range(bossEnemiesToSpawn.Count - 1, 0)], transform.position, Quaternion.identity);
                AddNewEnemyToPool(newEnemy);
            }

            totalEnemyPoolCount = easyEnemyPoolCount + mediumEnemyPoolCount + hardEnemyPoolCount + bossEnemyPoolCount;
        }
    }

    private void SpawnChance()
    {
        // As difficulty increases, easier enemies lose weight and harder ones gain
        float easyScaled = easySpawnWeight / currentDifficulty;
        float mediumScaled = mediumSpawnWeight * Mathf.Lerp(0f, 1f, currentDifficulty / scalingSegments);
        float hardScaled = hardSpawnWeight * Mathf.Lerp(0f, 1f, currentDifficulty / scalingSegments);
        float bossScaled = bossSpawnWeight * Mathf.Lerp(0f, 1f, currentDifficulty / scalingSegments);

        // Normalize so total = 100%
        collectiveSpawnChance = easyScaled + mediumScaled + hardScaled + bossScaled;

        CurrentEasySpawnChance = (easyScaled / collectiveSpawnChance) * 100f;
        CurrentMediumSpawnChance = (mediumScaled / collectiveSpawnChance) * 100f;
        CurrentHardSpawnChance = (hardScaled / collectiveSpawnChance) * 100f;
        CurrentBossSpawnChance = (bossScaled / collectiveSpawnChance) * 100f;
    }

    private bool TrySetNewSpawnPosition(Transform enemyTransform)
    {
        int spawnAttempts = 10;

        for (int i = 0; i < spawnAttempts; i++)
        {
            for (int x = 0; x < roomSpawnBounds.Length; x++)
            {
                // find the closest room to spawn in
                if (closestSpawn == null)
                {
                    // initial spawn room
                    closestSpawn = roomSpawnBounds[x];
                }
                else if (Vector3.Distance(roomSpawnBounds[x].transform.position, player.transform.position) < Vector3.Distance(closestSpawn.transform.position, player.transform.position))
                {
                    secondClosestSpawn = closestSpawn;
                    if (Vector3.Distance(secondClosestSpawn.transform.position, player.transform.position) < splitSpawnDistance && Vector3.Distance(closestSpawn.transform.position, player.transform.position) < splitSpawnDistance)
                    {
                        // if the distance between two potential spawn rooms are close in proximity then use both at random
                        int randNumb = Random.Range(0, 100);
                        if (randNumb > 50)
                        {
                            closestSpawn = roomSpawnBounds[x];
                        }
                        else
                        {
                            closestSpawn = secondClosestSpawn;
                        }
                    }
                    else
                    {
                        // by default the closer spawn room overrides the previous
                        closestSpawn = roomSpawnBounds[x];
                    }
                }
            }
            Vector3 nspawnPosition = new Vector3(Random.Range(closestSpawn.bounds.min.x, closestSpawn.bounds.max.x), Random.Range(closestSpawn.bounds.min.y, closestSpawn.bounds.max.y), 0f);
            float ndistance = Vector3.Distance(nspawnPosition, player.transform.position);

            if (ndistance < spawnMaxDistance && ndistance > spawnMinDistance)
            {
                // if the new spawn location distance is correct, spawn the enemy, else retry
                enemyTransform.position = nspawnPosition;
                closestSpawn = null;
                return true; 
            }
        }

        return false; 
    }

    private void SpawnNewEnemy()
    {
        if (player != null)
        {
            if (enemiesInactiveInPool != null && enemiesInactiveInPool.Count != 0)
            {
                if (amountOfEnemiesToSpawn > totalEnemiesActive)
                {
                    int randomNumb = Random.Range(1, 100);
                    if (currentHardSpawnChance > 0 && hardEnemiesSpawned < hardEnemyPoolCount)
                    {
                        if (randomNumb <= CurrentHardSpawnChance)
                        {
                            int enemyToSpawn = SearchForEnemyTypeInPool(EnemyBase.DifficultyType.Hard);

                            if (enemiesInactiveInPool[enemyToSpawn].GetComponent<EnemyBase>().currentDifficultyType == EnemyBase.DifficultyType.Hard)
                            {
                                bool success = TrySetNewSpawnPosition(enemiesInactiveInPool[enemyToSpawn].transform);

                                if (success)
                                {
                                    RemoveFromPool(enemiesInactiveInPool[enemyToSpawn].gameObject);
                                    hardEnemiesSpawned++;
                                }
                            }
                        }
                    }
                    if (currentBossSpawnChance > 0 && bossEnemiesSpawned < bossEnemyPoolCount)
                    {
                        if (randomNumb <= CurrentBossSpawnChance)
                        {
                            int enemyToSpawn = SearchForEnemyTypeInPool(EnemyBase.DifficultyType.Boss);

                            if (enemiesInactiveInPool[enemyToSpawn].GetComponent<EnemyBase>().currentDifficultyType == EnemyBase.DifficultyType.Boss)
                            {
                                bool success = TrySetNewSpawnPosition(enemiesInactiveInPool[enemyToSpawn].transform);

                                if (success)
                                {
                                    RemoveFromPool(enemiesInactiveInPool[enemyToSpawn].gameObject);
                                    bossEnemiesSpawned++;
                                }
                            }
                        }
                    }
                    if (currentMediumSpawnChance > 0 && mediumEnemiesSpawned < mediumEnemyPoolCount)
                    {
                        if (randomNumb <= CurrentMediumSpawnChance)
                        {
                            int enemyToSpawn = SearchForEnemyTypeInPool(EnemyBase.DifficultyType.Medium);

                            if (enemiesInactiveInPool[enemyToSpawn].GetComponent<EnemyBase>().currentDifficultyType == EnemyBase.DifficultyType.Medium)
                            {
                                bool success = TrySetNewSpawnPosition(enemiesInactiveInPool[enemyToSpawn].transform);

                                if (success)
                                {
                                    RemoveFromPool(enemiesInactiveInPool[enemyToSpawn].gameObject);
                                    mediumEnemiesSpawned++;
                                }
                            }
                        }
                    }
                    if (currentEasySpawnChance > 0 && easyEnemiesSpawned < easyEnemyPoolCount)
                    {
                        if (randomNumb <= CurrentEasySpawnChance)
                        {
                            int enemyToSpawn = SearchForEnemyTypeInPool(EnemyBase.DifficultyType.Easy);

                            if (enemiesInactiveInPool[enemyToSpawn].GetComponent<EnemyBase>().currentDifficultyType == EnemyBase.DifficultyType.Easy)
                            {
                                bool success = TrySetNewSpawnPosition(enemiesInactiveInPool[enemyToSpawn].transform);

                                if (success)
                                {
                                    RemoveFromPool(enemiesInactiveInPool[enemyToSpawn].gameObject);
                                    easyEnemiesSpawned++;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void AddNewEnemyToPool(GameObject go)
    {
        // used when spawning an entirly new pool
        go.transform.SetParent(poolParent.transform);
        enemiesInactiveInPool.Add(go);
        go.transform.position = poolParent.transform.position;
        go.SetActive(false);
        allEnemies.Add(go);
        totalEnemiesInactive++;
    }

    public void AddToPool(GameObject go)
    {
        // used when the enemy dies and needs to be reset
        enemiesInactiveInPool.Add(go);
        go.transform.position = poolParent.transform.position;
        go.SetActive(false);
        totalEnemiesActive--;
        totalEnemiesInactive++;
    }

    public void RemoveFromPool(GameObject go)
    {
        // used when spawning a new enemy
        enemiesInactiveInPool.Remove(go);
        go.SetActive(true);
        totalEnemiesActive++;
        totalEnemiesInactive--;
    }

    private int SearchForEnemyTypeInPool(EnemyBase.DifficultyType type)
    {
        for (int i = 0; i < enemiesInactiveInPool.Count; i++)
        {
            if (enemiesInactiveInPool[i].GetComponent<EnemyBase>().currentDifficultyType != type)
            {
                continue;
            }
            else
            {
                type = enemiesInactiveInPool[i].GetComponent<EnemyBase>().currentDifficultyType;
                return i;
            }
        }

        return 0;
    }

    public void EnemyDeath(EnemyBase.DifficultyType type)
    {
        if (type == EnemyBase.DifficultyType.Easy)
        {
            easyEnemiesSpawned--;
        }
        if (type == EnemyBase.DifficultyType.Medium)
        {
            mediumEnemiesSpawned--;
        }
        if (type == EnemyBase.DifficultyType.Hard)
        {
            hardEnemiesSpawned--;
        }
        if (type == EnemyBase.DifficultyType.Boss)
        {
            bossEnemiesSpawned--;
        }
    }

    public void ResetInfestedRoom()
    {
        Debug.Log("Reset room");
        if (timerReachedMaxLength == false)
        {
            currentTime = 0;
        }
        easyEnemiesSpawned = 0;
        mediumEnemiesSpawned = 0;
        hardEnemiesSpawned = 0;
        bossEnemiesSpawned = 0;

        for (int i = 0; i < allEnemies.Count; i++)
        {
            if (allEnemies[i].activeInHierarchy == true)
            {
                Debug.Log("Reset " + allEnemies[i].name);
                AddToPool(allEnemies[i]);
            }
        }
    }

    public void DisabledQuestObjects()
    {
        foreach (GameObject go in questObjects)
        {
            if (go.GetComponent<QuestTaskBase>())
            {
                go.GetComponent<QuestTaskBase>().enabled = false;
                continue;
            }
        }
    }
}
