using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PowerGridSwitch : QuestTaskBase, IInteractable
{
    public void DisableInteractionComponent()
    {
        return;
    }

    public void EnableInteractionComponent()
    {
        return;
    }

    public void OnEndInteraction()
    {
        
    }

    public void OnInteract()
    {
        // switch animation
        if (this.enabled == true)
        {
            RegisterQuestInteraction();
        }
    }
}
