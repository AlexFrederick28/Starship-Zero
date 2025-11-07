using UnityEngine;

public class ExperiencePoint : MonoBehaviour
{
    public EnemyBase.DifficultyType currentExperienceType;

    [SerializeField] private float currentExperienceAmount;
    [SerializeField] private float easyDefaultExperience;
    [SerializeField] private float mediumDefaultExperience;
    [SerializeField] private float hardDefaultExperience;
    [SerializeField] private float bossDefaultExperience;

    private void Start()
    {
        //ScaleExperience();
    }

    private void OnEnable()
    {
        // setting the experience upon enabing the game object as to allow for the correct type of experience to spawn when a certain type of enemy dies
        SetExperience(currentExperienceType);
    }

    private void SetExperience(EnemyBase.DifficultyType currentType)
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

    public void ScaleExperience(float currentExperience)
    {
        // scale the current experience to whatever it should be based on quest level

    }
}
