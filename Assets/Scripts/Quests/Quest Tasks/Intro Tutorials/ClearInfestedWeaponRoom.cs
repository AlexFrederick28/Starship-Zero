using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class ClearInfestedWeaponRoom : QuestTaskBase
{
    private bool subscribedToCompletionEvent = false;

    public override void OnEnable()
    {
        if (Spawning.instance != null)
        {
            Spawning.instance.OnCompletingInfestedRoom += CompletedInfestedRoom;
        }
    }

    public override void OnDisable()
    {
        if (Spawning.instance != null)
        {
            Spawning.instance.OnCompletingInfestedRoom -= CompletedInfestedRoom;
        }
    }

    public override void Update()
    {
        base.Update();
        if (Spawning.instance != null && subscribedToCompletionEvent == false)
        {
            Spawning.instance.OnCompletingInfestedRoom += CompletedInfestedRoom;
            subscribedToCompletionEvent = true;
        }
    }

    public void CompletedInfestedRoom()
    {
        if (Spawning.instance == null) { return; }
        if (Spawning.instance.questID == questInfos[0].quest.prerequisite.id)
        {
            if (Spawning.instance.playerClearedRoom == true)
            {
                RegisterQuestInteraction();
                gameObject.SetActive(false);
            }
        }
    }
}
