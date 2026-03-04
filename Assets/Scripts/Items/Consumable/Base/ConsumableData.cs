using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumable Data")]
public class ConsumableData : ItemData
{
    [Header("Consumable Info")]
    public float healAmount;
    public float useTime;
}
