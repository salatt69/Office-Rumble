using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Data")]
public class WeaponData : ItemData
{
    [Header("Projectile")]
    public Projectile projectilePrefab;
}