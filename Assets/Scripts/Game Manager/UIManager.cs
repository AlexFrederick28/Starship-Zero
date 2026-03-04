using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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
    public float notificationTime;
    public float notificationYPosition;
    public float notificationYDestination;
    public float notificationSpeed;

    [Space]
    [Header("Inventory")]
    public GameObject inventoryParent;
    public GameObject inventorySlotContentParent;
    public GameObject slot;
    public TextMeshProUGUI infoInventoryName;
    public Image infoInventoryImage;
    public TextMeshProUGUI infoWeaponInventoryText;
    public TextMeshProUGUI infoInventoryText;
    public TextMeshProUGUI weaponStatInventoryText;
    public TextMeshProUGUI selectedStackAmount;
    public Slider stackAmountSlider;
    public Button weaponLoadoutButton;

    [Space]
    [Header("Permanent Upgrades")]


    [Space]
    [Header("Death Menu")]
    public GameObject deathMenuParent;
    public Button respawnButton;
    public Button retryInfestedRoomButton;

    [Space]
    [Header("Level Up Menu")]
    public GameObject levelUpMenuParent;
    public GameObject[] levelUpCards;
    public TextMeshProUGUI[] levelUpItemName;
    public TextMeshProUGUI[] levelUpItemStatDescription;
    public TextMeshProUGUI[] levelUpItemLevel;
    public Image[] levelUpItemImage;

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
    public Scrollbar weaponListScrollbar;
    public GameObject weaponCraftingRecipeParent;
    public GameObject weaponCraftingRecipePrefab;
    public GameObject weaponUnlockButton;
    public GameObject weaponCraftButton;
    public List<GameObject> weaponCraftingRecipePrefabList;

    [Header("Dialogue")]
    [Space]
    public GameObject canvas;
    public GameObject dialogueParent;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject continueButton;

    [Header("Quests")]
    [Space]
    public GameObject questParent;
    public GameObject questPrefab;

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
    }

    private void OnDisable()
    {
        if (instance == this)
        {
            instance = null;
        }
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
    /// Closes all of the UI the player may have open, such as their inventory, or a crafting bench. Changes the state to main
    /// </summary>
    public void CloseAllInteractiveUI()
    {
        if (GameState.instance.currentState == GameState.States.RoomClear) { return; }
        inventoryParent.SetActive(false);
        weaponCraftingUIParent.SetActive(false);
        GameState.instance.ChangeStateToMain();
    }

    public void SwapToInventoryUI()
    {
        // tab 1
    }

    public void SwapToWeaponCraftingUI()
    {
        // tab 2
    }

    public void SwapToPermanentUpgradeUI()
    {
        // tab 3
    }
}
