using UnityEngine;

public class LifeStealCard : CardBase
{
    public override void AddStatUpgrade()
    {
        base.AddStatUpgrade();

        //LevelUpManager.instance.chosenWeapon.critChance += LevelUpManager.instance.chosenCard.statUpgradeAmount;
        LevelUpManager.instance.chosenCard.cardInfo.cardLevel++;

        Debug.Log("Applied: " + LevelUpManager.instance.chosenCard.statUpgradeAmount.ToString() + "from life steal Card!");
    }

    public override void CustomOnHitEvent(float damageDealt, EnemyBase enemy)
    {
        if (totalStatAmount == 0) { return; }
        base.CustomOnHitEvent(damageDealt, enemy);

        float healthToAdd = ((float)totalStatAmount/100) * damageDealt;
        Debug.Log("Health to add = " + healthToAdd + " From stat amount: " + totalStatAmount + " Damage Dealt: " + damageDealt);

        GameState.instance.player.AddHealth((int)healthToAdd);
    }
}
