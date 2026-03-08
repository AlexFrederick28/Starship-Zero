using System.Linq;
using TMPro;
using UnityEngine;

public class QuestObjectBase : MonoBehaviour, IInteractable
{
    [Tooltip("Unlocks the crafting machine after the quest is complete")]
    public int questID;
    public bool behaviourEnabled = false;
    public TextMeshPro itemText;
    public bool doesOpenUI = false;

    public virtual void Start()
    {
        itemText.enabled = false;
    }

    public virtual void OnEnable()
    {
        NPCBase.OnHandedInQuest += EnableQuestObject;
    }

    public virtual void OnDisable()
    {
        NPCBase.OnHandedInQuest -= EnableQuestObject;
    }

    public virtual void DisableInteractionComponent()
    {
        behaviourEnabled = false;
        itemText.enabled = false;
    }

    public virtual void EnableInteractionComponent()
    {
        behaviourEnabled = true;
        itemText.enabled = true;
    }

    public virtual void EnableQuestObject()
    {
        Debug.Log("Enabling quest object");
        if (QuestManager.instance.activeQuests.Any(q => q.prerequisite.id == questID))
        {
            Debug.Log("Found quest with same ID and activating quest object");
            EnableInteractionComponent();
        }
    }

    public virtual void OnInteract()
    {
        if (doesOpenUI == true)
        {
            if (GameState.instance.currentState == GameState.States.OpenUI)
            {
                UIManager.instance.OnClosedUI?.Invoke();
            }
            else
            {
                UIManager.instance.OnOpenedUI?.Invoke();
            }
        }
    }

    public virtual void OnEndInteraction()
    {
        if (doesOpenUI == true)
        {
            UIManager.instance.OnClosedUI?.Invoke();
        }
    }
}
