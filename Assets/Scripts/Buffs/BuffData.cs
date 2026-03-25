using UnityEngine;

public abstract class BuffData : ScriptableObject
{
    public GameObject[] effects;
    public abstract void Apply(GameObject target);
    public virtual string GetDescription() => "";
}