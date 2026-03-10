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

    public virtual void Use() { }

    protected DamageData BuildProjectileDamage(Vector2 dir)
    {
        var body = Owner ? Owner.GetComponent<EntityBody>() : null;

        float dmgAmount = body.Damage * Mathf.Max(0f, WD.damageCoefficient);
        float critChanceValue = body.CritChance * Mathf.Max(1f, WD.critChanceCoefficient);

        if (Random.value < critChanceValue)
        {
            float critDamageValue = body.CritDamage * Mathf.Max(1f, WD.critDamageCoefficient);
            dmgAmount *= critDamageValue;
        }

        return new DamageData(Owner, dmgAmount, dir, DamageType.Projectile);
    }

    protected float GetAttackSpeed()
    {
        var body = Owner ? Owner.GetComponent<EntityBody>() : null;
        return body ? body.AttackSpeed : 1f;
    }

    protected float GetShotInterval()
    {
        float atkSpd = Mathf.Max(0.01f, GetAttackSpeed());
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