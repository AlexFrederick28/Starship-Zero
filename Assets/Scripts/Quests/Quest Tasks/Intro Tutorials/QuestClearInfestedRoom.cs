using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class QuestClearInfestedRoom : QuestTaskBase
{
    private bool subscribedToCompletionEvent = false;

    public override void OnEnable()
    {
        if (Spawning.instance != null)
        {
            GameState.instance.OnCompletedInfestedClear += CompletedInfestedRoom;
        }
    }

    public override void OnDisable()
    {
        if (Spawning.instance != null)
        {
            GameState.instance.OnCompletedInfestedClear -= CompletedInfestedRoom;
        }
    }

    public override void Update()
    {
        base.Update();
        if (Spawning.instance != null && subscribedToCompletionEvent == false)
        {
            GameState.instance.OnCompletedInfestedClear += CompletedInfestedRoom;
            subscribedToCompletionEvent = true;
        }
    }

    public virtual void CompletedInfestedRoom(Room room)
    {
        if (Spawning.instance == null) { return; }
        if (Spawning.instance.parentRoom.questID != questInfos[0].quest.prerequisite.id) { return; }

        if (Spawning.instance.playerClearedRoom == true)
        {
            RegisterQuestInteraction();
            gameObject.SetActive(false);
        }
    }
}
