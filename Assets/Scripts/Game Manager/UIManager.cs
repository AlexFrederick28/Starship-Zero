using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Player")]
    public GameObject playerUIParent;
    public Slider playerHealthSlider;
    public Slider playerLevelSlider;
    public TextMeshProUGUI playerCurrency;
    public TextMeshProUGUI playerLevel;
    public GameObject notificationPrefab;
    public GameObject largeNotificationPrefab;
    public float notificationTime;
    public float notificationYPosition;
    public float notificationYDestination;
    public float notificationSpeed;

    [Space]
    [Header("Tabs")]
    public GameObject tabParent;
    public Color originalColor;
    public Color lockedColor;
    public Image weaponCraftingButtonImage;
    public EventTrigger weaponCraftingButtonEventTrigger;
    public GameObject weaponCraftingButtonText;
    public Button weaponCraftingButton;
    public Image cyberneticUpgradeButtonImage;
    public Image cyberneticUpgradeButtonEventTrigger;
    public GameObject cyberneticUpgradeButtonText;
    public Button cyberneticUpgradeButton;
    public Image cardUpgradeButtonImage;
    public Image cardUpgradeButtonEventTrigger;
    public GameObject cardUpgradeButtonText;
    public Button cardUpgradeButtonButton;

    [Space]
    [Header("Inventory")]
    public GameObject inventoryParent;
    public GameObject inventorySlotContentParent;
    public GameObject slot;
    public GameObject projectileSelectButton;
    public Image projectileSelectButtonImage;
    public GameObject projectileSelectDropDown;
    public TextMeshProUGUI infoInventoryName;
    public Image infoInventoryImage;
    public TextMeshProUGUI infoWeaponInventoryText;
    public TextMeshProUGUI infoInventoryText;
    public TextMeshProUGUI weaponStatInventoryText;
    public TextMeshProUGUI selectedStackAmount;
    public Slider stackAmountSlider;
    public Button weaponLoadoutButton;

    [Space]
    [Header("Weapon Inventory")]
    public GameObject weaponInventorySlotContentParent;
    public GameObject weaponInventorySlot;

    [Space]
    [Header("Card Unlock")]
    public GameObject cardUnlockParent;
    public Image cardDescriptionImage;
    public TextMeshProUGUI cardDescriptionText;
    public TextMeshProUGUI cardDescriptionNameText;
    public Button cardUnlockButton; // use the button to apply the unlock cost visually

    [Space]
    [Header("Permanent Upgrades")]
    public GameObject upgradeParent;
    public GameObject upgradeCardParent;

    [Space]
    [Header("Death Menu")]
    public GameObject deathMenuParent;
    public Button respawnButton;
    public Button retryInfestedRoomButton;

    [Space]
    [Header("Navigation Menu")]
    public GameObject navigationNonQuestScrollView;
    public GameObject navigationQuestScrollView;
    public GameObject navigationMenuParent;
    public GameObject navigationQuestParent;
    public GameObject navigationEasyUIParent;
    public GameObject navigationMediumUIParent;
    public GameObject navigationHardUIParent;
    public TextMeshProUGUI navigationRoomInfoText;
    public Button navigationTravelButton;

    [Serializable]
    public class LevelUpCardUI
    {
        public GameObject levelUpCard;
        public TextMeshProUGUI levelUpItemName;
        public TextMeshProUGUI levelUpItemStatDescription;
        public TextMeshProUGUI levelUpItemLevel;
        public TextMeshProUGUI levelUpItemSlot;
        public Image levelUpItemImage;
    }

    [Space]
    [Header("Level Up Menu")]
    public GameObject levelUpMenuParent;
    public LevelUpCardUI[] levelUpCardUIList;

    [Space]
    [Header("Infested Room")]
    public TextMeshProUGUI currentTime;
    public TextMeshProUGUI currentDifficulty;

    [Space]
    [Header("Weapon Room")]
    public GameObject weaponCraftingUIParent;
    public TextMeshProUGUI weaponCraftingSelectedName;
    public TextMeshProUGUI weaponCraftingSelectedDescription;
    public TextMeshProUGUI weaponCraftingSelectedDescriptionAmount;
    public Image weaponCraftingSelectedImage;
    public GameObject weaponCraftingRecipeParent;
    public GameObject weaponCraftingRecipePrefab;
    public GameObject weaponUnlockButton;
    public GameObject weaponCraftButton;
    public GameObject weaponListParent;
    public GameObject projectileListParent;
    public List<GameObject> weaponCraftingRecipePrefabList;

    [Space]
    [Header("Super Computer")]
    public Image newDialogueIcon;

    [Space]
    [Header("Dialogue")]
    public GameObject canvas;
    public GameObject dialogueParent;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject continueButton;

    [Header("Quests")]
    [Space]
    public GameObject questParent;
    public GameObject questPrefab;

    public Action OnOpenedUI;
    public Action OnClosedUI;

    [Header("Pause UI")]
    [Space]

    public GameObject pauseMenuParent;
    public GameObject pauseButtonPanel;
    public GameObject audioPanel;
    public GameObject controlPanel;

    public Slider MasterSlider;// access the slider volume - MasterSlider.value
    public Slider MusicSlider;
    public Slider SoundsEffectsSlider;


    public static UIManager instance;
    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        OnOpenedUI += OpenTabUI;
        OnClosedUI += CloseTabUI;
    }

    private void OnDisable()
    {
        if (instance == this)
        {
            instance = null;
        }

        OnOpenedUI -= OpenTabUI;
        OnClosedUI -= CloseTabUI;
    }

    private void Start()
    {
        if (dialogueParent == null)
        {
            GameObject parent = Instantiate(dialogueParent);
            parent.transform.SetParent(canvas.transform);
        }
        if (questParent == null)
        {
            GameObject parent = Instantiate(questParent);
            parent.transform.SetParent(canvas.transform);
        }
    }

    private void Update()
    {
        GetUINumbersTEMP();
    }

    public void OpenTabUI()
    {
        tabParent.SetActive(true);
        //Debug.Log("Opened tab parent");
    }

    public void CloseTabUI()
    {
        tabParent.SetActive(false);
        //Debug.Log("Closed tab parent");
    }

    public void GetUINumbersTEMP()
    {
        if (Spawning.instance != null)
        {
            // this is a temporary function to get the infested room time working for prototype testing
            float difficulty = Spawning.instance.CurrentDifficulty;
            currentTime.enabled = true;
            currentDifficulty.enabled = true;

            currentTime.text = "|Time|" + "\n" + Spawning.instance.currentMinuteTime.x.ToString() + ":" + Spawning.instance.currentMinuteTime.y;
            currentDifficulty.text = "|Difficulty|" + "\n" + (int)difficulty;
        }
        else
        {
            currentTime.enabled = false;
            currentDifficulty.enabled = false;
        }
    }

    /// <summary>
    /// A coroutine that displays a notification above the player
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    public IEnumerator NewNotification(string description)
    {
        GameObject newNotification = Instantiate(instance.notificationPrefab);
        newNotification.transform.SetParent(instance.playerUIParent.transform);
        newNotification.GetComponent<TextMeshProUGUI>().text = description;
        newNotification.GetComponent<RectTransform>().localPosition = new Vector3(0, notificationYPosition, 0);

        float timer = 0;
        while (timer < notificationTime)
        {
            Vector3 newPos = new Vector3(0, notificationYPosition, 0) - new Vector3 (0, notificationYDestination, 0);
            newNotification.transform.localPosition -= newPos * Time.deltaTime * notificationSpeed;
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForEndOfFrame();
        Destroy(newNotification);
    }

    /// <summary>
    /// A large notification coroutine that plays on the UIManager.cs
    /// </summary>
    /// <param name="routine"></param>
    public void SpawnLargeNotification(string description, float screenTime, float sizeIncrease)
    {
        Debug.Log("Spawned large notification");
        GameObject newNotification = Instantiate(instance.largeNotificationPrefab);
        newNotification.transform.SetParent(instance.canvas.transform);
        newNotification.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

        LargeNotification noti = newNotification.GetComponent<LargeNotification>();
        noti.description = description;
        noti.screenTime = screenTime;
        noti.sizeIncrease = sizeIncrease;
    }

    /// <summary>
    /// Closes all of the UI the player may have open, such as their inventory, or a crafting bench. Changes the state to main
    /// </summary>
    public void CloseAllInteractiveUI()
    {
        if (GameState.instance.currentState == GameState.States.Paused) { return; }
        if (GameState.instance.currentState == GameState.States.RoomClear || GameState.instance.currentState == GameState.States.Main ) { return; }
        inventoryParent.SetActive(false);
        weaponCraftingUIParent.SetActive(false);
        upgradeParent.SetActive(false);
        cardUnlockParent.SetActive(false);
        tabParent.SetActive(false);
        navigationMenuParent.SetActive(false);

        StopAllCoroutines();
        StartCoroutine(CloseInteractiveUIDelay());
    }

    IEnumerator CloseInteractiveUIDelay()
    {
        yield return new WaitForSeconds(0.1f);

        GameState.instance.ChangeStateToMainWithoutMusic();

    }

    public void SwapToInventoryUI()
    {
        // tab 1
        inventoryParent.SetActive(true);
        weaponCraftingUIParent.SetActive(false);
        upgradeParent.SetActive(false);
        cardUnlockParent.SetActive(false);
        navigationMenuParent.SetActive(false);
    }

    public void SwapToWeaponCraftingUI()
    {
        if (GameState.instance.weaponCraftingUnlocked == false) { return; }

        // tab 2
        inventoryParent.SetActive(false);
        weaponCraftingUIParent.SetActive(true);
        upgradeParent.SetActive(false);
        cardUnlockParent.SetActive(false);
        navigationMenuParent.SetActive(false);
    }

    public void SwapToCardUI()
    {
        if (GameState.instance.cyberneticUpgradesUnlocked == false) { return; }

        // tab 3
        inventoryParent.SetActive(false);
        weaponCraftingUIParent.SetActive(false);
        upgradeParent.SetActive(false);
        cardUnlockParent.SetActive(true);
        navigationMenuParent.SetActive(false);
    }

    public void SwapToPermanentUpgradeUI()
    {
        if (GameState.instance.cyberneticUpgradesUnlocked == false) { return; }

        // tab 4
        inventoryParent.SetActive(false);
        weaponCraftingUIParent.SetActive(false);
        upgradeParent.SetActive(true);
        cardUnlockParent.SetActive(false);
        navigationMenuParent.SetActive(false);
    }

    public void SwapToNavigationUI()
    {
        // tab 5
        inventoryParent.SetActive(false);
        weaponCraftingUIParent.SetActive(false);
        upgradeParent.SetActive(false);
        cardUnlockParent.SetActive(false);
        navigationMenuParent.SetActive(true);
    }

    public void SwapToProjectileTabUI()
    {
        projectileListParent.SetActive(true);
        weaponListParent.SetActive(false);
    }

    public void SwapToWeaponTabUI()
    {
        projectileListParent.SetActive(false);
        weaponListParent.SetActive(true);
    }

    public void SwapToNonQuestNavigationUI()
    {
        navigationNonQuestScrollView.SetActive(true);
        navigationQuestScrollView.SetActive(false);
    }

    public void SwapToQuestNavigationUI()
    {
        navigationNonQuestScrollView.SetActive(false);
        navigationQuestScrollView.SetActive(true);
    }
}
