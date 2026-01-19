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
        if (Spawning.instance != null && Spawning.instance.CurrentTime >= Spawning.instance.TimerLength)
        {
            // switch animation
            RegisterQuestInteraction();
        }
    }
}
