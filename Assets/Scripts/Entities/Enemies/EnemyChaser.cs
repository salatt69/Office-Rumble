using UnityEngine;

[RequireComponent(typeof(EntityBody))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChaser : MonoBehaviour
{
    [Header("Radius")]
    [SerializeField] float chaseRadius = 10f;
    public float Radius => chaseRadius;

    EntityBody body;
    Rigidbody2D rb;

    void Awake()
    {
        body = GetComponent<EntityBody>();
        rb = GetComponent<Rigidbody2D>();
    }

    public bool ChaseTarget(Transform target)
    {
        if (target == null) return false;

        Vector2 toTarget = (Vector2)target.position - rb.position;

        float moveSpeed = body.MoveSpeed;
        rb.linearVelocity = toTarget.normalized * moveSpeed;

        return true;
    }
}