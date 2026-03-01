using UnityEngine;

public class StaplerWeapon : Weapon
{
    [Header("Configuration")]
    [SerializeField] float shotInterval = 0.3f;

    float nextShotTime;

    public override void Use(GameObject owner)
    {
        if (Time.time < nextShotTime)
            return;

        Fire(owner);
        nextShotTime = Time.time + shotInterval;
    }

    void Fire(GameObject owner)
    {
        Debug.Log($"Stapler fired by {owner.name}! {Random.Range(10000, 1000000)}");

        if (owner == null) return;
        if (firePoint == null || WD.projectilePrefab == null) return;

        Vector2 dir = GetMouseDir(firePoint.position);

        var proj = Instantiate(WD.projectilePrefab, firePoint.position, Quaternion.identity);
        proj.Init(owner, dir);
    }
}