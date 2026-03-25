using UnityEngine;

public class Consumable : Item, IUsable
{
    public ConsumableData CD => (ConsumableData)Data;

    public void Use()
    {
        if (CD == null) return;

        string effects = "";

        foreach (var b in CD.buffs)
        {
            if (b) 
            {
                b.Apply(Owner);
                effects += (string.IsNullOrEmpty(effects) ? "" : ", ") + b.GetDescription();
            }
        }

        var notifier = Object.FindAnyObjectByType<ConsumableNotificationUI>();
        if (notifier != null)
            notifier.Show(CD, effects);
    }
}