using UnityEngine;

public class DropChanceCard : CardBase
{
    public override void AddStatUpgrade()
    {
        base.AddStatUpgrade();

        LevelUpManager.instance.chosenCard.cardInfo.cardLevel++;

        Debug.Log("Applied: " + LevelUpManager.instance.chosenCard.statUpgradeAmount.ToString() + "from drop chance Card!");
    }

    public override void CustomOnHitEvent(float damageDealt, EnemyBase enemy)
    {
        if (totalStatAmount == 0) { return; }
        base.CustomOnHitEvent(damageDealt, enemy);

        float increaseAmount = (totalStatAmount/100) * (float)enemy.dropFrequencyPercentChance;
        Debug.Log("Drop chance will equal = " + increaseAmount + " From stat amount: " + totalStatAmount + " Damage Dealt: " + damageDealt);

        enemy.IncreaseDropChance((int)increaseAmount);
        GameState.instance.player.AddHealth(totalStatAmount);
    }
}
