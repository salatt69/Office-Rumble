using UnityEngine;

public enum Tier
{
    Common = 64,
    Rare = 32,
    Epic = 16,
    Legendary = 8
}

public abstract class ItemData : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    public GameObject prefab;
    public int price;
    public Tier tier;

    [Header("Item Sprites")]
    public Sprite unequipped;
    public Sprite highlightedUnequipped;
    public Sprite equipped;
    public Sprite icon;
}
