using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Inventory : MonoBehaviour
{
    [Serializable]
    public class ItemStack
    {
        public int amount;
        public InventoryItemPackage inventoryItem;
    }

    // need to create seperate list for other objects that are not specimens
    public int maxInventorySlots;
    public InventorySlot selectedSlot;
    public List<InventorySlot> inventorySlots;
    public List<ItemStack> inventoryItemList;
    public List<ItemStack> InventoryItemList
    {
        get { return inventoryItemList; }
        set
        {
            if (inventoryItemList.Count >= maxInventorySlots)
            {
                Debug.Log("Inventory full!");
                return;
            }
        }
    }

    private float collectionTimer;
    public float collectionRadius;
    public float collectionInterval;
    public float collectionSpeed;
    public LayerMask layerMask;
    public GameObject inventoryUI;

    public List<InventorySlot> multiSelectedSlots;
    public Action OnClearingMultiSelectedSlotsFromList;

    private void OnEnable()
    {
        // hiding the image because it is a blank white cube, and nothing is selected so you shouldnt see it anyway
        UIManager.instance.infoInventoryImage.enabled = false;
    }

    private void Start()
    {
        if (UIManager.instance != null)
        {
            SetInventoryOnStart();
            inventoryUI = UIManager.instance.inventoryParent;
            Debug.Log("Added inventory slots");
        }
    }

    private void Update()
    {
        CollectNearbyResource();
    }

    public void SetInventoryOnStart()
    {
        for (int i = 0; i < maxInventorySlots; i++)
        {
            GameObject newSlot = Instantiate(UIManager.instance.slot, UIManager.instance.inventorySlotContentParent.transform);
            newSlot.GetComponent<InventorySlot>().slotPosition = inventorySlots.Count;
            inventorySlots.Add(newSlot.GetComponent<InventorySlot>());
        }
    }

    public void SelectItem(InventorySlot slot)
    {
        if (selectedSlot != null)
        {
            selectedSlot = null;
        }

        selectedSlot = slot;
        selectedSlot.SelectSlot();
    }

    public void SortStacksInInventory()
    {
        // can possible move this function to work off of a button, as selling an amount from a stack but not seeing the stack count go down is a little jarring

        for (int desiredPosition = 0; desiredPosition < inventorySlots.Count; desiredPosition++)
        {
            // loop through the max amount of slots, and for each slot, search for a fillable stack to combine
            for (int i = desiredPosition + 1; i < inventorySlots.Count; i++)
            {
                // nested for loop allows us to look ahead of the chosen stack (desiredPosition) rather than whats behind it - also helps avoid merging backwards
                if (inventorySlots[i].inventoryItem != null && inventorySlots[desiredPosition].inventoryItem != null)
                {
                    // if the current inventory slot (i) is not empty, then it is occupied 
                    if (i != desiredPosition)
                    {
                        if (inventorySlots[desiredPosition].inventoryItem.itemName == inventorySlots[i].inventoryItem.itemName)
                        {
                            // if there is a stack that is not full, add to it and remove it from the secondary stack
                            int spaceLeft = inventorySlots[desiredPosition].inventoryItem.maxStackSize - inventorySlots[desiredPosition].currentStackSize;
                            int amountToMove = 0;
                            // move onto the next desired position if the stack is already full
                            if (spaceLeft <= 0) { break; }
                            if (spaceLeft > 0 && inventorySlots[i].currentStackSize > 0)
                            {
                                // if there is enough space left in the desired position, then add to that stack without exceeding the max stack amount
                                amountToMove = Math.Min(inventorySlots[i].currentStackSize, Math.Max(0, inventorySlots[desiredPosition].inventoryItem.maxStackSize - inventorySlots[desiredPosition].currentStackSize));
                                inventorySlots[desiredPosition].currentStackSize += amountToMove;
                                inventorySlots[i].currentStackSize -= amountToMove;

                                inventorySlots[i].RefreshSlot();
                                inventorySlots[desiredPosition].RefreshSlot();
                                Debug.Log("Filled stacks");
                            }
                            if (inventorySlots[i].currentStackSize <= 0)
                            {
                                inventorySlots[i].RenewObject();
                                inventorySlots[i].RefreshSlot();
                            }
                            Debug.Log("Found item with same name in inventory at position: " + i + " Space Left: " + spaceLeft + " Amount to move: " + amountToMove);
                        }
                    }
                }
            }
        }
    }

    public void RemoveGapsFromInventory()
    {
        int desiredPosition = 0;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            // scan through the inventory
            if (inventorySlots[i].inventoryItem != null)
            {
                // if the current inventory slot (i) is not empty, then it is occupied 
                if (i != desiredPosition)
                {
                    // if the occupied position is not equal to the desired position, move slots
                    inventorySlots[desiredPosition].inventoryItem = inventorySlots[i].inventoryItem;
                    inventorySlots[desiredPosition].currentStackSize = inventorySlots[i].currentStackSize;

                    inventorySlots[i].RenewObject();
                    inventorySlots[desiredPosition].RefreshSlot();
                    inventorySlots[i].RefreshSlot();
                }

                // if the current occupied slot (i) is equal to the desired position keep looking for an empty slot
                desiredPosition++;
            }
        }
    }

    public void AddItemToInventory(InventoryItemPackage type)
    {
        // WEAPONS SHOULD NOT STACK 
        int desiredStack = 0;
        if (type.isWeapon == false)
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (InventoryItemList == null) { continue; }
                Debug.Log("List count: " + InventoryItemList.Count + " Iteration: " + i);
                if (inventorySlots[i].inventoryItem != null)
                {
                    if (inventorySlots[desiredStack].inventoryItem.itemName == type.itemName && InventoryItemList[desiredStack].inventoryItem.maxStackSize > 1 && inventorySlots[desiredStack].currentStackSize < InventoryItemList[desiredStack].inventoryItem.maxStackSize)
                    {
                        inventorySlots[desiredStack].currentStackSize++;
                        inventorySlots[desiredStack].stackNumberText.text = inventorySlots[desiredStack].currentStackSize.ToString();
                        inventoryItemList[desiredStack].amount++;
                        Debug.Log("FOUND SAME TYPE AND ADDED TO DESIRED STACK");
                        desiredStack = 0;
                        return;
                    }
                    else if (i < InventoryItemList.Count)
                    {
                        desiredStack++;
                        continue;
                    }
                }

            }
        }

        // if there are no spots available for stacking, make a new stack
        for (int x = 0; x < inventorySlots.Count; x++)
        {
            if (inventorySlots[x].inventoryItem == null)
            {
                ItemStack newStack = new ItemStack();
                newStack.inventoryItem = type;
                newStack.amount = 1;
                InventoryItemList.Add(newStack);
                Debug.Log("Added specimen to NEW slot: " + inventorySlots[x]);
                inventorySlots[x].inventoryItem = type;
                inventorySlots[x].currentStackSize = 1;
                return;
            }
        }
    }

    public void RemoveNullItemsFromList()
    {
        for (int i = 0; i < InventoryItemList.Count; i++)
        {
            if (InventoryItemList[i] == null)
            {
                InventoryItemList.RemoveAt(i);
            }
        }
    }

    public void CollectNearbyResource()
    {
        collectionTimer += Time.deltaTime;

        if (collectionTimer > collectionInterval)
        {
            // needs to be placed in update although only every 
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, collectionRadius, transform.forward, layerMask);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.GetComponent<ICollectable>() != null)
                {
                    StartCoroutine(hits[i].transform.GetComponent<ICollectable>().Collect());
                    Debug.Log("Collected " + hits[i].transform.name);
                }
            }

            collectionTimer = 0f;
        }
    }

    public void ShowOrCloseInventory(InputAction.CallbackContext context)
    {
        if (GameState.instance.currentState != GameState.States.RoomClear && context.performed)
        {
            if (inventoryUI.activeSelf == true)
            {
                inventoryUI.SetActive(false);
                UIManager.instance.infoInventoryName.text = string.Empty;
                UIManager.instance.infoInventoryImage.sprite = null;
                UIManager.instance.infoInventoryText.text = string.Empty;
                GameState.instance.ChangeToPreviousState();
            }
            else
            {
                GameState.instance.ChangeStateToOpenUI();
                SortStacksInInventory();
                RemoveGapsFromInventory();
                inventoryUI.SetActive(true);
            }

            Debug.Log("Inventory key pressed");
        }
    }

    public void SellSelectedItems()
    {
        int totalEarned = 0;
        if (multiSelectedSlots.Count != 0)
        {
            // sorting the slots to count down rather than up, as to remove any chance of changing slot positions and selling the wrong item
            // using the lambda expression rather than a custom function to set a rule to decide the sorting order (switching positions based on the numbers value)
            multiSelectedSlots.Sort((a, b) => b.slotPosition.CompareTo(a.slotPosition));
            for (int i = 0; i < multiSelectedSlots.Count; i++)
            {
                if (multiSelectedSlots[i] != null)
                {
                    totalEarned += multiSelectedSlots[i].inventoryItem.sellAmount * multiSelectedSlots[i].currentStackSize;
                    Debug.Log("Removing item at position: " + multiSelectedSlots[i].slotPosition);
                    inventoryItemList.RemoveAt(multiSelectedSlots[i].slotPosition);
                    multiSelectedSlots[i].SellItem();
                    multiSelectedSlots[i].RefreshSlot();
                }
            }
            RemoveNullItemsFromList();
        }
        else if (selectedSlot.inventoryItem != null && selectedSlot.viewingSlot == true)
        {
            totalEarned += selectedSlot.inventoryItem.sellAmount * selectedSlot.amountFromStackToSell;
            if (selectedSlot.amountFromStackToSell == selectedSlot.currentStackSize) { inventoryItemList.RemoveAt(selectedSlot.slotPosition); }
            else { inventoryItemList[selectedSlot.slotPosition].amount -= selectedSlot.amountFromStackToSell; }
            selectedSlot.SellItem();
        }
        else
        {
            return;
        }

        OnClearingMultiSelectedSlotsFromList?.Invoke();

        StartCoroutine(UIManager.instance.NewNotification("Currency + " + totalEarned));
        RemoveGapsFromInventory();
        multiSelectedSlots.Clear();
    }

    public void ShowSelectedItem(InventorySlot slot)
    {
        UIManager.instance.infoInventoryName.text = slot.inventoryItem.itemName;
        UIManager.instance.infoInventoryImage.sprite = slot.inventoryItem.sprite;
        UIManager.instance.infoInventoryImage.enabled = true;
        UIManager.instance.infoInventoryText.text = slot.inventoryItem.description;
        UIManager.instance.stackAmountSlider.minValue = 0;
        UIManager.instance.stackAmountSlider.maxValue = slot.currentStackSize;
        UIManager.instance.stackAmountSlider.value = slot.currentStackSize;
        slot.slotImage.color = Color.white;
        selectedSlot = slot;
        slot.viewingSlot = true;
    }

    public void HideSelectedItem(InventorySlot slot)
    {
        if (selectedSlot == slot)
        {
            selectedSlot = null;
            UIManager.instance.infoInventoryName.text = string.Empty;
            UIManager.instance.infoInventoryImage.sprite = null;
            UIManager.instance.infoInventoryImage.enabled = false;
            UIManager.instance.infoInventoryText.text = string.Empty;
            UIManager.instance.stackAmountSlider.minValue = 0;
            UIManager.instance.stackAmountSlider.maxValue = 0;
            UIManager.instance.stackAmountSlider.value = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<SpecimenObject>())
        {
            InventoryItemPackage obj = collision.gameObject.GetComponent<SpecimenObject>().specimenType;
            AddItemToInventory(obj);
            if (Spawning.instance != null)
            {
                Spawning.instance.specimenPool.AddToPool(collision.gameObject.GetComponent<SpecimenObject>());
            }
            else
            {
                collision.gameObject.SetActive(false);
            }
            Debug.Log("Added new object to inventory: " + obj.itemName);
        }
    }
}
