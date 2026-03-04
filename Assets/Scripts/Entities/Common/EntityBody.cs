using UnityEngine;

public class EntityBody : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] float baseMaxHealth = 100f;
    [SerializeField] float baseHealthRegen = 0f;
    [SerializeField] float baseMoveSpeed = 7f;
    [SerializeField] float baseAcceleration = 40f;
    [SerializeField] float baseDamage = 10f;
    [SerializeField] float baseAttackSpeed = 1f;
    [SerializeField, Range(0f, 1f)] float baseCrit = 0.05f;
    [SerializeField] float baseCritMultiplier = 1.5f;

    [Header("Runtime Modifiers (items/buffs)")]
    [SerializeField] float bonusMaxHealth = 0f;
    [SerializeField] float bonusHealthRegen = 0f;
    [SerializeField] float bonusMoveSpeed = 0f;
    [SerializeField] float bonusAcceleration = 0f;
    [SerializeField] float bonusDamage = 0f;
    [SerializeField] float bonusAttackSpeed = 0f;
    [SerializeField] float bonusCrit = 0f;
    [SerializeField] float bonusCritMultiplier = 1f;


    public float MaxHealth => Mathf.Max(1f, baseMaxHealth + bonusMaxHealth);
    public float HealthRegen => baseHealthRegen + bonusHealthRegen;
    public float MoveSpeed => Mathf.Max(0f, baseMoveSpeed + bonusMoveSpeed);
    public float Acceleration => Mathf.Max(0f, baseAcceleration + bonusAcceleration);
    public float Damage => Mathf.Max(0f, baseDamage + bonusDamage);
    public float AttackSpeed => Mathf.Max(0.01f, baseAttackSpeed + bonusAttackSpeed);
    public float CritChance => Mathf.Clamp01(baseCrit + bonusCrit);
    public float CritMultiplier => Mathf.Max(1f, baseCritMultiplier + bonusCritMultiplier);


    public void AddBonusDamage(float amount) => bonusDamage += amount;
    public void AddBonusMaxHealth(float amount) => bonusMaxHealth += amount;
    public void AddBonusCrit(float amount) => bonusCrit += amount;
    public void AddBonusCritMultiplier(float amount) => bonusCritMultiplier += amount;
    public void AddBonusAttackSpeed(float amount) => bonusAttackSpeed += amount;

    public void ClearBonuses()
    {
        bonusMaxHealth = 0f;
        bonusHealthRegen = 0f;
        bonusMoveSpeed = 0f;
        bonusAcceleration = 0f;
        bonusDamage = 0f;
        bonusAttackSpeed = 0f;
        bonusCrit = 0f;
        bonusCritMultiplier = 1f;
    }
}