using UnityEngine;

public class EnergyDrinkWeapon : Weapon
{
    [Header("Charge")]
    [SerializeField] float maxChargeTime = 1.2f;
    [SerializeField] int minProjectiles = 3;
    [SerializeField] int maxProjectiles = 12;
    [SerializeField, Range(30f, 180f)] float arcDegrees = 90f;
    [SerializeField] float releaseGrace = 0.08f;

    [Header("Burst Randomness")]
    [SerializeField, Range(0f, 1f)] float centerBias = 0.7f;
    [SerializeField] float spawnJitterRadius = 0.08f;
    [SerializeField] Vector2 speedMultiplierRange = new(0.7f, 1.15f);
    [SerializeField] Vector2 scaleRange = new(0.8f, 1.35f);
    [SerializeField] Vector2 lifetimeMultiplierRange = new(0.75f, 1.15f);

    float charge;
    float lastUseTime;
    bool charging;
    GameObject lastOwner;

    void Update()
    {
        if (charging && (Time.time - lastUseTime) > releaseGrace)
        {
            FireBurst(lastOwner);
            charging = false;
            charge = 0f;
        }
    }

    public override void Use(GameObject owner)
    {
        charging = true;
        lastOwner = owner;
        lastUseTime = Time.time;

        charge += Time.deltaTime;
        if (charge > maxChargeTime)
            charge = maxChargeTime;
    }

    void FireBurst(GameObject owner)
    {
        if (owner == null) return;
        if (firePoint == null || WD.projectilePrefab == null) return;

        float tCharge = Mathf.Clamp01(charge / maxChargeTime);
        int count = Mathf.RoundToInt(Mathf.Lerp(minProjectiles, maxProjectiles, tCharge));

        Vector2 centerDir = GetMouseDir(firePoint.position);
        float halfArc = arcDegrees * 0.5f;

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

            proj.transform.localScale *= Random.Range(scaleRange.x, scaleRange.y);

            proj.Speed *= Random.Range(speedMultiplierRange.x, speedMultiplierRange.y);
            proj.Lifetime *= Random.Range(lifetimeMultiplierRange.x, lifetimeMultiplierRange.y);

            proj.Init(owner, dir);
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