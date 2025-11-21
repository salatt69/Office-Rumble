using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Data")]
public class WeaponData : ItemData
{
    [Header("Weapon Stats")]
    public float damage;
    public float fireRate;
    public AudioClip fireSound;
}
