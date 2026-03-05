using UnityEngine;

public class SpecificWeaponFireRateCard : CardBase
{
    public override void AddStatUpgrade()
    {
        base.AddStatUpgrade();

        LevelUpManager.instance.chosenWeapon.fireRate += LevelUpManager.instance.chosenCard.statUpgradeAmount;
        LevelUpManager.instance.chosenWeapon.weaponLevel++;

        Debug.Log("Applied: " + LevelUpManager.instance.chosenCard.statUpgradeAmount.ToString() + "from Fire Rate Card!");
    }
}
