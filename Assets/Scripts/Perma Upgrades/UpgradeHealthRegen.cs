using UnityEngine;

public class UpgradeHealthRegen : UpgradeBase
{
    public override void UpgradeCard()
    {
        if (GameState.instance.player.currency < currentStatIncreaseAndCost.y) { return; }

        base.UpgradeCard();

        //float regenIncrease = (currentStatIncreaseAndCost.x / 100) * GameState.instance.player.maxHealth;
        float regenIncrease = currentStatIncreaseAndCost.x;
        if ((int)regenIncrease < 1)
        {
            regenIncrease = 1f;
        }

        GameState.instance.player.healthRegenAmount += (int)regenIncrease;
    }

    public override void RefundCard()
    {
        if (previousStatIncreaseAndCost.Count < 1) { return; }

        base.RefundCard();

        //float regenDecrease = (currentStatIncreaseAndCost.x / 100) * GameState.instance.player.maxHealth;
        float regenDecrease = currentStatIncreaseAndCost.x;
        if ((int)regenDecrease < 1)
        {
            regenDecrease = 1f;
        }

        GameState.instance.player.healthRegenAmount -= regenDecrease;
    }
}
