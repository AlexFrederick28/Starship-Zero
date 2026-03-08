using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class WeaponCraftingMachine : QuestObjectBase, IInteractable
{
    public override void OnEndInteraction()
    {
        // close upgrade UI when walking away
        base.OnEndInteraction();
        if (UIManager.instance.weaponCraftingUIParent.activeSelf == true)
        {
            UIManager.instance.weaponCraftingUIParent.SetActive(false);
            GameState.instance.ChangeToPreviousState();
        }
    }

    public override void OnInteract()
    {
        // open upgrade UI
        if (behaviourEnabled == false) { return; }

        base.OnInteract();
        if (UIManager.instance.weaponCraftingUIParent.activeSelf == true)
        {
            UIManager.instance.weaponCraftingUIParent.SetActive(false);
            GameState.instance.ChangeToPreviousState();
        }
        else if (UIManager.instance.weaponCraftingUIParent.activeSelf == false && GameState.instance.currentState != GameState.States.OpenUI)
        {
            UIManager.instance.weaponCraftingUIParent.SetActive(true);
            GameState.instance.ChangeStateToOpenUI();
        }
    }
}
