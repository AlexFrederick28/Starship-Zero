using UnityEngine;

public class UpgradeDamage : UpgradeBase
{
    public override void UpgradeCard()
    {
        if (GameState.instance.player.currency < currentStatIncreaseAndCost.y) { return; }

        base.UpgradeCard();

        //float damageIncrease = (currentStatIncreaseAndCost.x / 100) * GameState.instance.player.damage;
        float damageIncrease = currentStatIncreaseAndCost.x;
        if ((int)damageIncrease < 1)
        {
            damageIncrease = 1f;
        }

        GameState.instance.player.damage += (int)damageIncrease;
    }

    public override void RefundCard()
    {
        if (previousStatIncreaseAndCost.Count < 1) { return; }

        base.RefundCard();

        //float damageDecrease = (currentStatIncreaseAndCost.x / 100) * GameState.instance.player.damage;
        float damageDecrease = currentStatIncreaseAndCost.x;
        if ((int)damageDecrease < 1)
        {
            damageDecrease = 1f;
        }

        GameState.instance.player.damage -= damageDecrease;
    }
}
