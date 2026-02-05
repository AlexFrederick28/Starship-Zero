using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    // must be changed to a generic type if there will be weapons etc in the inventory not just specimens
    public InventoryItem specimenType;
    public Image childImage;
    public Image slotImage;
    public Color originalColour;
    public Color highlightedColour;
    private bool selectedSlot = false;
    private bool viewingSlot = false;

    private void OnEnable()
    {
        GameState.instance.playerInventory.OnClearingMultiSelectedSlotsFromList += DeselectSlot;

        if (specimenType == null)
        {
            specimenType = new InventoryItem();
            specimenType.name = string.Empty;   
        }
        if (specimenType.name != string.Empty)
        {
            childImage.enabled = true;
            childImage.sprite = specimenType.sprite;
        }
        else
        {
            childImage.enabled = false;
        }
    }

    private void OnDisable()
    {
        GameState.instance.playerInventory.OnClearingMultiSelectedSlotsFromList += DeselectSlot;

        DeselectSlot();
    }

    private void Update()
    {
        // maybe change this to an event handled by the inventory instead of update
        if (viewingSlot == true && GameState.instance.playerInventory.selectedSlot != this)
        {
            slotImage.color = originalColour;
            viewingSlot = false;
        }
    }

    public void RenewObject()
    {
        specimenType = new InventoryItem();
        specimenType.name = string.Empty;
    }

    public void RefreshSlot()
    {
        if (specimenType == null)
        {
            RenewObject();
        }
        if (specimenType.name == string.Empty)
        {
            // if there is essentially no object
            childImage.enabled = false;
        }
        else
        {
            childImage.sprite = specimenType.sprite;
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
        else if (specimenType.name != string.Empty && viewingSlot == false)
        {
            UIManager.instance.infoName.text = specimenType.name;
            UIManager.instance.infoImage.sprite = specimenType.sprite;
            UIManager.instance.infoText.text = specimenType.description;
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
        GameState.instance.player.AddCurrency(specimenType.sellAmount);
        specimenType = null;
        childImage.sprite = null;
        childImage.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (specimenType != null)
            {
                // Tell the manager this slot was clicked
                GameState.instance.playerInventory.SelectItem(this);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right && selectedSlot == false && specimenType.name != string.Empty)
        {
            MultiSelectSlot();
        }
        else if (eventData.button == PointerEventData.InputButton.Right && selectedSlot == true)
        {
            DeselectSlot();
        }
    }
}
