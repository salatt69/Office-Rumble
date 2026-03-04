using UnityEngine;

public class StaplerWeapon : Weapon
{
    float nextShotTime;

    public override void Use(GameObject owner)
    {
        if (!owner || !WD || !WD.projectilePrefab) return;

        if (Time.time < nextShotTime) return;

        Vector2 dir = GetMouseDir(owner.transform.position);

        DamageData dmg = BuildProjectileDamage(owner, dir);

        Projectile proj = Instantiate(WD.projectilePrefab, firePoint.position, Quaternion.identity);

        float? spd = WD.projectileSpeedOverride > 0f ? WD.projectileSpeedOverride : (float?)null;
        float? life = WD.projectileLifetimeOverride > 0f ? WD.projectileLifetimeOverride : (float?)null;

        proj.Init(owner, dir, dmg, spd, life);

        nextShotTime = Time.time + GetShotInterval(owner);
    }
}