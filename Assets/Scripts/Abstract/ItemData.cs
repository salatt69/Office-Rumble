using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    public Sprite icon;
    public GameObject prefab;    
}
