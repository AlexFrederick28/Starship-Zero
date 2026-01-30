using System;
using System.Collections.Generic;
using UnityEngine;

public class SpecimenPool : MonoBehaviour
{
    [SerializeField] private int maxSpecimenPoolCount;
    [SerializeField] private int totalSpecimensInPool;
    private bool spawnedPool = false;

    [SerializeField] private GameObject poolParent;
    [SerializeField] private GameObject poolParentToSpawn;
    [SerializeField] private GameObject specimenPrefab;
    public List<SpecimenObject> specimenPool;
    public SpecimenObject selectedSpecimen;

    private void Update()
    {
        if (selectedSpecimen == null && specimenPool != null)
        {
            selectedSpecimen = specimenPool[0];
        }
    }

    public void SpawnPool()
    {
        if (poolParent == null)
        {
            poolParent = Instantiate(poolParentToSpawn, transform.position, Quaternion.identity);
        }

        if (specimenPool.Count < maxSpecimenPoolCount && spawnedPool == false)
        {
            for (int i = specimenPool.Count; i < maxSpecimenPoolCount; i++)
            {
                GameObject newSpecimen = Instantiate(specimenPrefab, poolParent.transform.position, Quaternion.identity);
                newSpecimen.transform.SetParent(poolParent.transform);
                AddToPool(newSpecimen.GetComponent<SpecimenObject>());
            }

            Debug.Log("Spawned specimen pool");
            spawnedPool = true;
        }
    }

    public void AddToPool(SpecimenObject obj)
    {
        specimenPool.Add(obj);
        obj.transform.position = poolParent.transform.position;
        obj.gameObject.SetActive(false);
        totalSpecimensInPool++;
    }

    public void RemoveFromPool(SpecimenObject obj, SpecimenType type, Transform spawnArea)
    {
        if (obj != null)
        {
            specimenPool.Remove(obj);
            obj.gameObject.SetActive(true);
            totalSpecimensInPool--;
            obj.specimenType = type;
            obj.transform.position = spawnArea.position;
            obj.spriteRenderer.sprite = type.sprite;
        }

        selectedSpecimen = null;
    }
}
