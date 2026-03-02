using System;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WeaponCrafting : MonoBehaviour, IPointerClickHandler
{
    public bool weaponUnlocked = false;
    public bool canCraft = false;
    public CraftingRecipe recipe;
    public Image weaponLockedImage;
    public bool craftedFirstWeaponTutorial = false;

    public static Action OnFirstWeaponCraft;

    private void OnEnable()
    {
        CheckCraftingPossibility();
    }

    private void OnDisable()
    {
        // resetting crafting on disabling the menu as the crafting possibilty needs to be rechecked
        canCraft = false;
    }

    public void CheckCraftingPossibility()
    {
        if (weaponUnlocked == false) { return; }
        if (GameState.instance.player.currency < recipe.purchaseCost) { Debug.Log("Player does not have enough currency to craft"); return; }
        // function should play on click when the item is selected, or possible when the weapon crafting menu is enabled (Would allow for reactive UI such as green text to visually tell the player they can craft it)
        int amountOfIngredients = 0;
        int inventoryContains = 0;
        for (int x = 0; x < recipe.ingredients.Length; x++)
        {
            amountOfIngredients++;
            int amountOfCurrentItem = 0;
            // for each ingredient check if the player has the required amount
            for (int i = 0; i < GameState.instance.playerInventory.inventorySlots.Count; i++)
            {
                if (GameState.instance.playerInventory.inventorySlots[i].inventoryItem == null) { continue; }
                if (GameState.instance.playerInventory.inventorySlots[i].inventoryItem.itemName == recipe.ingredients[x].ingredient.itemName)
                {
                    // getting the amount of the item from the inventory slot itself, as it will have the same list position as the current item
                    amountOfCurrentItem += GameState.instance.playerInventory.inventorySlots[i].currentStackSize;
                }
            }

            if (recipe.ingredients[x].amount <= amountOfCurrentItem)
            {
                inventoryContains++;
            }
        }

        if (amountOfIngredients == inventoryContains)
        {
            // once we have checked to see if we have all the ingredients in the players inventory, SHOW the player they can craft it (like a red or green text/background)
            canCraft = true;
        }
    }

    public void ShowCraftingSummary()
    {
        // when this weapon is clicked in the crafting menu, show all thd details related to it

        UIManager.instance.weaponCraftingSelectedDescription.text = string.Empty;
        UIManager.instance.weaponCraftingSelectedDescriptionAmount.text = string.Empty;
        for (int i = 0; i < recipe.weaponDescription.Length; i++)
        {
            if (i == 0)
            {
                UIManager.instance.weaponCraftingSelectedDescription.text += recipe.weaponDescription[i].description;
                UIManager.instance.weaponCraftingSelectedDescriptionAmount.text += recipe.weaponDescription[i].amount;
            }
            else
            {
                UIManager.instance.weaponCraftingSelectedDescription.text += "\n" + recipe.weaponDescription[i].description;
                UIManager.instance.weaponCraftingSelectedDescriptionAmount.text += "\n" + recipe.weaponDescription[i].amount;
            }
        }
        UIManager.instance.weaponCraftingSelectedImage.sprite = recipe.weapon.weaponSprite;
        UIManager.instance.weaponCraftingSelectedName.text = recipe.weapon.weaponName;

        if (UIManager.instance.weaponCraftingRecipePrefabList.Count > 0)
        {
            for (int x = 0; x < UIManager.instance.weaponCraftingRecipePrefabList.Count; x++)
            {
                Destroy(UIManager.instance.weaponCraftingRecipePrefabList[x].gameObject);
            }
            UIManager.instance.weaponCraftingRecipePrefabList.Clear();
        }
        if (weaponUnlocked == false) { return; } // if the weapon is not unlocked, dont show the recipe
        for (int i = 0; i < recipe.ingredients.Length; i++)
        {
            GameObject newRecipe = Instantiate(UIManager.instance.weaponCraftingRecipePrefab, UIManager.instance.weaponCraftingRecipeParent.transform);
            RecipePanelDisplay display = newRecipe.GetComponent<RecipePanelDisplay>();
            display.recipeName.text = recipe.ingredients[i].ingredient.itemName;
            display.recipeAmount.text = recipe.ingredients[i].amount.ToString();
            display.recipeImage.sprite = recipe.ingredients[i].ingredient.sprite;
            UIManager.instance.weaponCraftingRecipePrefabList.Add(newRecipe);
        }
    }

    public void UnlockWeapon()
    {
        // use currency to unlock a weapon, to then be able to craft it
        if (weaponUnlocked == true) { return; }
        if (GameState.instance.player.currency >= recipe.unlockCost)
        {
            weaponUnlocked = true;
            GameState.instance.player.currency -= recipe.unlockCost;
            UIManager.instance.NewNotification("Currency -" + recipe.unlockCost);
            UIManager.instance.weaponUnlockButton.SetActive(false);
            UIManager.instance.weaponCraftButton.SetActive(true);
            UIManager.instance.weaponCraftButton.GetComponentInChildren<TextMeshProUGUI>().text = "Craft: $" + recipe.purchaseCost.ToString();
            UIManager.instance.weaponCraftButton.GetComponent<Button>().onClick.RemoveAllListeners();
            UIManager.instance.weaponCraftButton.GetComponent<Button>().onClick.AddListener(CraftWeapon);
            weaponLockedImage.enabled = false;
        }

        ShowCraftingSummary();
        CheckCraftingPossibility();
    }

    public void CraftWeapon()
    {
        // take the necessary items out of the players inventory and add this weapon to their inventory
        Inventory inventory = GameState.instance.playerInventory;
        if (recipe.isFree == true)
        {
            for (int i = 0; i < inventory.inventorySlots.Count; i++)
            {
                if (inventory.inventorySlots[i].inventoryItem == recipe.item)
                {
                    Debug.Log("Player inventory already contains this free item!");
                    return;
                }
            }
        }
        else 
        {
            CheckCraftingPossibility();
            if (canCraft == false)
            {
                Debug.Log("Not enough ingredients to craft"); 
                return;
            }
            for (int x = 0; x < recipe.ingredients.Length; x++)
            {
                // for each ingredient take the required amount for the recipe
                for (int i = 0; i < recipe.ingredients.Length; i++)
                {
                    InventoryItemPackage item = recipe.ingredients[x].ingredient;
                    GameState.instance.playerInventory.RemoveItem(item, recipe.ingredients[x].amount, recipe.ingredients[i].ingredient.itemName);
                }
            }
            GameState.instance.player.currency -= recipe.purchaseCost;
        }

        GameState.instance.playerInventory.AddItemToInventory(recipe.item);
        Debug.Log("Crafted weapon: " + recipe.weapon.weaponName);
        if (craftedFirstWeaponTutorial == false)
        {
            OnFirstWeaponCraft?.Invoke();
            craftedFirstWeaponTutorial = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.weaponUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
        UIManager.instance.weaponCraftButton.GetComponent<Button>().onClick.RemoveAllListeners();

        if (weaponUnlocked == true)
        {
            UIManager.instance.weaponUnlockButton.SetActive(false);
            UIManager.instance.weaponCraftButton.SetActive(true);
            UIManager.instance.weaponCraftButton.GetComponentInChildren<TextMeshProUGUI>().text = "Craft: $" + recipe.purchaseCost.ToString();
            UIManager.instance.weaponCraftButton.GetComponent<Button>().onClick.AddListener(CraftWeapon);
        }
        else
        {
            UIManager.instance.weaponUnlockButton.SetActive(true);
            UIManager.instance.weaponCraftButton.SetActive(false);
            UIManager.instance.weaponUnlockButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unlock: $" + recipe.unlockCost.ToString();
            UIManager.instance.weaponUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockWeapon);
        }

        ShowCraftingSummary();
        CheckCraftingPossibility();
    }
}
