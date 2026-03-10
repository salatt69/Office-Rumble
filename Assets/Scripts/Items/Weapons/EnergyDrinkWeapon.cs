using UnityEngine;

public class EnergyDrinkWeapon : ChargedWeapon
{
    [Header("Burst Settings")]
    [SerializeField] int minProjectiles = 3;
    [SerializeField] int maxProjectiles = 12;
    [SerializeField, Range(30f, 180f)] float arcDegrees = 90f;

    [Header("Burst Randomness")]
    [SerializeField, Range(0f, 1f)] float centerBias = 0.7f;
    [SerializeField] float spawnJitterRadius = 0.08f;
    [SerializeField] Vector2 speedMultiplierRange = new(0.7f, 1.15f);
    [SerializeField] Vector2 scaleRange = new(0.8f, 1.35f);
    [SerializeField] Vector2 lifetimeMultiplierRange = new(0.75f, 1.15f);

    public override void Fire(GameObject owner)
    {
        if (!owner) return;
        if (!firePoint || WD == null || !WD.projectilePrefab) return;

        float tCharge = Mathf.Clamp01(currentCharge / CWD.weaponChargeTime);
        int count = Mathf.RoundToInt(Mathf.Lerp(minProjectiles, maxProjectiles, tCharge));

        // Aim source: holder direction (not mouse)
        Vector2 centerDir = GetMouseDir(owner.transform.position);
        if (centerDir.sqrMagnitude < 0.0001f) centerDir = Vector2.right;

        float halfArc = arcDegrees * 0.5f;

        // If you want "one crit roll per burst", roll once here and reuse
        // var baseDamageData = BuildProjectileDamage(owner, centerDir);

        for (int i = 0; i < count; i++)
        {
            // Biased random angle
            float u = Random.value;
            float biased = Mathf.Lerp(u, 0.5f, centerBias);
            float offset = Mathf.Lerp(-halfArc, halfArc, biased);

            Vector2 dir = Rotate(centerDir, offset);

            // Spawn jitter
            Vector2 jitter = Random.insideUnitCircle * spawnJitterRadius;
            Vector3 spawnPos = firePoint.position + (Vector3)jitter;

            var proj = Instantiate(WD.projectilePrefab, spawnPos, Quaternion.identity);

            // Visual randomness
            proj.transform.localScale *= Random.Range(scaleRange.x, scaleRange.y);

            // Speed/Lifetime randomness (local overrides)
            float speedMult = Random.Range(speedMultiplierRange.x, speedMultiplierRange.y);
            float lifeMult = Random.Range(lifetimeMultiplierRange.x, lifetimeMultiplierRange.y);

            float? speedOverride = proj.Speed * speedMult;
            float? lifetimeOverride = proj.Lifetime * lifeMult;

            // Build baked damage from EntityBody + WeaponData coefficient
            // (crit can roll per projectile; if you want per-burst, compute once outside loop)
            DamageData dmg = BuildProjectileDamage(dir);

            proj.Init(owner, dir, dmg, speedOverride, lifetimeOverride);
        }
    }

    static Vector2 Rotate(Vector2 v, float degrees)
    {
        float r = degrees * Mathf.Deg2Rad;
        float s = Mathf.Sin(r);
        float c = Mathf.Cos(r);
        return new Vector2(c * v.x - s * v.y, s * v.x + c * v.y);
    }
}