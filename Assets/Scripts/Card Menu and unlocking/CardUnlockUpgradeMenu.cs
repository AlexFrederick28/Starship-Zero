using System;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;

public class CardUnlockUpgradeMenu : QuestObjectBase, IInteractable
{
    public CardSlot selectedCardSlot;

    public static Action<CardSlot> OnViewingCard;

    public static CardUnlockUpgradeMenu instance;

    public override void OnEnable()
    {
        base.OnEnable();

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    public override void OnEndInteraction()
    {
        base.OnEndInteraction();
        if (UIManager.instance.cardUnlockParent.activeSelf == true)
        {
            UIManager.instance.cardUnlockParent.SetActive(false);
            GameState.instance.ChangeToPreviousState();
        }
    }

    public override void OnInteract()
    {
        if (behaviourEnabled == false) { return; }

        base.OnInteract();
        if (UIManager.instance.cardUnlockParent.activeSelf == true)
        {
            UIManager.instance.cardUnlockParent.SetActive(false);
            GameState.instance.ChangeToPreviousState();
        }
        else if (UIManager.instance.cardUnlockParent.activeSelf == false && GameState.instance.currentState != GameState.States.OpenUI)
        {
            UIManager.instance.cardUnlockParent.SetActive(true);
            GameState.instance.ChangeStateToOpenUI();
        }
    }
}
