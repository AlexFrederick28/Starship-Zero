using System;
using System.Collections;
using System.Runtime.CompilerServices;
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
                CalculateExperienceNeeded();
                GameState.instance.OnPlayerLevelUp?.Invoke(); // pauses the game when the player levels up. Needs to be invoked a second time to unpause
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
    [SerializeField] private float recordedMaxHealth;

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
    public float recordedSpeed;
    public int currency;

    private IInteractable interactable;

    [Space]
    [Header("Audio")]
    [SerializeField] protected float volume;
    [SerializeField] protected float minPitch;
    [SerializeField] protected float maxPitch;
    [SerializeField] protected AudioClip playerHurtClip;

    public static Action OnPlayerDeath;
    public static Action OnPressingRetryOrRespawn;

    public bool playerDead = false;
    public bool PlayerDead()
    {
        if (currentHealth <= 0)
        {
            UIManager.instance.deathMenuParent.SetActive(true);
            if (Spawning.instance != null)
            {
                UIManager.instance.respawnButton.onClick.AddListener(Spawning.instance.checkpoint.Respawn);
                UIManager.instance.retryInfestedRoomButton.onClick.AddListener(Spawning.instance.checkpoint.RetryInfestedRoom);
                PauseAndUnpausePlayer(); // pause player
            }
            OnPlayerDeath?.Invoke();
            playerDead = true;
            return true;
        }
        else
        {
            // this part will play once the player has pressed respawn or retry, as that resets the players health to max
            if (UIManager.instance.deathMenuParent.activeSelf == true)
            {
                UIManager.instance.deathMenuParent.SetActive(false);
                PauseAndUnpausePlayer(); // unpause player
                if (Spawning.instance != null)
                {
                    UIManager.instance.respawnButton.onClick.RemoveAllListeners();
                    UIManager.instance.retryInfestedRoomButton.onClick.RemoveAllListeners();
                }
                OnPressingRetryOrRespawn?.Invoke();
                playerDead = false;
            }
            return false;
        }
    }

    public bool PauseAndUnpausePlayer()
    {
        if (GetComponent<PlayerMovement>().enabled == false)
        {
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
            GameState.instance.OnEnteringInfestedRoom += RecordVariablesOnRoomStart;
            GameState.instance.OnCompletedInfestedClear += ResetVariablesToRecorded;
            GameState.instance.OnPlayerRespawn += ResetVariablesToRecorded;
            GameState.instance.OnPlayerRetry += ResetVariablesToRecorded;

            GameState.instance.OnPlayerRespawn += ResetPlayerStatsOnRespawn;
            GameState.instance.OnPlayerRetry += ResetPlayerStatsOnRespawn;
            GameState.instance.OnPlayerLevelUp += PausePlayerOnPlayerLevelUp;
            GameState.instance.OnCompletedInfestedClear += ResetPlayerHealth;
        }
    }

    private void OnDisable()
    {
        GameState.instance.OnEnteringInfestedRoom -= RecordVariablesOnRoomStart;
        GameState.instance.OnCompletedInfestedClear -= ResetVariablesToRecorded;
        GameState.instance.OnPlayerRespawn -= ResetVariablesToRecorded;
        GameState.instance.OnPlayerRetry -= ResetVariablesToRecorded;

        GameState.instance.OnPlayerRespawn -= ResetPlayerStatsOnRespawn;
        GameState.instance.OnPlayerRetry -= ResetPlayerStatsOnRespawn;
        GameState.instance.OnPlayerLevelUp -= PausePlayerOnPlayerLevelUp;
        GameState.instance.OnCompletedInfestedClear -= ResetPlayerHealth;
    }

    private void Start()
    {
        CalculateExperienceNeeded();
        //CurrentExperience += 1000;
    }

    private void Update()
    {
        if (GameState.instance != null && GameState.instance.player == null)
        {
            // setting the reference for the player so that global scripts can access the data if necessary
            GameState.instance.playerTransform = transform;
            GameState.instance.playerInventory = GetComponent<Inventory>();
            GameState.instance.player = this;
        }

        // temp function for player UI
        SetPlayerUI();
    }

    public void RecordVariablesOnRoomStart()
    {
        recordedSpeed = speed;
        recordedMaxHealth = maxHealth;
    }

    public void ResetVariablesToRecorded()
    {
        speed = recordedSpeed;
        maxHealth = recordedMaxHealth;
    }

    private void PausePlayerOnPlayerLevelUp()
    {
        PauseAndUnpausePlayer();
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

        if (Health != 0)
        {
            Health -= damage;
            SoundManager.instance.PlaySoundClip(playerHurtClip, transform, volume, true, true, minPitch, maxPitch);
            Debug.Log(name + " took " + damage + "!");
        }
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

    public void AddHealth(int amount)
    {
        Health += amount;
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerDead == true) { return; }
        if (collision.gameObject.GetComponent<ExperiencePoint>())
        {
            ExperiencePoint point = collision.gameObject.GetComponent<ExperiencePoint>();
            AddExperience((int)point.currentExperienceAmount);
            Spawning.instance.experience.AddToPool(point);
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
        UIManager.instance.playerCurrency.text = "Currency: $" + currency.ToString();
    }

    public void ResetPlayerStatsOnRespawn()
    {
        Level = (int)Spawning.instance.questLevel.x;
        CalculateExperienceNeeded();
        CurrentExperience = (int)Spawning.instance.questLevel.y;
        ResetPlayerHealth();
    }

    public void ResetPlayerHealth()
    {
        Health = maxHealth;
    }
}
