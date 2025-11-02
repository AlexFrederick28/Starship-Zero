using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Experience : MonoBehaviour
{
    [SerializeField] private List<ExperiencePoint> experiencePool;
    [SerializeField] private GameObject poolParent;
    [SerializeField] private int maxExperiencePoolCount;
    [SerializeField] private int totalExperienceInPool;

    private void SpawnPool()
    {

    }

    public void AddToPool(ExperiencePoint exp)
    {
        // used when the exp is gathered and needs to be reset
        experiencePool.Add(exp);
        exp.transform.position = poolParent.transform.position;
        exp.gameObject.SetActive(false);
        //amountCurrentlyInPool++;
        totalExperienceInPool--;
    }

    public void RemoveFromPool(ExperiencePoint exp)
    {
        experiencePool.Remove(exp);
        exp.gameObject.SetActive(true);
        //amountCurrentlyInPool--;
        totalExperienceInPool++;
    }
}
