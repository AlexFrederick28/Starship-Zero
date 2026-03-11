using UnityEngine;

public class MovementSpeedCard : CardBase
{
    public override void AddStatUpgrade()
    {
        base.AddStatUpgrade();

        float movementSpeedIncrease = ((float)statUpgradeAmount / 100) * GameState.instance.player.speed;

        GameState.instance.player.speed += movementSpeedIncrease;
        LevelUpManager.instance.chosenCard.cardInfo.cardLevel++;

        Debug.Log("Applied: " + movementSpeedIncrease + "from movement speed Card!");
    }
}
