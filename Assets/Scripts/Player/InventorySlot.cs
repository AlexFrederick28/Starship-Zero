using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    // must be changed to a generic type if there will be weapons etc in the inventory not just specimens
    public SpecimenType specimenType;
    public Image childImage;

    private void OnEnable()
    {
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

    public void SelectSlot()
    {
        Debug.Log("Selected slot + " + transform.name);
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
    }
}
