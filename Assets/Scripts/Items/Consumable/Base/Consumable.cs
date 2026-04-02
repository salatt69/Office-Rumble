using UnityEngine;

public class Consumable : Item, IUsable
{
    public ConsumableData CD => (ConsumableData)Data;

    public void Use()
    {
        if (CD == null) return;

        foreach (var b in CD.buffs)
        {
            if (b) 
            {
                b.Apply(Owner);
            }
        }
    }
}