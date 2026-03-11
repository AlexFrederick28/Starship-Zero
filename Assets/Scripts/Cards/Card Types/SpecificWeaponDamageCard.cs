using UnityEngine;

public class SpecificWeaponDamageCard : CardBase
{
    public override void AddStatUpgrade()
    {
        base.AddStatUpgrade();

        float damageIncrease = ((float)statUpgradeAmount / 100) * LevelUpManager.instance.chosenWeapon.damage;
        LevelUpManager.instance.chosenWeapon.damage += (int)damageIncrease;
        LevelUpManager.instance.chosenWeapon.weaponLevel++;

        Debug.Log("Applied: " + damageIncrease + " from Damage Card!");
    }
}
