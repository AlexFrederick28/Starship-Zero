using UnityEngine;

public class UpgradeFireRate : UpgradeBase
{
    public override void UpgradeCard()
    {
        if (GameState.instance.player.currency < currentStatIncreaseAndCost.y) { return; }

        base.UpgradeCard();

        //float fireRateIncrease = (currentStatIncreaseAndCost.x / 100) * GameState.instance.player.fireRate;
        float fireRateIncrease = currentStatIncreaseAndCost.x;
        if ((int)fireRateIncrease < 1)
        {
            fireRateIncrease = 1f;
        }

        GameState.instance.player.fireRate += (int)fireRateIncrease;
    }

    public override void RefundCard()
    {
        if (previousStatIncreaseAndCost.Count < 1) { return; }

        base.RefundCard();

        //float fireRateDecrease = (currentStatIncreaseAndCost.x / 100) * GameState.instance.player.fireRate;
        float fireRateDecrease = currentStatIncreaseAndCost.x;
        if ((int)fireRateDecrease < 1)
        {
            fireRateDecrease = 1f;
        }

        GameState.instance.player.fireRate -= fireRateDecrease;
    }
}
