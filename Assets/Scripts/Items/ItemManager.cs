using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{

    // Item Manager - stores all the info of items and gives their effects to the weapon

    [SerializeField] public List<GameObject> itemListGO;

    [SerializeField] public List<string> itemNamesGO;
    [SerializeField] public List<int> itemScalingGO;
    [SerializeField] public List<int> itemCountGO;
    [SerializeField] public List<string> itemDescriptionGO;
    [SerializeField] public List<Sprite> itemSpritesGO;

    [SerializeField] public List<int> itemCountInitial; // used to reset the item count on death to its initial on restart

    public WeaponBase[] allWeapons;

    protected void OnEnable()
    {
        if (Spawning.instance != null)
        {
            Spawning.instance.OnCompletingInfestedRoom += RecordInitialItemCount;
        }
        if (GameState.instance != null)
        {
            GameState.instance.OnPlayerRespawn += ResetInitialItemCount;
            GameState.instance.OnPlayerRetry += ResetInitialItemCount;
        }

    }

    protected void OnDisable()
    {
        Spawning.instance.OnCompletingInfestedRoom -= RecordInitialItemCount;
    }

    // initialise
    void Start()
    {
        foreach (GameObject item in itemListGO)
        {
            Instantiate(item, transform, worldPositionStays: false);
        }

        ItemStats(); 
        ItemInfo(); 
        ItemScaleAllWeapons();
    }

    // grabs the amount of items at the start/end of a room
    public void RecordInitialItemCount()
    {
        Debug.Log("Record Initial Item Count");
        itemCountInitial = new List<int>(itemCountGO);
    }

    public void ClearInitialItemCount()
    {
        itemCountInitial.Clear();
    }

    // rewind the amount of items a player at the start of a room
    public void ResetInitialItemCount()
    {
        Debug.Log("Reset Initial Item Count");
        if (itemCountInitial.Count > 0)
        {
            itemCountGO = new List<int>(itemCountInitial);
        }
        else
        {
            Debug.Log("Nothing to Reset back to");
        }

        foreach (WeaponBase weapon in allWeapons)
        {
            weapon.ItemScaling();
        }
    }

    // finds all weapons and runs a function to calculate the items on them
    public void ItemScaleAllWeapons()
    {
        allWeapons = FindObjectsByType<WeaponBase>(FindObjectsSortMode.None);

        foreach (WeaponBase weapon in allWeapons)
        {
            weapon.ItemScaling();
        }
    }

    // finds the 2 stat items that affect a weapons stats
    public void ItemStats() 
    {
        // reset
        itemScalingGO.Clear();
        itemCountGO.Clear();

        foreach (GameObject itemGO in itemListGO)
        {
            ItemBase itemBase = itemGO.GetComponent<ItemBase>();

            if (itemBase == null)
            {
                Debug.Log("[" + itemGO.name + "] has no ItemBase ");
                return;
            }

            ItemScriptableObject item = itemBase.itemType;


            itemScalingGO.Add(item.itemScaling);
            itemCountGO.Add(item.itemCount);

            //Debug.Log("Item: Scaling [" + item.itemScaling + "], Count [" + item.itemCount + "]");
        }
    }

    // the information side of the items such as name and what they do
    public void ItemInfo()   
    {
        // reset
        itemNamesGO.Clear();
        itemDescriptionGO.Clear();
        itemSpritesGO.Clear();

        foreach (GameObject itemGO in itemListGO)
        {
            ItemBase itemBase = itemGO.GetComponent<ItemBase>();

            if (itemBase == null)
            {
                Debug.Log("[" + itemGO.name + "] has no ItemBase ");
                return;
            }

            ItemScriptableObject item = itemBase.itemType;

            itemNamesGO.Add(item.itemName);
            itemDescriptionGO.Add(item.itemDescription);
            itemSpritesGO.Add(item.itemSprite);

            //Debug.Log("Item: Name [" + item.itemName + "], Description [" + item.itemDescription + "]");
        }
    }

    // reset all items to 0
    public void ResetItemCount()
    {

        for (int i = 0; i < itemCountGO.Count; i++)
        {
            itemCountGO[i] = 0;
        }

        Debug.Log("Reset Item Count");

    }
}
