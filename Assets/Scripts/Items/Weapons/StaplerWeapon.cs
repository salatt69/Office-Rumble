using UnityEngine;

public class StaplerWeapon : Weapon
{
    float nextShotTime;

    public override void Use()
    {
        if (!Owner || !WD || !WD.projectilePrefab) return;

        if (Time.time < nextShotTime) return;

        Vector2 dir = GetMouseDir(Owner.transform.position);

        DamageData dmg = BuildProjectileDamage(dir);

        Projectile proj = Instantiate(WD.projectilePrefab, firePoint.position, Quaternion.identity);

        float? spd = WD.projectileSpeedOverride > 0f ? WD.projectileSpeedOverride : (float?)null;
        float? life = WD.projectileLifetimeOverride > 0f ? WD.projectileLifetimeOverride : (float?)null;

        proj.Init(Owner, dir, dmg, spd, life);

        nextShotTime = Time.time + GetShotInterval();
    }
}