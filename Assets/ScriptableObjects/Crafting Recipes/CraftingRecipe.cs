using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Scriptable Objects/CraftingRecipe")]
public class CraftingRecipe : ScriptableObject
{
    [Serializable]
    public class Ingredient
    {
        public int amount;
        public InventoryItemPackage ingredient;
    }

    public int unlockCost;
    public int purchaseCost;
    public Ingredient[] ingredients;
}
