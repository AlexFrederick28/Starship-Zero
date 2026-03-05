using UnityEngine;

public class SpecificWeaponCritDamageCard : CardBase
{
    public override void AddStatUpgrade()
    {
        base.AddStatUpgrade();

        LevelUpManager.instance.chosenWeapon.critDamage += LevelUpManager.instance.chosenCard.statUpgradeAmount;
        LevelUpManager.instance.chosenWeapon.weaponLevel++;

        Debug.Log("Applied: " + LevelUpManager.instance.chosenCard.statUpgradeAmount.ToString() + "from Crit Damage Card!");
    }
}
