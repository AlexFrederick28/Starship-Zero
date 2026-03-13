using UnityEngine;

public class CraftFirstWeaponTutorial : QuestTaskBase
{
    public override void OnEnable()
    {
        base.OnEnable();

        CraftingBase.OnFirstWeaponCraft += CraftedFirstWeapon;
    }

    public override void OnDisable()
    {
        base.OnDisable();

        CraftingBase.OnFirstWeaponCraft -= CraftedFirstWeapon;
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
