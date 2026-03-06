
using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/Timed/Stat Modifier")]
public class StatModifierTimedBuff : BuffData
{
    public StatType stat;
    public StatModMode mode;
    public float value;
    public float duration = 5f;

    public override void Apply(GameObject target)
    {
        var body = target.GetComponent<EntityBody>();
        if (!body) return;

        body.AddTimedBuff(new BuffStat { stat = stat, mode = mode, value = value }, duration);
    }
}