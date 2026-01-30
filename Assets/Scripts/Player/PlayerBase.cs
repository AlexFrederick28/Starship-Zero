using System.Collections;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.UIElements;
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
                CalculateExperienceNeeded();
            }
            if (currentExperience < 0)
            {
                currentExperience = 0;
            }
        }
    }

    [SerializeField] private int experienceNeeded;
    public int ExperienceNeeded
    {
        get { return experienceNeeded; }
        private set
        {
            experienceNeeded = value;
        }
    }
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
            PlayerDead(); // will play each time the player takes damage (Checks to see if the player should be dead)
        }
    }

    public float speed;
    public int currency;

    private IInteractable interactable;

    public bool PlayerDead()
    {
        if (currentHealth <= 0)
        {
            UIManager.instance.deathMenuParent.SetActive(true);
            if (Spawning.instance != null)
            {
                UIManager.instance.respawnButton.onClick.AddListener(Spawning.instance.checkpoint.Respawn);
                UIManager.instance.retryInfestedRoomButton.onClick.AddListener(Spawning.instance.checkpoint.RetryInfestedRoom);
                PausePlayer();
            }
            return true;
        }
        else
        {
            // this part will play once the player has pressed respawn or retry, as that resets the players health to max
            if (UIManager.instance.deathMenuParent.activeSelf == true)
            {
                UIManager.instance.deathMenuParent.SetActive(false);
                PausePlayer();
                if (Spawning.instance != null)
                {
                    UIManager.instance.respawnButton.onClick.RemoveAllListeners();
                    UIManager.instance.retryInfestedRoomButton.onClick.RemoveAllListeners();
                }
            }
            return false;
        }
    }

    public bool PausePlayer()
    {
        if (GetComponent<PlayerMovement>().enabled == false)
        {
            // TODO: pause weapon as well
            GetComponent<PlayerMovement>().enabled = true;
            return true;
        }
        else
        {
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            return false;
        }
    }

    private void OnEnable()
    {
        if (GameState.instance != null)
        {
            GameState.instance.OnPlayerRespawn += ResetPlayerStatsOnRespawn;
            GameState.instance.OnPlayerRetry += ResetPlayerStatsOnRespawn;
        }
    }

    private void OnDisable()
    {
        GameState.instance.OnPlayerRespawn -= ResetPlayerStatsOnRespawn;
        GameState.instance.OnPlayerRetry -= ResetPlayerStatsOnRespawn;
    }

    private void Start()
    {
        CalculateExperienceNeeded();
        CurrentExperience += 1000;
    }

    private void Update()
    {
        if (GameState.instance != null && GameState.instance.player == null)
        {
            // setting the reference for the player so that global scripts can access the data if necessary
            GameState.instance.playerTransform = transform;
            GameState.instance.player = this;
        }

        // temp function for player UI
        SetPlayerUI();
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

    private void CalculateExperienceNeeded()
    {
        if (ExperienceNeeded != (int)experienceCurve.Evaluate(Level + 1))
        {
            // calculation using animation curve to determine the experience needed for the next level
            ExperienceNeeded = (int)experienceCurve.Evaluate(Level + 1);
            //Debug.Log("Updated Experience Needed");
        }
    }

    public void AddExperience(int amount)
    {
        CurrentExperience += amount;
        StartCoroutine(UIManager.instance.NewNotification("Exp +" + amount));
    }

    public void LevelUp()
    {
        Debug.Log("Leveled Up!");
        StartCoroutine(UIManager.instance.NewNotification("Level +1"));
        Level++;
    }

    public void AddCurrencyFromNPC(NPCBase npc)
    {
        if (npc.currentDialogue.quest.prerequisite.currencyReward > 0)
        {
            currency += npc.currentDialogue.quest.prerequisite.currencyReward;
            StartCoroutine(UIManager.instance.NewNotification("Currency +" + npc.currentDialogue.quest.prerequisite.currencyReward));
        }
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
        else if (collision.gameObject.GetComponentInChildren<IInteractable>() != null)
        {
            interactable = collision.gameObject.GetComponentInChildren<IInteractable>();
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

    private void SetPlayerUI()
    {
        UIManager.instance.playerLevel.text = "Level: " + Level;
        UIManager.instance.playerLevelSlider.maxValue = ExperienceNeeded;
        UIManager.instance.playerLevelSlider.minValue = 0f;
        UIManager.instance.playerLevelSlider.value = currentExperience;
        UIManager.instance.playerHealthSlider.maxValue = maxHealth;
        UIManager.instance.playerHealthSlider.minValue = 0f;
        UIManager.instance.playerHealthSlider.value = currentHealth;
    }

    public void ResetPlayerStatsOnRespawn()
    {
        Level = (int)Spawning.instance.questLevel.x;
        CalculateExperienceNeeded();
        CurrentExperience = (int)Spawning.instance.questLevel.y;
        Health = maxHealth;
    }
}
