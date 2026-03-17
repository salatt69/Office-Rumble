using UnityEngine;

[RequireComponent(typeof(EntityBody))]
public class EnemyShooter : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform firePoint;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] EntityBody body;

    [Header("Firing")]
    [SerializeField] float projectileSpeedOverride = -1f;    // <=0 uses prefab
    [SerializeField] float projectileLifetimeOverride = -1f; // <=0 uses prefab

    float nextFire;

    void Awake()
    {
        if (!firePoint) firePoint = transform;
        if (!body) body = GetComponent<EntityBody>();
    }

    public bool TryShootAt(Transform target)
    {
        if (!target || !projectilePrefab || !firePoint) return false;

        float attackSpeed = body.AttackSpeed;
        float interval = 1.0f / Mathf.Max(0.01f, attackSpeed);

        if (Time.time < nextFire) return false;
        nextFire = Time.time + interval;

        Vector2 dir = (target.position - firePoint.position);
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;
        dir.Normalize();

        // Build baked damage from EntityBody
        float dmgAmount = body.Damage;

        // Crit (optional; keep simple for now)
        float critChance = body ? body.CritChance : 0f;
        if (Random.value < critChance)
            dmgAmount *= 2f; // placeholder crit multiplier

        var dmg = new DamageData(gameObject, dmgAmount, dir, DamageType.Projectile);

        var proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        float? spd = projectileSpeedOverride > 0f ? projectileSpeedOverride : (float?)null;
        float? life = projectileLifetimeOverride > 0f ? projectileLifetimeOverride : (float?)null;

        proj.Init(gameObject, dir, dmg, spd, life);

        return true;
    }
}