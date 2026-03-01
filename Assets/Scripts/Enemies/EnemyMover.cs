using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMover : MonoBehaviour
{
    [SerializeField] float moveSpeed = 4f;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    public void MoveTowards(Vector2 worldPos)
    {
        Vector2 dir = (worldPos - rb.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
    }

    public void MoveAwayFrom(Vector2 worldPos)
    {
        Vector2 dir = (rb.position - worldPos).normalized;
        rb.linearVelocity = dir * moveSpeed;
    }

    public void Stop()
    {
        rb.linearVelocity = Vector2.zero;
    }
}