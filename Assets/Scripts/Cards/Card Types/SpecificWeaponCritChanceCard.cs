using UnityEngine;

public class SpecificWeaponCritChanceCard : CardBase
{
    public override void AddStatUpgrade()
    {
        base.AddStatUpgrade();

        float critChanceIncrease = 0;

        if (LevelUpManager.instance.chosenWeapon.critChance > 0)
        {
            //critChanceIncrease = ((float)totalStatAmount / 100) * LevelUpManager.instance.chosenWeapon.critChance; OLD: used the entire tracked amount rather than the shown amount
            critChanceIncrease = ((float)statUpgradeAmount / 100) * LevelUpManager.instance.chosenWeapon.critChance;

            if (critChanceIncrease < 1f)
            {
                critChanceIncrease = 1f;
            }
        }
        else
        {
            //critChanceIncrease = ((float)totalStatAmount / 100) * 5f; OLD: used the entire tracked amount rather than the shown amount
            critChanceIncrease = ((float)statUpgradeAmount / 100) * 5f;
        }

        LevelUpManager.instance.chosenWeapon.critChance += (int)critChanceIncrease;
        LevelUpManager.instance.chosenWeapon.weaponLevel++;

        Debug.Log("Applied: " + LevelUpManager.instance.chosenCard.statUpgradeAmount.ToString() + "from crit chance Card!");
    }
}
