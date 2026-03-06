using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumable")]
public class ConsumableData : ItemData
{
    [Header("Consumable Settings")]
    public int maxUses = 1;
    public BuffData[] buffs;
}
