using System;
using Unity.VisualScripting;
using UnityEngine;

public class CardUnlockUpgradeMenu : MonoBehaviour, IInteractable
{
    public CardSlot selectedCardSlot;
    public bool unlocksEnabled = false; // blocks the menu popping up (needed if the player is in an infested clear and shouldnt be able to open it)

    public static Action<CardSlot> OnViewingCard;

    public static CardUnlockUpgradeMenu instance;

    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDisable()
    {
        
    }

    public void DisableInteractionComponent()
    {
        unlocksEnabled = false;
    }

    public void EnableInteractionComponent()
    {
        unlocksEnabled = true;
    }

    public void OnEndInteraction()
    {
        throw new System.NotImplementedException();
    }

    public void OnInteract()
    {
        if (unlocksEnabled == false) { return; }


    }
}
