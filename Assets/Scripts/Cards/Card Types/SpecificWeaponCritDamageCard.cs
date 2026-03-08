using UnityEngine;

public class SpecificWeaponCritDamageCard : CardBase
{
    public override void AddStatUpgrade()
    {
        base.AddStatUpgrade();

        float critDamageIncrease = 0;

        if (LevelUpManager.instance.chosenWeapon.critDamage > 0)
        {
            critDamageIncrease = ((float)totalStatAmount / 100) * LevelUpManager.instance.chosenWeapon.critDamage;
        }
        else
        {
            critDamageIncrease = ((float)totalStatAmount / 100) * 5f;
        }

        LevelUpManager.instance.chosenWeapon.critDamage += (int)critDamageIncrease;
        LevelUpManager.instance.chosenWeapon.weaponLevel++;

        Debug.Log("Applied: " + critDamageIncrease + "from Crit Damage Card!");
    }
}
