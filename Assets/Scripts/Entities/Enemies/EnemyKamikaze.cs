using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EnemySensor))]
public class EnemyKamikaze : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] float explosionRadius = 2f;
    [SerializeField] float explosionDamageMultiplier = 2f;
    [SerializeField] float knockbackForce = 10f;
    [SerializeField] LayerMask targetLayers;

    [Header("Timing")]
    [SerializeField] float fuseTime = 0.5f;

    EnemySensor sensor;
    AIPath aiPath;
    AIDestinationSetter destinationSetter;
    EntityBody body;
    Animator animator;
    Health health;

    bool isExploding;

    void Awake()
    {
        sensor = GetComponent<EnemySensor>();
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        body = GetComponent<EntityBody>();
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<Health>();

        if (aiPath && body)
            aiPath.maxSpeed = body.MoveSpeed;
    }

    void Update()
    {
        if (isExploding) return;

        if (sensor.target == null)
        {
            if (aiPath) aiPath.isStopped = true;
            animator?.SetBool("ShouldChase", false);
            return;
        }

        destinationSetter.target = sensor.target;

        float dist = sensor.distanceToTarget;
        bool canSeeTarget = sensor.hasLineOfSight;

        bool isChasing = dist > sensor.AttackRadius || !canSeeTarget;
        if (aiPath) aiPath.isStopped = !isChasing;

        animator?.SetBool("ShouldChase", isChasing);

        if (sensor.TargetInAttackRange && canSeeTarget)
            StartCoroutine(Explode());
    }

    System.Collections.IEnumerator Explode()
    {
        if (isExploding) yield break;
        isExploding = true;

        if (aiPath) aiPath.isStopped = true;

        animator?.SetTrigger("Explode");

        yield return new WaitForSeconds(fuseTime);

        float damageAmount = body ? body.Damage * explosionDamageMultiplier : 10f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, targetLayers);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable == null) continue;

            Vector2 direction = (hit.transform.position - transform.position).normalized;
            var dmgData = new DamageData(gameObject, damageAmount, direction, DamageType.Explosion);
            dmgData.knockbackForceMultiplier = knockbackForce;
            damageable.TakeDamage(dmgData);
        }

        if (health)
            health.TakeDamage(new DamageData(gameObject, health.CurrentHealth, Vector2.zero, DamageType.Explosion));
    }

    void OnDrawGizmos()
    {
        DrawGizmos.Circle(transform.position, explosionRadius, Color.red);
    }
}
