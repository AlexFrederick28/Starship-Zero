using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
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
            GameObject newSlot = Instantiate(UIManager.instance.slot, UIManager.instance.inventoryContentParent.transform);
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
        for (int i = 0; i < InventoryList.Count; i++)
        {
            if (InventoryList[i] == null)
            {
                InventoryList.RemoveAt(i);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<SpecimenObject>())
        {
            SpecimenType obj = collision.gameObject.GetComponent<SpecimenObject>().specimenType;
            AddItemToInventory(obj);
            Spawning.instance.specimenPool.AddToPool(collision.gameObject.GetComponent<SpecimenObject>());
            Debug.Log("Added new specimen to inventory: " + obj.name);
        }
    }
}
