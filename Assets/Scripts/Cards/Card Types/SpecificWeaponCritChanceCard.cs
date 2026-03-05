using UnityEngine;

public class SpecificWeaponCritChanceCard : CardBase
{
    public override void AddStatUpgrade()
    {
        base.AddStatUpgrade();

        LevelUpManager.instance.chosenWeapon.critChance += LevelUpManager.instance.chosenCard.statUpgradeAmount;
        LevelUpManager.instance.chosenWeapon.weaponLevel++;

        Debug.Log("Applied stat from Crit Chance Card!");
    }
}
