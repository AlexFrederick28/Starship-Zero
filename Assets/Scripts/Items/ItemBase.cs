using UnityEngine;

public class ItemBase : MonoBehaviour
{

    // base item class - used to set up the items

    [SerializeField] public ItemScriptableObject itemType;
    public string itemBaseName;
    public int itemBaseScaling; 
    public int itemBaseCount;
    public string itemBaseDescription;
    public Sprite itemBaseSprite;


    void Start()
    {
        if (itemType != null) // if scriptable is not empty add the scriptable values
        {
            itemBaseName = itemType.itemName;
            itemBaseScaling = itemType.itemScaling;
            itemBaseCount = itemType.itemCount;
            itemBaseDescription = itemType.itemDescription;
            itemBaseSprite = itemType.itemSprite;
        }
        else
        {
            Debug.Log("item null");
        }
    }


    void Update()
    {
        
    }

    private void Awake()
    {
        //Debug.Log("Object Name [" + gameObject.name + "]");
        //Debug.Log("Item [" +  itemType.itemName+ "], Scaling [" + itemType.itemScaling + "] , Amount [" + itemType.itemCount + "]");
    }

}
