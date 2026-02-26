using TMPro;
using UnityEngine;

public class PickupStarterWeaponTutorial : QuestTaskBase, IInteractable
{
    [SerializeField] private TextMeshPro gunText;
    [SerializeField] private Collider2D pickupCollider;
    [SerializeField] private InventoryItemPackage weapon;

    public override void Start()
    {
        base.Start();

        gunText.enabled = false;
        pickupCollider.enabled = false;
        //Debug.Log("Disabled gun");
    }

    public override void OnQuestUpdate()
    {
        base.OnQuestUpdate();

        if (gunText.enabled == false)
        {
            pickupCollider.enabled = true;
            gunText.enabled = true;
        }
    }

    public void PickupWeapon()
    {
        GameState.instance.playerInventory.AddItemToInventory(weapon);
    }

    public void OnInteract()
    {
        if (gunText.enabled == false) { return; }
        PickupWeapon();
        RegisterQuestInteraction();
        gameObject.SetActive(false);
    }

    public void OnEndInteraction()
    {
        return;
    }
}
