using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCrafting : MonoBehaviour
{
    public bool weaponUnlocked = false;
    public bool canCraft = false;
    public CraftingRecipe recipe;
    public Image weaponLockedImage;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        // resetting crafting on disabling the menu as the crafting possibilty needs to be rechecked
        canCraft = false;
        weaponLockedImage.enabled = true;
    }

    public void CheckCraftingPossibility()
    {
        // function should play on click when the item is selected, or possible when the weapon crafting menu is enabled (Would allow for reactive UI such as green text to visually tell the player they can craft it)
        int amountOfIngredients = 0;
        int inventoryContains = 0;
        for (int x = 0; x < recipe.ingredients.Length; x++)
        {
            amountOfIngredients++;
            int amountOfCurrentItem = 0;
            // for each ingredient check if the player has the required amount
            for (int i = 0; i < GameState.instance.playerInventory.InventoryItemList.Count; i++)
            {
                if (GameState.instance.playerInventory.InventoryItemList[i].inventoryItem.itemName == recipe.ingredients[x].ingredient.itemName)
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
            // once we have checked to see if we have all the ingredients in the players inventory, show the player they can craft it
            canCraft = true;
            weaponLockedImage.enabled = false;
        }
    }

    public void ShowCraftingSummary()
    {
        // when this weapon is clicked in the crafting menu, show all thd details related to it
    }

    public void CraftWeapon()
    {
        // take the necessary items out of the players inventory and add this weapon to their inventory
    }
}
