using UnityEngine;

public class WeaponCraftingMachine : MonoBehaviour, IInteractable
{
    public void OnEndInteraction()
    {
        return;
    }

    public void OnInteract()
    {
        // open upgrade UI
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
