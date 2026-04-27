using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;

public class Spawning : Difficulty
{
    [Space]
    [Header("Parent Room")]
    public Room parentRoom;

    [Space]
    [Header("Quest Info")]
    [Tooltip("If this room is attached to a quest, enter its ID here to obtain its level and dynamically changing the scaling of enemies")]
    public Vector2 questLevel;
    public bool entryQuestActivated = false;
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
    public Experience experience;

    [Space]
    [Header("Specimens")]
    public SpecimenPool specimenPool;

    public PlayerBase player { get; private set; }

    private int easyIndexNumb;
    private int mediumIndexNumb;
    private int hardIndexNumb;
    private int bossIndexNumb;

    public Action OnInfestedRoomReset;

    public bool playerClearedRoom = false;
    private bool PlayerWinCondition()
    {
        if (timerReachedMaxLength == true && totalEnemiesActive < 1)
        {
            // if there are no enemies active and the timer reached the limit
            // enemies no longer spawn after the timer is over, however the player has to clear all of them to succeed/proceed.

            //StartCoroutine(UIManager.instance.NewLargeNotification("Room Cleared", screenTime, textSizeIncrease));
            UIManager.instance.SpawnLargeNotification("Room Cleared", screenTime, textSizeIncrease);

            //ResetInfestedRoom();
            OnInfestedRoomReset?.Invoke();
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
            GameState.instance.OnCompletedInfestedClear?.Invoke(parentRoom);
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
        if (GameState.instance != null)
        {
            //Debug.Log(this.name + "Subscribed");
            GameState.instance.OnPlayerRespawn += ResetInfestedRoom;
            GameState.instance.OnPlayerRespawn += PauseAndUnpauseSpawning;
            GameState.instance.OnPlayerRespawn += DisableInstanceOnPlayerRespawn;
            GameState.instance.OnPlayerRetry += ResetInfestedRoom;
            GameState.instance.OnPlayerRetry += PauseAndUnpauseSpawning;
            GameState.instance.OnPlayerLevelUp += PauseAndUnpauseSpawning;
            LevelUpManager.OnCardChosen += PauseAndUnpauseSpawning;

            GameState.instance.OnCompletedInfestedClear += GameState.instance.playerInventory.DestroyLoadout;
            GameState.instance.OnCompletedInfestedClear += GameState.instance.playerInventory.RemoveGapsFromInventoryOnInfestedRoomCompletion;

            GameState.instance.OnEnteringInfestedRoom?.Invoke();

            PlayerBase.OnPlayerDeath += PauseAndUnpauseSpawning;

            GameState.instance.OnGamePause += PauseSpawning;
            GameState.instance.OnGameUnPause += UnPauseSpawning;

        }

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // turns off duplicate instances if there are more than one enabled
            gameObject.SetActive(false);
        }

        if (entryQuestActivated == false)
        {
            questLevel.x = GameState.instance.player.Level;
            questLevel.y = GameState.instance.player.CurrentExperience;
        }
    }

    private void OnDisable()
    {
        GameState.instance.OnPlayerRespawn -= ResetInfestedRoom;
        GameState.instance.OnPlayerRespawn -= PauseAndUnpauseSpawning;
        GameState.instance.OnPlayerRespawn -= DisableInstanceOnPlayerRespawn;
        GameState.instance.OnPlayerRetry -= ResetInfestedRoom;
        GameState.instance.OnPlayerRetry -= PauseAndUnpauseSpawning;
        GameState.instance.OnPlayerLevelUp -= PauseAndUnpauseSpawning;
        LevelUpManager.OnCardChosen -= PauseAndUnpauseSpawning;

        GameState.instance.OnCompletedInfestedClear -= GameState.instance.playerInventory.DestroyLoadout;
        GameState.instance.OnCompletedInfestedClear -= GameState.instance.playerInventory.RemoveGapsFromInventoryOnInfestedRoomCompletion;

        PlayerBase.OnPlayerDeath -= PauseAndUnpauseSpawning;

        GameState.instance.OnGamePause -= PauseSpawning;
        GameState.instance.OnGameUnPause -= UnPauseSpawning;

        if (instance == this)
        {
            instance = null;
        }
        if (entryQuestActivated == false)
        {
            questLevel.x = 0;
            questLevel.y = 0;
        }
    }

