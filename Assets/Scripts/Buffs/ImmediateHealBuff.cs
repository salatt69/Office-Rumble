using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/Immediate Heal")]
public class ImmediateHealBuff : BuffData
{
    public float amount;
    public override void Apply(GameObject target)
    {
        var h = target.GetComponent<Health>();
        if (h) h.Heal(amount);
    }
}