using Unity.VisualScripting;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private int currentExperience;
    public int CurrentExperience
    {
        get
        {
            return currentExperience;
        }
        set
        {
            currentExperience = value;

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
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;

    public float Health
    {
        get
        {
            return currentHealth;
        }
        set
        {
            if (value > maxHealth)
            {
                value = maxHealth;
            }
            if (value < 0)
            {
                value = 0;
            }

            currentHealth = value;
        }
    }

    public float speed;

    private void Update()
    {
        ExperienceNeeded();
    }

    public void TakeDamage(float damage)
    {
        // enemy take damage from player

        Health -= damage;
        Debug.Log(name + " took " + damage + "!");
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
        CurrentExperience += amount;
    }

    public void LevelUp()
    {
        Debug.Log("Leveled Up!");
        level++;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<ExperiencePoint>())
        {
            ExperiencePoint point = collision.gameObject.GetComponent<ExperiencePoint>();
            AddExperience((int)point.currentExperienceAmount);
            Spawning.instance.GetComponent<Experience>().AddToPool(point);
        }
    }
}
