using UnityEngine;

public class SpecificWeaponCritChanceCard : CardBase
{
    public override void AddStatUpgrade()
    {
        base.AddStatUpgrade();

        LevelUpManager.instance.chosenWeapon.critChance += LevelUpManager.instance.chosenCard.statUpgradeAmount;
        LevelUpManager.instance.chosenWeapon.weaponLevel++;

        Debug.Log("Applied: " + LevelUpManager.instance.chosenCard.statUpgradeAmount.ToString() + "from crit chance Card!");
    }
}
