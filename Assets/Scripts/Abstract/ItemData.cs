using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    public GameObject prefab;

    [Header("Item Sprites")]
    public Sprite unequipped;
    public Sprite highlightedUnequipped;
    public Sprite equipped;
    public Sprite icon;
}
