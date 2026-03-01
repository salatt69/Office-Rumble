using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : Item, IUsable
{
    [SerializeField] protected Transform firePoint;
    public WeaponData WD => (WeaponData)Data;

    public override void Initialize(ItemData newData)
    {
        base.Initialize(newData);
        if (firePoint == null)
            firePoint = transform.Find("FirePoint");
    }

    public virtual void Use(GameObject owner)
    {
    }

    protected Vector2 GetMouseDir(Vector3 fromWorldPos)
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;
        return ((Vector2)(mouseWorld - fromWorldPos)).normalized;
    }
}