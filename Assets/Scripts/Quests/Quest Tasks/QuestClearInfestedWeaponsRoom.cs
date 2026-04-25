using UnityEngine;

public class QuestClearInfestedWeaponsRoom : QuestClearInfestedRoom
{
    public override void CompletedInfestedRoom(Room room)
    {
        base.CompletedInfestedRoom(room);
        if (Spawning.instance.parentRoom.questID != questInfos[0].quest.prerequisite.id) { return; }

        if (Spawning.instance.playerClearedRoom == true)
        {
            UIManager.instance.weaponCraftingButtonImage.color = UIManager.instance.originalColor;
            UIManager.instance.weaponCraftingButtonText.SetActive(true);
            UIManager.instance.weaponCraftingButton.enabled = true;
            UIManager.instance.weaponCraftingButtonEventTrigger.enabled = true;
            GameState.instance.weaponCraftingUnlocked = true;
        }
    }
}
