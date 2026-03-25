
using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/Permanent/Stat Modifier")]
public class StatModifierBuff : BuffData
{
    public StatType stat;
    public StatModMode mode;
    public float value;

    public override void Apply(GameObject target)
    {
        var body = target.GetComponent<EntityBody>();
        if (!body) return;

        body.AddBuff(new BuffStat { stat = stat, mode = mode, value = value });
    }
    public override string GetDescription() => $"+{value} {stat}";
}