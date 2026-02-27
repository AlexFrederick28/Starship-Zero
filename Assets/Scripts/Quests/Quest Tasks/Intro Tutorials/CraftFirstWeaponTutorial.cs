using UnityEngine;

public class CraftFirstWeaponTutorial : QuestTaskBase
{
    public override void OnEnable()
    {
        base.OnEnable();

        WeaponCrafting.OnFirstWeaponCraft += CraftedFirstWeapon;
    }

    public override void OnDisable()
    {
        base.OnDisable();

        WeaponCrafting.OnFirstWeaponCraft -= CraftedFirstWeapon;
    }

    public override void OnQuestUpdate()
    {
        base.OnQuestUpdate();
    }

    public void CraftedFirstWeapon()
    {
        RegisterQuestInteraction();
    }
}
