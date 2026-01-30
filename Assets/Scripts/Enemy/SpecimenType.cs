using System;
using UnityEngine;

[Serializable]
public class SpecimenType
{
    public enum Rarity { common, rare, ultraRare, unique}
    public string name;
    public string description;
    public int sellAmount;
    [Tooltip("This is the weighting of a specimen being chosen to drop, not out of a 100% chance")]
    public int dropChance;
    public Rarity rarity;
    public Sprite sprite;

}
