using UnityEngine;

public class ItemBase : MonoBehaviour
{
    [SerializeField] public ItemScriptableObject itemType;
    public string itemBaseName;
    public int itemBaseScaling; 
    public int itemBaseCount;
    public string itemBaseDescription;
    public Sprite itemBaseSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (itemType != null)
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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        //Debug.Log("Object Name [" + gameObject.name + "]");
        //Debug.Log("Item [" +  itemType.itemName+ "], Scaling [" + itemType.itemScaling + "] , Amount [" + itemType.itemCount + "]");
    }

}
