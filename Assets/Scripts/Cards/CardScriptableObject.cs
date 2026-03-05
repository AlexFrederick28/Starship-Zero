using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/ItemScriptableObject")]
public class CardScriptableObject : ScriptableObject
{
    // item scriptable object - contains the data for items

    // add item stat ID? for easy access in list?
    public bool defaultCard = false;
    public string cardName;
    public float cardScalingMin; // not to be confused with the documents scaling (which are all linear), this refers to the increase amount per item count e.g. 5, 10, 15 etc
    public float cardScalingMax; 
    public int cardLevel;
    public string cardDescription;
    public Sprite cardSprite;

}
