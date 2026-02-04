using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    // need to create seperate list for other objects that are not specimens
    public int maxInventorySlots;
    public InventorySlot selectedSlot;
    public List<InventorySlot> inventorySlots;
    public List<SpecimenType> inventoryList;
    public List<SpecimenType> InventoryList
    {
        get { return inventoryList; }
        set
        {
            if (inventoryList.Count >= maxInventorySlots)
            {
                Debug.Log("Inventory full!");
                return;
            }
            else
            {
                // sort inventory (no gaps): Rather than calling this each time the inventory is updated, only update it when the inventory is opened visually (Perhaps a keybind)
                //RemoveGapsFromInventory();
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

    public void RemoveGapsFromInventory()
    {
        int desiredPosition = 0;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            // scan through the inventory
            if (inventorySlots[i].specimenType.name != string.Empty)
            {
                // if the current inventory slots (i) name is not empty, then it is occupied 
                if (i != desiredPosition)
                {
                    // if the occupied position is not equal to the desired position, move slots
                    inventorySlots[desiredPosition].specimenType = inventorySlots[i].specimenType;

                    inventorySlots[i].RenewObject();
                    inventorySlots[desiredPosition].RefreshSlot();
                    inventorySlots[i].RefreshSlot();
                }

                // if the current occupied slot (i) is equal to the desired position keep looking for an empty slot
                desiredPosition++;
            }
        }
    }

    public void AddItemToInventory(SpecimenType type)
    {
        InventoryList.Add(type);
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].specimenType.name == string.Empty)
            {
                Debug.Log("Added specimen to slot: " + inventorySlots[i]);
                inventorySlots[i].specimenType = type;
                return;
            }
        }
    }

    public void RemoveNullItemsFromList()
    {
        for (int i = 0; i < inventoryList.Count; i++)
        {
            InventoryList.RemoveAt(i);
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
                UIManager.instance.infoName.text = string.Empty;
                UIManager.instance.infoImage.sprite = null;
                UIManager.instance.infoText.text = string.Empty;
                GameState.instance.ChangeToPreviousState();
            }
            else
            {
                GameState.instance.ChangeStateToOpenUI();
                RemoveGapsFromInventory();
                inventoryUI.SetActive(true);
            }

            Debug.Log("Inventory key pressed");
        }
    }

    public void SellSelectedItems()
    {
        int earned = 0;
        if (multiSelectedSlots.Count != 0)
        {
            for (int i = 0; i < multiSelectedSlots.Count; i++)
            {
                if (multiSelectedSlots[i] != null)
                {
                    earned += multiSelectedSlots[i].specimenType.sellAmount;
                    multiSelectedSlots[i].SellItem();
                    multiSelectedSlots[i].RefreshSlot();
                }
            }
            RemoveGapsFromInventory();
            RemoveNullItemsFromList();
        }
        else
        {
            return;
        }

        OnClearingMultiSelectedSlotsFromList?.Invoke();

        StartCoroutine(UIManager.instance.NewNotification("Currency + " + earned));
        multiSelectedSlots.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<SpecimenObject>())
        {
            SpecimenType obj = collision.gameObject.GetComponent<SpecimenObject>().specimenType;
            AddItemToInventory(obj);
            if (Spawning.instance != null)
            {
                Spawning.instance.specimenPool.AddToPool(collision.gameObject.GetComponent<SpecimenObject>());
            }
            else
            {
                collision.gameObject.SetActive(false);
            }
            Debug.Log("Added new specimen to inventory: " + obj.name);
        }
    }
}
