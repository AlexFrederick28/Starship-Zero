using UnityEngine;

public class UpgradeCritDamage : UpgradeBase
{
    public override void UpgradeCard()
    {
        if (GameState.instance.player.currency < currentStatIncreaseAndCost.y) { return; }

        base.UpgradeCard();

        float critDamageIncrease = (currentStatIncreaseAndCost.x / 100) * GameState.instance.player.critDamage;
        if ((int)critDamageIncrease < 1)
        {
            critDamageIncrease = 1f;
        }

        GameState.instance.player.critDamage += (int)critDamageIncrease;
    }

    public override void RefundCard()
    {
        if (previousStatIncreaseAndCost.Count < 1) { return; }

        base.RefundCard();

        float critDamageDecrease = (currentStatIncreaseAndCost.x / 100) * GameState.instance.player.critDamage;
        if ((int)critDamageDecrease < 1)
        {
            critDamageDecrease = 1f;
        }

        GameState.instance.player.critDamage -= critDamageDecrease;
    }
}
