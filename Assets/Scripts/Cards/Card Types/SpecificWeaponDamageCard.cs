using UnityEngine;

public class SpecificWeaponDamageCard : CardBase
{
    public override void AddStatUpgrade()
    {
        base.AddStatUpgrade();

        LevelUpManager.instance.chosenWeapon.damage += LevelUpManager.instance.chosenCard.statUpgradeAmount;
        LevelUpManager.instance.chosenWeapon.weaponLevel++;

        Debug.Log("Applied: " + LevelUpManager.instance.chosenCard.statUpgradeAmount.ToString() + "from Damage Card!");
    }
}
