using System.Collections;
using UnityEngine;

public class ExperiencePoint : MonoBehaviour, ICollectable
{
    public EnemyBase.DifficultyType currentExperienceType;

    public float currentExperienceAmount;
    [SerializeField] private float easyDefaultExperience;
    [SerializeField] private float mediumDefaultExperience;
    [SerializeField] private float hardDefaultExperience;
    [SerializeField] private float bossDefaultExperience;
    [SerializeField] private float easyScaleExperience;
    [SerializeField] private float mediumScaleExperience;
    [SerializeField] private float hardScaleExperience;
    [SerializeField] private float bossScaleExperience;

    private void OnEnable()
    {
        Spawning.instance.OnInfestedRoomReset += AddExperiencePointBackToSpawnPool;

        // setting the experience upon enabing the game object as to allow for the correct type of experience to spawn when a certain type of enemy dies
        SetExperience(currentExperienceType);
    }

    private void OnDisable()
    {
        Spawning.instance.OnInfestedRoomReset -= AddExperiencePointBackToSpawnPool;
    }

    public void SetExperience(EnemyBase.DifficultyType currentType)
    {
        if (currentExperienceType == EnemyBase.DifficultyType.Easy)
        {
            currentExperienceAmount = easyDefaultExperience;
        }
        if (currentExperienceType == EnemyBase.DifficultyType.Medium)
        {
            currentExperienceAmount = mediumDefaultExperience;
        }
        if (currentExperienceType == EnemyBase.DifficultyType.Hard)
        {
            currentExperienceAmount = hardDefaultExperience;
        }
        if (currentExperienceType == EnemyBase.DifficultyType.Boss)
        {
            currentExperienceAmount = bossDefaultExperience;
        }
    }

    public void AddExperienceAdditive(int additive)
    {
        if (additive == 0) { return; }
        currentExperienceAmount += additive;
    }

    public void ScaleExperience(float currentExperience)
    {
        // setting the initial default experience
        SetExperience(currentExperienceType);
        // scale the current experience to whatever it should be based on quest level
        if (Spawning.instance != null)
        {
            float previousExperience = currentExperience;
            easyDefaultExperience += easyScaleExperience * GameState.instance.player.ExperienceNeeded;
            mediumDefaultExperience += mediumScaleExperience * GameState.instance.player.ExperienceNeeded;
            hardDefaultExperience += hardScaleExperience * GameState.instance.player.ExperienceNeeded;
            bossDefaultExperience += bossScaleExperience * GameState.instance.player.ExperienceNeeded;

            // set the experience a second time once scaled to overwrite the default
            SetExperience(currentExperienceType);
        }
    }

    public void AddExperiencePointBackToSpawnPool()
    {
        Spawning.instance.experience.AddToPool(this);
    }

    IEnumerator ICollectable.Collect()
    {
        if (GameState.instance.player.playerDead == true) { yield break; }
        while (gameObject.activeSelf == true)
        {
            // once the object has been collected and turn off, the coroutine will stop
            Vector3 targetPosition = (GameState.instance.playerTransform.position - transform.position).normalized;
            transform.position += targetPosition * GameState.instance.playerInventory.collectionSpeed * Time.deltaTime;

            yield return null;
        }
    }
}
