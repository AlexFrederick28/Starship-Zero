using Unity.VisualScripting;
using UnityEngine;

public class ClearInfestedWeaponRoom : QuestTaskBase
{
    private void FixedUpdate()
    {
        CompletedInfestedRoom();
    }

    public void CompletedInfestedRoom()
    {
        if (Spawning.instance == null) { return; }
        if (Spawning.instance.questID == quest[0].prerequisite.id)
        {
            if (Spawning.instance.playerClearedRoom == true)
            {
                RegisterQuestInteraction();
                gameObject.SetActive(false);
            }
        }
    }
}
