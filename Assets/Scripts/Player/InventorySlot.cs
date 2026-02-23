using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    // must be changed to a generic type if there will be weapons etc in the inventory not just specimens
    public int currentStackSize;
    public int amountFromStackToSell;
    public int slotPosition;
    public InventoryItemPackage inventoryItem;
    public Image childImage;
    public Image slotImage;
    public TextMeshProUGUI stackNumberText;
    public Color originalColour;
    public Color highlightedColour;
    public bool selectedSlot = false;
    public bool viewingSlot = false;
    public bool weaponEquipped = false;

    private void OnEnable()
    {
        if (transform.parent != UIManager.instance.inventorySlotContentParent.transform) { return; }
        GameState.instance.playerInventory.OnClearingMultiSelectedSlotsFromList += DeselectSlot;
        RefreshSlot();

        if (inventoryItem != null)
        {
            childImage.enabled = true;
            childImage.sprite = inventoryItem.sprite;
        }
        else
        {
            childImage.enabled = false;
        }
    }

    private void OnDisable()
    {
        if (transform.parent != UIManager.instance.inventorySlotContentParent.transform) { return; }
        GameState.instance.playerInventory.OnClearingMultiSelectedSlotsFromList -= DeselectSlot;

        if (viewingSlot == true)
        {
            // deselects a viewed object
            SelectSlot();
        }
        // deselects multi selected objects
        DeselectSlot();
    }

    private void Update()
    {
        // maybe change this to an event handled by the inventory instead of update
        if (viewingSlot == true && GameState.instance.playerInventory.selectedSlot != this)
        {
            // if another slot has been selected after this one, deselect it (no longer viewing this slot)
            slotImage.color = originalColour;
            viewingSlot = false;
        }
        if (viewingSlot == true)
        {
            UIManager.instance.selectedStackAmount.text = UIManager.instance.stackAmountSlider.value.ToString();
            amountFromStackToSell = (int)UIManager.instance.stackAmountSlider.value;
        }
        else
        {
            amountFromStackToSell = 0;
        }
    }

    public void RenewObject()
    {
        inventoryItem = null;
        currentStackSize = 0;
        childImage.sprite = null;
        childImage.enabled = false;
    }

    public void RefreshSlot()
    {
        stackNumberText.text = currentStackSize.ToString();
        if (inventoryItem == null)
        {
            // if there is essentially no object
            childImage.enabled = false;
            stackNumberText.text = string.Empty;
        }
        else
        {
            childImage.sprite = inventoryItem.sprite;
            childImage.enabled = true;
        }
    }

    public void SelectSlot()
    {
        if (viewingSlot == true)
        {
            slotImage.color = originalColour;
            viewingSlot = false;
            GameState.instance.playerInventory.HideSelectedItem(this);
        }
        else if (inventoryItem != null && viewingSlot == false)
        {
            if (selectedSlot == true) { DeselectSlot(); } // deselect the slot if it was multi selected
            GameState.instance.playerInventory.ShowSelectedItem(this);
        }
    }

    public void MultiSelectSlot()
    {
        // selecting and deselct parameters are handled by the OnClickEvent
        if (viewingSlot == true) { SelectSlot(); } // deslect the viewed slot 
        GameState.instance.playerInventory.multiSelectedSlots.Add(this);
        slotImage.color = highlightedColour;
        selectedSlot = true;
    }

    public void DeselectSlot()
    {
        // selecting and deselct parameters are handled by the OnClickEvent
        GameState.instance.playerInventory.multiSelectedSlots.Remove(this);
        slotImage.color = originalColour;
        selectedSlot = false;
    }

    public void SellItem()
    {
        if (selectedSlot == true)
        {
            GameState.instance.player.AddCurrency(inventoryItem.sellAmount * currentStackSize);
            currentStackSize = 0;
        }
        else if (viewingSlot == true && amountFromStackToSell > 0)
        {
            GameState.instance.player.AddCurrency(inventoryItem.sellAmount * amountFromStackToSell);
            currentStackSize -= amountFromStackToSell;
            SelectSlot(); // turns the viewed slot off
        }
        stackNumberText.text = currentStackSize.ToString();
        if (currentStackSize <= 0)
        {
            // if there are no items in the stack, reset the slot 
            if (viewingSlot == true)
            {
                SelectSlot();
            }
            RenewObject();
            RefreshSlot();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (inventoryItem != null)
            {
                // Tell the manager this slot was clicked
                GameState.instance.playerInventory.SelectItem(this);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right && selectedSlot == false && inventoryItem != null)
        {
            MultiSelectSlot();
        }
        else if (eventData.button == PointerEventData.InputButton.Right && selectedSlot == true)
        {
            DeselectSlot();
        }
    }
}
