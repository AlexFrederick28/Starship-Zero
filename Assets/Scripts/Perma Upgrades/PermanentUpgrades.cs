using UnityEngine;

public class PermanentUpgrades : QuestObjectBase, IInteractable
{
    public override void OnEndInteraction()
    {
        base.OnEndInteraction();
        if (UIManager.instance.upgradeParent.activeSelf == true)
        {
            UIManager.instance.weaponCraftingUIParent.SetActive(false);
            GameState.instance.ChangeToPreviousState();
        }
    }

    public override void OnInteract()
    {
        if (behaviourEnabled == false) { return; }

        base.OnInteract();
        if (UIManager.instance.upgradeParent.activeSelf == true)
        {
            UIManager.instance.upgradeParent.SetActive(false);
            GameState.instance.ChangeToPreviousState();
        }
        else if (UIManager.instance.upgradeParent.activeSelf == false && GameState.instance.currentState != GameState.States.OpenUI)
        {
            UIManager.instance.upgradeParent.SetActive(true);
            GameState.instance.ChangeStateToOpenUI();
        }
    }
}
