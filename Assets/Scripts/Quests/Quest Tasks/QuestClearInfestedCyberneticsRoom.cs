using UnityEngine;

public class QuestClearInfestedCyberneticsRoom : QuestClearInfestedRoom
{
    public override void CompletedInfestedRoom(Room room)
    {
        base.CompletedInfestedRoom(room);
        if (Spawning.instance.parentRoom.questID != questInfos[0].quest.prerequisite.id) { return; }

        if (Spawning.instance.playerClearedRoom == true)
        {
            UIManager.instance.cyberneticUpgradeButtonImage.color = UIManager.instance.originalColor; ;
            UIManager.instance.cyberneticUpgradeButtonText.SetActive(true); ;
            UIManager.instance.cyberneticUpgradeButton.enabled = true;
            UIManager.instance.cyberneticUpgradeButtonEventTrigger.enabled = true;
            UIManager.instance.cardUpgradeButtonImage.color = UIManager.instance.originalColor;
            UIManager.instance.cardUpgradeButtonText.SetActive(true);
            UIManager.instance.cardUpgradeButtonButton.enabled = true;
            UIManager.instance.cardUpgradeButtonEventTrigger.enabled = true;
            GameState.instance.cyberneticUpgradesUnlocked = true;
        }
    }
}
