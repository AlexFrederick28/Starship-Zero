using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public ScriptableObject inventoryItemPackage;
    public bool isSpecimen;
    public bool isWeapon;

    public enum Rarity { common, rare, ultraRare, unique }
    [Header("General Item Info")]
    [Tooltip("How many times this item can stack")]
    public int maxStackSize;
    public string name;
    public string description;
    public int sellAmount;
    public Rarity rarity;
    public Sprite sprite;
    [Header("Specimen")]
    [Tooltip("This is the weighting of an item (generally a specimen from an alien) being chosen to drop, not out of a 100% chance")]
    public int dropChance;

    [Header("Weapon")]
    public WeaponScriptableObject weapon;
}
