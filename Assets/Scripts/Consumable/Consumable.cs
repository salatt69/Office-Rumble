using UnityEngine;

public class Consumable : Item, IUsable
{
    public ConsumableData CD => (ConsumableData)Data;

    public void Use(GameObject target)
    {
        var hp = target.GetComponentInChildren<Health>();

        hp.Heal(CD.healAmount);
    }

    public void StopUsing(GameObject owner)
    {
        throw new System.NotImplementedException();
    }
}
