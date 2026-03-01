using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [SerializeField] Transform firePoint;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] float fireInterval = 0.4f;

    float nextFire;

    public bool TryShootAt(Transform target)
    {
        if (!target) return false;
        if (Time.time < nextFire) return false;

        nextFire = Time.time + fireInterval;

        Vector2 dir = ((Vector2)(target.position - firePoint.position)).normalized;

        var proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.Init(gameObject, dir);

        return true;
    }
}