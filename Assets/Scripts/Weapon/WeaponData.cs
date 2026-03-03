using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Data")]
public class WeaponData : ItemData
{
    [Header("Firing")]
    public float shotInterval = 0.3f;          // base interval at AttackSpeed = 1
    public float damageCoefficient = 1f;       // 1 = 100% of EntityBody.Damage

    [Header("Projectile")]
    public Projectile projectilePrefab;
    public float projectileSpeedOverride = -1f;     // <=0 means use prefab speed
    public float projectileLifetimeOverride = -1f;  // <=0 means use prefab lifetime
}