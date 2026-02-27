using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class WeaponCraftingMachine : MonoBehaviour, IInteractable
{
    [Tooltip("Unlocks the crafting machine after the quest is complete")]
    public int questID;
    private bool craftingEnabled = false;
    public TextMeshPro itemText;

    private void Start()
    {
        itemText.enabled = false;
    }

    private void OnEnable()
    {
        NPCBase.OnHandedInQuest += EnableQuestObject;
    }

    private void OnDisable()
    {
        NPCBase.OnHandedInQuest -= EnableQuestObject;
    }

    public void DisableInteractionComponent()
    {
        craftingEnabled = false;
        itemText.enabled = false;
    }

    public void EnableInteractionComponent()
    {
        craftingEnabled = true;
        itemText.enabled = true;
    }

    public void EnableQuestObject()
    {
        // TODO: quest object is not being activated
        Debug.Log("Enabling quest object");
        if (QuestManager.instance.activeQuests.Any(q => q.prerequisite.id == questID))
        {
            Debug.Log("Found quest with same ID and activating quest object");
            EnableInteractionComponent();
        }
    }

    public void OnEndInteraction()
    {
        // close upgrade UI when walking away
        if (UIManager.instance.weaponCraftingUIParent.activeSelf == true)
        {
            UIManager.instance.weaponCraftingUIParent.SetActive(false);
            GameState.instance.ChangeToPreviousState();
        }
    }

    public void OnInteract()
    {
        // open upgrade UI
        if (craftingEnabled == false) { return; }
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
