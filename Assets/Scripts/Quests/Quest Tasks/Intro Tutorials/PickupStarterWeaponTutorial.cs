using TMPro;
using UnityEngine;

public class PickupStarterWeaponTutorial : QuestTaskBase, IInteractable
{
    [SerializeField] private TextMeshPro gunText;
    [SerializeField] private Collider2D pickupCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private InventoryItemPackage weapon;
    private bool pickedUpWeapon = false;

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

        if (gunText.enabled == false && pickedUpWeapon == false)
        {
            pickupCollider.enabled = true;
            gunText.enabled = true;
        }

        if (GameState.instance.playerInventory.weaponLoadoutList.Count > 0)
        {
            RegisterQuestInteraction();
            Debug.Log("Quest complete");
            gameObject.SetActive(false);
        }
    }

    public void PickupWeapon()
    {
        GameState.instance.playerInventory.AddWeaponToInventory(weapon);
    }

    public void OnInteract()
    {
        if (gunText.enabled == false) { return; }
        PickupWeapon();
        pickupCollider.enabled = false;
        spriteRenderer.enabled = false;
        gunText.enabled = false;
        pickedUpWeapon = true;
    }

    public void OnEndInteraction()
    {
        return;
    }

    public void DisableInteractionComponent()
    {
        return;
    }

    public void EnableInteractionComponent()
    {
        return;
    }
}
