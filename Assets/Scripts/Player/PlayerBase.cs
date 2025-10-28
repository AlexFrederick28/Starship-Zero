using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private int currentExperience;
    
    private int CurrentExperience
    {
        get
        {
            return currentExperience;
        }
        set
        {
            if (currentExperience >= experienceNeeded)
            {
                LevelUp();
                currentExperience = 0;
            }
            if (currentExperience < 0)
            {
                currentExperience = 0;
            }
        }
    }

    [SerializeField] private int experienceNeeded;
    [SerializeField] private AnimationCurve experienceCurve;

    [Space]
    [SerializeField] private float health;
    public float speed;

    private void Update()
    {
        ExperienceNeeded();
    }

    private void ExperienceNeeded()
    {
        if (experienceNeeded != (int)experienceCurve.Evaluate(level + 1))
        {
            // calculation using animation curve to determine the experience needed for the next level
            experienceNeeded = (int)experienceCurve.Evaluate(level + 1);
            //Debug.Log("Updated Experience Needed");
        }
    }

    public void AddExperience(int amount)
    {
        currentExperience += amount;
    }

    public void LevelUp()
    {
        Debug.Log("Leveled Up!");
    }
}
