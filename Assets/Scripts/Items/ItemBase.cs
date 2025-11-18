using UnityEngine;

public class ItemBase : MonoBehaviour
{
    [SerializeField] public ItemScriptableObject itemType;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
