using UnityEngine;

[CreateAssetMenu(fileName = "LevelUpCardScriptableObject", menuName = "Scriptable Objects/LevelUpCardScriptableObject")]
public class LevelCardScriptableObject : ScriptableObject
{
    public string cardName; // heading of the card
    public string cardText; // text description to display on the card
    public Sprite cardSprite; // image to display on the card
    public int cardItemCount; // amount of item to give to the player
    public CardItemEffect cardEffectType; // item that will get obtained/upgraded
    // Color? for card boarder

    public enum CardItemEffect
    {
        AttackDamage,
        AttackSpeed,
        CritChance,
        CritDamage
    }

}
