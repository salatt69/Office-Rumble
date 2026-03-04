using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    public Transform target { get; private set; }
    public float distanceToTarget { get; private set; }

    [SerializeField] float acquireRadius = 12f;
    [SerializeField] LayerMask targetMask;

    void Update()
    {
        if (target == null)
        {
            var hit = Physics2D.OverlapCircle(transform.position, acquireRadius, targetMask);
            if (hit) target = hit.transform;
        }

        if (target != null)
            distanceToTarget = Vector2.Distance(transform.position, target.position);
    }

    public void ForgetTarget() => target = null;
}