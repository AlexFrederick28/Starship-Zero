using UnityEngine;

public class UpgradeMaxHealth : UpgradeBase
{
    public override void UpgradeCard()
    {
        if (GameState.instance.player.currency < currentStatIncreaseAndCost.y) { return; }

        base.UpgradeCard();

        float healthIncrease = (currentStatIncreaseAndCost.x / 100) * GameState.instance.player.maxHealth;

        if ((int)healthIncrease < 1)
        {
            healthIncrease = 1;
        }

        GameState.instance.player.maxHealth += (int)healthIncrease;
        GameState.instance.player.AddHealth((int)GameState.instance.player.maxHealth);
    }

    public override void RefundCard()
    {
        if (previousStatIncreaseAndCost.Count < 1) { return; }

        base.RefundCard();

        float healthDecrease = (currentStatIncreaseAndCost.x / 100) * GameState.instance.player.maxHealth;

        if ((int)healthDecrease < 1)
        {
            healthDecrease = 1;
        }

        GameState.instance.player.maxHealth -= (int)healthDecrease;
        GameState.instance.player.AddHealth((int)GameState.instance.player.maxHealth);
    }
}
