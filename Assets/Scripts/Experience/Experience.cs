using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Experience : MonoBehaviour
{
    [SerializeField] private int maxExperiencePoolCount;
    [SerializeField] private int totalExperienceInPool;
    private bool spawnedPool = false;

    [SerializeField] private GameObject poolParent;
    [SerializeField] private GameObject poolParentToSpawn;
    [SerializeField] private GameObject experiencePointPrefab;
    [SerializeField] private List<ExperiencePoint> experiencePool;
    public ExperiencePoint selectedExperiencePoint;

    private void Start()
    {
        // this shouldonly be spawned when entering a room, just like the enemies
        SpawnPool();
    }

    private void Update()
    {
        if (selectedExperiencePoint == null && experiencePool != null)
        {
            selectedExperiencePoint = experiencePool[0];
        }
    }

    private void SpawnPool()
    {
        if (poolParent == null)
        {
            poolParent = Instantiate(poolParentToSpawn, transform.position, Quaternion.identity);
        }

        if (experiencePool.Count < maxExperiencePoolCount && spawnedPool == false)
        {
            for (int i = experiencePool.Count; i < maxExperiencePoolCount; i++)
            {
                GameObject newExperiencePoint = Instantiate(experiencePointPrefab, poolParent.transform.position, Quaternion.identity);
                newExperiencePoint.transform.SetParent(poolParent.transform);
                AddToPool(newExperiencePoint.GetComponent<ExperiencePoint>());
            }

            Debug.Log("Spawned experience pool");
            spawnedPool = true;
        }
    }

    public void AddToPool(ExperiencePoint exp)
    {
        experiencePool.Add(exp);
        exp.transform.position = poolParent.transform.position;
        exp.gameObject.SetActive(false);
        totalExperienceInPool++;
    }

    public void RemoveFromPool(ExperiencePoint exp, EnemyBase.DifficultyType type, Transform spawnArea)
    {
        if (exp != null)
        {
            experiencePool.Remove(exp);
            exp.gameObject.SetActive(true);
            totalExperienceInPool--;
            exp.currentExperienceType = type;
            exp.transform.position = spawnArea.position;
            exp.SetExperience(type);

            selectedExperiencePoint = null;
        }
    }
}
