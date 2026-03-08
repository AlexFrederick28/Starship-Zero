using UnityEngine;

public class SpecificWeaponFireRateCard : CardBase
{
    public override void AddStatUpgrade()
    {
        base.AddStatUpgrade();

        float fireRateIncrease = ((float)totalStatAmount / 100) * LevelUpManager.instance.chosenWeapon.fireRate;
        LevelUpManager.instance.chosenWeapon.fireRate += fireRateIncrease;
        LevelUpManager.instance.chosenWeapon.weaponLevel++;

        Debug.Log("Applied: " + fireRateIncrease + "from Fire Rate Card!");
    }
}
