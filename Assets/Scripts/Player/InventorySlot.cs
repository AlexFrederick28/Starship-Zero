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
    public InventoryItem inventoryItem;
    public Image childImage;
    public Image slotImage;
    public TextMeshProUGUI stackNumberText;
    public Color originalColour;
    public Color highlightedColour;
    public bool selectedSlot = false;
    public bool viewingSlot = false;

    private void OnEnable()
    {
        GameState.instance.playerInventory.OnClearingMultiSelectedSlotsFromList += DeselectSlot;

        if (inventoryItem == null)
        {
            inventoryItem = new InventoryItem();
            inventoryItem.name = string.Empty;
        }
        if (inventoryItem.name != string.Empty)
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
        GameState.instance.playerInventory.OnClearingMultiSelectedSlotsFromList -= DeselectSlot;

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
        inventoryItem = new InventoryItem();
        inventoryItem.name = string.Empty;
        currentStackSize = 0;
        childImage.sprite = null;
        childImage.enabled = false;
    }

    public void RefreshSlot()
    {
        stackNumberText.text = currentStackSize.ToString();
        if (inventoryItem == null)
        {
            RenewObject();
        }
        if (inventoryItem.name == string.Empty)
        {
            // if there is essentially no object
            childImage.enabled = false;
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
        }
        else if (inventoryItem.name != string.Empty && viewingSlot == false)
        {
            UIManager.instance.infoName.text = inventoryItem.name;
            UIManager.instance.infoImage.sprite = inventoryItem.sprite;
            UIManager.instance.infoText.text = inventoryItem.description;
            UIManager.instance.stackAmountSlider.minValue = 0;
            UIManager.instance.stackAmountSlider.maxValue = currentStackSize;
            slotImage.color = Color.white;
            GameState.instance.playerInventory.selectedSlot = this;
            viewingSlot = true;
        }
    }

    public void MultiSelectSlot()
    {
        // selecting and deselct parameters are handled by the OnClickEvent
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
        else if (viewingSlot == true)
        {
            GameState.instance.player.AddCurrency(inventoryItem.sellAmount * amountFromStackToSell);
            currentStackSize -= amountFromStackToSell;
        }
        stackNumberText.text = currentStackSize.ToString();
        if (currentStackSize <= 0)
        {
            // if there are no items in the stack, reset the slot 
            RenewObject();
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
        else if (eventData.button == PointerEventData.InputButton.Right && selectedSlot == false && inventoryItem.name != string.Empty)
        {
            MultiSelectSlot();
        }
        else if (eventData.button == PointerEventData.InputButton.Right && selectedSlot == true)
        {
            DeselectSlot();
        }
    }
}
