using UnityEngine;

public class Weapon : Item, IUsable
{
    [Header("References")]
    [SerializeField] protected Transform firePoint;

    public WeaponData wd => (WeaponData)Data;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Initialize(ItemData newData)
    {
        base.Initialize(newData);

        if (firePoint == null)
            firePoint = transform.Find("FirePoint");
    }

    public override void OnEquip()
    {
        base.OnEquip();

        Debug.Log($"Weapon [{wd.itemName}] ready for battle!");
    }

    public override void OnUnequip()
    {
        base.OnUnequip();

        Debug.Log($"Weapon [{wd.itemName}] being unequipped...");
    }

    public void Use(GameObject target)
    {
        if (firePoint == null)
        {
            Debug.LogWarning($"{name} has no FirePoint assigned!");
            return;
        }

        Debug.DrawRay(firePoint.position, firePoint.right, Color.red, 1f);
        Debug.Log($"{wd.itemName} fired, and dealt WHOPPING {wd.damage} DAMAGE to ABSOLUTELY nothing!");
    }
}
