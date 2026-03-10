using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class WeaponData : ItemData
{
    [Header("Firing")]
    public float damageCoefficient = 1f;
    public float attackSpeedCoefficient = 1f;
    public float critDamageCoefficient = 1f;
    public float critChanceCoefficient = 1f;

    [Header("Projectile")]
    public Projectile projectilePrefab;
    public float projectileSpeedOverride = -1f;
    public float projectileLifetimeOverride = -1f;
}