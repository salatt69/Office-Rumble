using UnityEngine;

[CreateAssetMenu(menuName = "Items/Charged Weapon")]
public class ChargedWeaponData : WeaponData
{
    [Header("Charge")]
    public float weaponChargeTime = 1.0f;
    public float releaseGrace = 0.08f;
}