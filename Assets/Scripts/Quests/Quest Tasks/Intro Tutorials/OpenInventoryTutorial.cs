using UnityEngine;

public class OpenInventoryTutorial : QuestTaskBase
{
    public override void OnQuestUpdate()
    {
        base.OnQuestUpdate();

        if (GameState.instance.playerInventory.weaponLoadoutList.Count > 0)
        {
            RegisterQuestInteraction();
            gameObject.SetActive(false);
        }
    }
}
