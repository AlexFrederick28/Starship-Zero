using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PowerGridSwitch : QuestTaskBase, IInteractable
{
    public void OnEndInteraction()
    {
        
    }

    public void OnInteract()
    {
        // switch animation
        RegisterQuestInteraction();
    }
}
