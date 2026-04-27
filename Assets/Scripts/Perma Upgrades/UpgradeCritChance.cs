using UnityEngine;

public class UpgradeCritChance : UpgradeBase
{
    public override void UpgradeCard()
    {
        if (GameState.instance.player.currency < currentStatIncreaseAndCost.y) { return; }

        base.UpgradeCard();

        //float critChanceIncrease = (currentStatIncreaseAndCost.x / 100) * GameState.instance.player.critChance;
        float critChanceIncrease = currentStatIncreaseAndCost.x;
        if ((int)critChanceIncrease < 1)
        {
            critChanceIncrease = 1f;
        }

        GameState.instance.player.critChance += (int)critChanceIncrease;
    }

    public override void RefundCard()
    {
        if (previousStatIncreaseAndCost.Count < 1) { return; }

        base.RefundCard();

        //float critChanceDecrease = (currentStatIncreaseAndCost.x / 100) * GameState.instance.player.critChance;
        float critChanceDecrease = currentStatIncreaseAndCost.x;
        if ((int)critChanceDecrease < 1)
        {
            critChanceDecrease = 1f;
        }

        GameState.instance.player.critChance -= critChanceDecrease;
    }
}
