using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    public Transform target { get; private set; }
    public float distanceToTarget { get; private set; }

    [SerializeField] LayerMask targetMask;
    [SerializeField] float acquireRadius = 12f;
    [SerializeField] float loseRadius = 15f;
    [SerializeField] float attackRadius = 5f;
    [SerializeField] bool drawGizmos = false;

    public float AcquireRadius => acquireRadius;
    public float LoseRadius => loseRadius;
    public float AttackRadius => attackRadius;

    public bool TargetInAttackRange => target != null && distanceToTarget <= attackRadius;

    void Update()
    {
        if (target == null)
        {
            var hit = Physics2D.OverlapCircle(transform.position, acquireRadius, targetMask);
            if (hit) target = hit.transform;
        }

        if (target != null)
        {
            distanceToTarget = Vector2.Distance(transform.position, target.position);

            if (distanceToTarget > loseRadius)
                ForgetTarget();
        }
    }

    public void ForgetTarget() => target = null;

    void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            DrawGizmos.Circle(transform.position, acquireRadius, Color.cyan);
            DrawGizmos.Circle(transform.position, loseRadius, Color.blue);
            DrawGizmos.Circle(transform.position, attackRadius, Color.green);
        }
    }
}