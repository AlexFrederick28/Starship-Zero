using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/ItemScriptableObject")]
public class ItemScriptableObject : ScriptableObject
{

    // item scriptable object - contains the data for items

    // add item stat ID? for easy access in list?
    public string itemName;
    public int itemScaling; // not to be confused with the documents scaling (which are all linear), this refers to the increase amount per item count e.g. 5, 10, 15 etc
    public int itemCount;
    public string itemDescription;
    public Sprite itemSprite;

}
