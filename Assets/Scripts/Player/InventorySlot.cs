using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    // must be changed to a generic type if there will be weapons etc in the inventory not just specimens
    public SpecimenType specimenType;
    public Image childImage;
    public Image slotImage;
    public Color originalColour;
    public Color highlightedColour;
    private bool selectedSlot = false;
    private bool viewingSlot = false;

    private void OnEnable()
    {
        if (specimenType == null)
        {
            specimenType = new SpecimenType();
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

    public void SelectSlot()
    {
        if (viewingSlot == true)
        {
            slotImage.color = originalColour;
            viewingSlot = false;
        }
        else
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
        GameState.instance.playerInventory.multiSelectedSlots.Add(this);
        slotImage.color = highlightedColour;
        selectedSlot = true;
    }

    public void DeselectSlot()
    {
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
