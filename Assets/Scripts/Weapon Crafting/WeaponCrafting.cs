using UnityEngine;

public class WeaponCrafting : MonoBehaviour
{
    public bool weaponUnlocked = false;
    public bool canCraft = false;
    public CraftingRecipe recipe;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        canCraft = false;
    }

    public void CheckCraftingPossibility()
    {

    }
}
