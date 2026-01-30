using System;
using UnityEngine;

[Serializable]
public class Specimens
{
    public enum Rarity { common, rare, ultraRare, unique}
    public string name;
    public string description;
    public int sellAmount;
    public Rarity rarity;
    public Sprite sprite;

}
