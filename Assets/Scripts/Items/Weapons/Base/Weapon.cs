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

    public virtual void Use(GameObject owner) { }

    protected DamageData BuildProjectileDamage(GameObject owner, Vector2 dir)
    {
        var body = owner ? owner.GetComponent<EntityBody>() : null;

        float dmgAmount = body.Damage * Mathf.Max(0f, WD.damageCoefficient);

        if (Random.value < body.CritChance)
        {
            dmgAmount *= body.CritMultiplier;
        }

        return new DamageData(owner, dmgAmount, dir, DamageType.Projectile);
    }

    protected float GetAttackSpeed(GameObject owner)
    {
        var body = owner ? owner.GetComponent<EntityBody>() : null;
        return body ? body.AttackSpeed : 1f;
    }

    protected float GetShotInterval(GameObject owner)
    {
        float atkSpd = Mathf.Max(0.01f, GetAttackSpeed(owner));
        return WD.shotInterval / atkSpd;
    }

    protected Vector2 GetMouseDir(Vector3 fromWorldPos)
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;
        return ((Vector2)(mouseWorld - fromWorldPos)).normalized;
    }
}