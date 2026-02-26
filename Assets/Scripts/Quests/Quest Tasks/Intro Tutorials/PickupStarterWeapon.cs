using TMPro;
using UnityEngine;

public class PickupStarterWeapon : QuestTaskBase
{
    [SerializeField] private TextMeshPro gunText;
    [SerializeField] private Collider2D pickupCollider;

    public override void Start()
    {
        gunText.enabled = false;
        pickupCollider.enabled = false;
    }
}
