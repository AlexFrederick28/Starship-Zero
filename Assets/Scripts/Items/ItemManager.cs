using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{

    [SerializeField] public List<GameObject> itemListGO;

    [SerializeField] public List<string> itemNamesGO;
    [SerializeField] public List<int> itemScalingGO;
    [SerializeField] public List<int> itemCountGO;
    [SerializeField] public List<string> itemDescriptionGO;
    [SerializeField] public List<Sprite> itemSpritesGO;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject item in itemListGO)
        {
            Instantiate(item, transform, worldPositionStays: false);
        }

        ItemStats(); 
        ItemInfo(); 
    }

    public void ItemStats() // stats that affect a weapon
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

    public void ItemInfo() // other info about the weapon      
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
}
