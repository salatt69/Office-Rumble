using UnityEngine;

public class Consumable : Item, IUsable
{
    public ConsumableData cd => (ConsumableData)Data;

    public void Use(GameObject target)
    {
        var hp = target.GetComponentInChildren<Health>();

        hp.Heal(cd.healAmount);
    }
}
