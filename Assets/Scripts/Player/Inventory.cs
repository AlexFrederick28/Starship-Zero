using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // need to create seperate list for other objects that are not specimens
    public int MaxInventorySlots { get; private set; }
    public List<SpecimenType> inventoryList;
    public List<SpecimenType> InventoryList
    {
        get { return inventoryList; }
        set
        {
            if (inventoryList.Count >= MaxInventorySlots)
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


    private void Update()
    {
        CollectNearbyResource();
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<SpecimenObject>())
        {
            SpecimenType obj = collision.gameObject.GetComponent<SpecimenObject>().specimenType;
            InventoryList.Add(obj);
            Spawning.instance.specimenPool.AddToPool(collision.gameObject.GetComponent<SpecimenObject>());
            Debug.Log("Added new specimen to inventory: " + obj.name);
        }
    }
}
