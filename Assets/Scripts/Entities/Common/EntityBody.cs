using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBody : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] float baseMaxHealth = 100f;
    [SerializeField] float baseHealthRegen = 0f;
    [SerializeField] float baseMoveSpeed = 5f;
    [Tooltip("Higher value == faster movement smooth, and vise versa")]
    [SerializeField] float baseAcceleration = 1f;
    [SerializeField] float baseDamage = 10f;
    [SerializeField] float baseAttackSpeed = 1f;
    [SerializeField] float baseCritChance = 0f;
    [SerializeField] float baseCritDamage = 1.5f;

    readonly List<BuffStat> buffs = new();

    public float MaxHealth => ApplyMods(StatType.MaxHealth, baseMaxHealth);
    public float HealthRegen => ApplyMods(StatType.HealthRegen, baseHealthRegen);
    public float MoveSpeed => ApplyMods(StatType.MoveSpeed, baseMoveSpeed);
    public float Acceleration => ApplyMods(StatType.Acceleration, baseAcceleration);
    public float Damage => ApplyMods(StatType.Damage, baseDamage);
    public float AttackSpeed => ApplyMods(StatType.AttackSpeed, baseAttackSpeed);
    public float CritChance => ApplyMods(StatType.CritChance, baseCritChance);
    public float CritDamage => ApplyMods(StatType.CritDamage, baseCritDamage);
    public GameObject EntityGameObject => gameObject;

    public event Action OnStatsChanged;

    public void AddBuff(BuffStat buff)
    {
        buffs.Add(buff);
        OnStatsChanged?.Invoke();
    }

    public void AddTimedBuff(BuffStat buff, float duration)
    {
        buffs.Add(buff);
        OnStatsChanged?.Invoke();
        StartCoroutine(RemoveLater(buff, duration));
    }

    public void RemoveBuff(BuffStat buff)
    {
        buffs.Remove(buff);
        OnStatsChanged?.Invoke();
    }

    float ApplyMods(StatType stat, float baseValue)
    {
        float add = 0f;
        float mul = 1f;

        for (int i = 0; i < buffs.Count; i++)
        {
            var m = buffs[i];
            if (m.stat != stat) continue;

            if (m.mode == StatModMode.Add) add += m.value;
            else if (m.mode == StatModMode.Mul) mul *= (m.value);
        }

        return (baseValue + add) * mul;
    }

    IEnumerator RemoveLater(BuffStat buuff, float duration)
    {
        yield return new WaitForSeconds(duration);
        buffs.Remove(buuff);
    }
}

public enum StatType { MaxHealth, HealthRegen, MoveSpeed, Acceleration, Damage, AttackSpeed, CritChance, CritDamage }
public enum StatModMode { Add, Mul }

[System.Serializable]
public class BuffStat
{
    public StatType stat;
    public StatModMode mode;
    public float value;
}