    private void Start()
    {
        // pools get spawned when this script is turned on, by default it is off until the player interacts with the infested door
        SpawnPools();
        DisableQuestObjects();
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

            PlayerWinCondition();

            if (timerReachedMaxLength == true) { return; }
            SpawnNewEnemy();
        }
    }

    public void SpawnPools()
    {
        SpawnPool(); // enemy pool
        experience = GetComponent<Experience>();
        experience.enabled = true;
        experience.SpawnPool(); // experience pool
        experience.ScaleEntireExperiencePool(); // scale experience with quest level
        specimenPool = GetComponent<SpecimenPool>();
        specimenPool.enabled = true;
        specimenPool.SpawnPool();
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
                GameObject newEnemy = Instantiate(easyEnemiesToSpawn[Random.Range(0, easyEnemiesToSpawn.Count)], transform.position, Quaternion.identity);
                AddNewEnemyToPool(newEnemy);
            }
            for (int i = 0; i < mediumEnemyPoolCount; i++)
            {
                GameObject newEnemy = Instantiate(mediumEnemiesToSpawn[Random.Range(0, mediumEnemiesToSpawn.Count)], transform.position, Quaternion.identity);
                AddNewEnemyToPool(newEnemy);
            }
            for (int i = 0; i < hardEnemyPoolCount; i++)
            {
                GameObject newEnemy = Instantiate(hardEnemiesToSpawn[Random.Range(0, hardEnemiesToSpawn.Count)], transform.position, Quaternion.identity);
                AddNewEnemyToPool(newEnemy);
            }
            for (int i = 0; i < bossEnemyPoolCount; i++)
            {
                GameObject newEnemy = Instantiate(bossEnemiesToSpawn[Random.Range(0, bossEnemiesToSpawn.Count)], transform.position, Quaternion.identity);
                AddNewEnemyToPool(newEnemy);
            }

            totalEnemyPoolCount = easyEnemyPoolCount + mediumEnemyPoolCount + hardEnemyPoolCount + bossEnemyPoolCount;
        }
    }

    private void SpawnChance()
    {
        // As difficulty increases, easier enemies lose weight and harder ones gain
        float easyScaled = easySpawnWeight / CurrentDifficulty;
        float mediumScaled = mediumSpawnWeight * Mathf.Lerp(0f, 1f, CurrentDifficulty / scalingSegments);
        float hardScaled = hardSpawnWeight * Mathf.Lerp(0f, 1f, CurrentDifficulty / scalingSegments);
        float bossScaled = bossSpawnWeight * Mathf.Lerp(0f, 1f, CurrentDifficulty / scalingSegments);

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
        if (timerPaused == true) { return; }
        if (GameState.instance.player.playerDead == true) { return; }
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
                                    return;
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
                                    return;
                                }
                            }
                        }
                    }
                    if (currentMediumSpawnChance > 0 && mediumEnemiesSpawned < mediumEnemyPoolCount)
                    {
                        if (randomNumb <= CurrentMediumSpawnChance)
                        {
                            //Debug.Log("Spawned medium enemy");
                            int enemyToSpawn = SearchForEnemyTypeInPool(EnemyBase.DifficultyType.Medium);

                            if (enemiesInactiveInPool[enemyToSpawn].GetComponent<EnemyBase>().currentDifficultyType == EnemyBase.DifficultyType.Medium)
                            {
                                bool success = TrySetNewSpawnPosition(enemiesInactiveInPool[enemyToSpawn].transform);

                                if (success)
                                {
                                    RemoveFromPool(enemiesInactiveInPool[enemyToSpawn].gameObject);
                                    mediumEnemiesSpawned++;
                                    return;
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
                                    return;
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
        //Debug.Log("Reset room");
        if (timerReachedMaxLength == false)
        {
            // only reset timer if the player has been respawned or retried the room
            currentTime = 0;
        }
        else
        {
            currentTime = 0;
            timerReachedMaxLength = false;
        }

        easyEnemiesSpawned = 0;
        mediumEnemiesSpawned = 0;
        hardEnemiesSpawned = 0;
        bossEnemiesSpawned = 0;

        // enemies reset themselves back into the pool
        OnInfestedRoomReset?.Invoke();
    }

    public void DisableQuestObjects()
    {
        foreach (GameObject go in questObjects)
        {
            if (go.GetComponent<QuestTaskBase>())
            {
                go.GetComponent<QuestTaskBase>().enabled = false;
                continue;
            }
            else if (go.GetComponent<IInteractable>() != null)
            {
                go.GetComponent<IInteractable>().DisableInteractionComponent();
            }
        }
    }

    public void DisableInstanceOnPlayerRespawn()
    {
        instance.enabled = false;
    }
}
