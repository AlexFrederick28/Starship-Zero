using UnityEngine;

public class ExperiencePoint : MonoBehaviour
{
    public EnemyBase.DifficultyType currentExperienceType;

    public float currentExperienceAmount;
    [SerializeField] private float easyDefaultExperience;
    [SerializeField] private float mediumDefaultExperience;
    [SerializeField] private float hardDefaultExperience;
    [SerializeField] private float bossDefaultExperience;

    private void OnEnable()
    {
        // setting the experience upon enabing the game object as to allow for the correct type of experience to spawn when a certain type of enemy dies
        //SetExperience(currentExperienceType);
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

    public void ScaleExperience(float currentExperience)
    {
        // scale the current experience to whatever it should be based on quest level
        if (Spawning.instance != null)
        {
            for (int i = 0; i < QuestManager.instance.activeQuests.Count; i++)
            {
                if (QuestManager.instance.activeQuests[i].prerequisite.id == Spawning.instance.questID)
                {
                    // checking the spawn ID matches an active quest and sets the experience amount
                    float previousExperience = currentExperience;
                    easyDefaultExperience = QuestManager.instance.activeQuests[i].prerequisite.level;
                    mediumDefaultExperience = QuestManager.instance.activeQuests[i].prerequisite.level;
                    hardDefaultExperience = QuestManager.instance.activeQuests[i].prerequisite.level;
                    bossDefaultExperience = QuestManager.instance.activeQuests[i].prerequisite.level;

                    // set the experience a second time once scaled to overwrite the default
                    SetExperience(currentExperienceType);
                    Debug.Log("Experience scaled to " + currentExperience + " from " + previousExperience);
                    break;
                }
            }
        }
    }
}
