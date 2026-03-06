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
}
