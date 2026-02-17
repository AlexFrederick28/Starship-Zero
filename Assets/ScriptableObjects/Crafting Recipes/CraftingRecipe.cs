using System;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Scriptable Objects/CraftingRecipe")]
public class CraftingRecipe : ScriptableObject
{
    [Serializable]
    public class Ingredient
    {
        public int amount;
        public InventoryItemPackage ingredient;
        public Sprite weaponCraftingRecipeSprite;

    }

    public int unlockCost;
    public int purchaseCost;
    public string weaponCraftingRecipeDescription;
    public WeaponScriptableObject weapon;
    public Ingredient[] ingredients;
}
