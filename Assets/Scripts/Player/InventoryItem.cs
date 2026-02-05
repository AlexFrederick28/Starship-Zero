using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public bool isSpecimen;
    public bool isWeapon;

    public enum Rarity { common, rare, ultraRare, unique}
    public string name;
    public string description;
    public int sellAmount;
    [Tooltip("This is the weighting of an item (generally a specimen from an alien) being chosen to drop, not out of a 100% chance")]
    public int dropChance;
    public Rarity rarity;
    public Sprite sprite;

    public float baseDamage;
    public float baseFireRate;
    public float baseCritChance;
    public float baseCritDamage;
    public float projectileSpeed;
}
