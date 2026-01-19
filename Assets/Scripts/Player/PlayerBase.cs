using System.Collections;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerBase : MonoBehaviour
{
    public int Level { get; private set; }
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

            if (experienceNeeded <= 0)
            {
                return;
            }

            while (currentExperience >= experienceNeeded)
            {
                LevelUp();
                currentExperience -= experienceNeeded;
                ExperienceNeeded();
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

    private IInteractable interactable;

    private void Start()
    {
        ExperienceNeeded();
        //CurrentExperience += 1000;
    }

    private void Update()
    {
        if (GameState.instance != null && GameState.instance.player == null)
        {
            // setting the reference for the player so that global scripts can access the data if necessary
            GameState.instance.player = this;
        }

        // temp function
        SetPlayerHealthSlider();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (interactable != null)
            {
                interactable.OnInteract();
            }
        }
    }

    public void TakeDamage(float damage)
    {
        // enemy take damage from player

        Health -= damage;
        Debug.Log(name + " took " + damage + "!");
    }

    private void ExperienceNeeded()
    {
        if (experienceNeeded != (int)experienceCurve.Evaluate(Level + 1))
        {
            // calculation using animation curve to determine the experience needed for the next level
            experienceNeeded = (int)experienceCurve.Evaluate(Level + 1);
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
        StartCoroutine(UIManager.instance.NewNotification("Level +1"));
        Level++;
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<IInteractable>() != null)
        {
            interactable = collision.gameObject.GetComponent<IInteractable>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (interactable != null && interactable == collision.gameObject.GetComponent<IInteractable>())
        {
            interactable.OnEndInteraction();
            interactable = null;
        }
    }

    private void SetPlayerHealthSlider()
    {
        UIManager.instance.playerLevel.text = "Level: " + Level;
        UIManager.instance.playerLevelSlider.maxValue = experienceNeeded;
        UIManager.instance.playerLevelSlider.minValue = 0f;
        UIManager.instance.playerLevelSlider.value = currentExperience;
        UIManager.instance.playerHealthSlider.maxValue = maxHealth;
        UIManager.instance.playerHealthSlider.minValue = 0f;
        UIManager.instance.playerHealthSlider.value = currentHealth;
    }
}
