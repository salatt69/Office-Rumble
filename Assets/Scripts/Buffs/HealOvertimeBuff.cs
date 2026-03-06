
using UnityEngine;

// TODO: make it overtime
[CreateAssetMenu(menuName = "Buffs/Overtime/Heal")]
public class HealOvertimeBuff : BuffData
{
    public float amount;
    public float duration = 5f;

    public override void Apply(GameObject target)
    {
        var h = target.GetComponent<Health>();
        if (h) h.Heal(amount);
    }
}