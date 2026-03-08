using UnityEngine;

public class ExpGainCard : CardBase
{
    public int initialAdditiveExp;

    public override void AddStatUpgrade()
    {
        base.AddStatUpgrade();

        LevelUpManager.instance.chosenCard.cardInfo.cardLevel++;

        Debug.Log("Applied: " + LevelUpManager.instance.chosenCard.statUpgradeAmount.ToString() + "from life steal Card!");
    }

    public override void CustomOnHitEvent(float damageDealt, EnemyBase enemy)
    {
        if (totalStatAmount == 0) { return; }
        base.CustomOnHitEvent(damageDealt, enemy);

        float expToAdd = ((float)totalStatAmount / 100) * enemy.experienceAdditive;
        if (expToAdd < 1)
        {
            expToAdd = initialAdditiveExp;
        }

        Debug.Log("Exp to add = " + (int)expToAdd + " From stat amount: " + totalStatAmount + " Damage Dealt: " + damageDealt);

        enemy.experienceAdditive = (int)expToAdd;
    }
}
