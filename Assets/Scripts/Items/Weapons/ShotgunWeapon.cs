using UnityEngine;

public class ShotgunWeapon : Weapon
{
    [SerializeField] protected float spread = 15f;
    float nextShotTime;

    public override void Use()
    {
        if (!Owner || !WD || !WD.projectilePrefab) return;

        if (Time.time < nextShotTime) return;

        Vector2 baseDir = GetMouseDir(Owner.transform.position);

        float angleOffset = spread / 2f;
        for (int i = -1; i <= 1; i++)
        {
            float angle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;
            angle += i * angleOffset;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            DamageData dmg = BuildProjectileDamage(dir);

            Projectile proj = Instantiate(WD.projectilePrefab, firePoint.position, Quaternion.identity);

            float? spd = WD.projectileSpeedOverride > 0f ? WD.projectileSpeedOverride : (float?)null;
            float? life = WD.projectileLifetimeOverride > 0f ? WD.projectileLifetimeOverride : (float?)null;

            proj.Init(Owner, dir, dmg, spd, life);
        }

        nextShotTime = Time.time + GetShotInterval();
    }
}
