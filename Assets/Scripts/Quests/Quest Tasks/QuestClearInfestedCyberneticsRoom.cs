using UnityEngine;

public class QuestClearInfestedCyberneticsRoom : QuestClearInfestedRoom
{
    public override void CompletedInfestedRoom(Room room)
    {
        base.CompletedInfestedRoom(room);
        if (Spawning.instance.parentRoom.questID != questInfos[0].quest.prerequisite.id) { return; }

        if (Spawning.instance.playerClearedRoom == true)
        {
            UIManager.instance.cyberneticUpgradeLock.enabled = false;
            UIManager.instance.cardUpgradeLock.enabled = false;
            GameState.instance.cyberneticUpgradesUnlocked = true;
        }
    }
}
