using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EntityBody))]
public class EntityMotor2D : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] EntityBody body;

    Vector2 moveInput;

    float knockbackTimer;
    public bool IsKnockedBack => knockbackTimer > 0f;

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!body) body = GetComponent<EntityBody>();
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = Vector2.ClampMagnitude(input, 1f);
    }

    public void ApplyKnockbackLock(float duration)
    {
        knockbackTimer = Mathf.Max(knockbackTimer, duration);
    }

    public void FixedTick()
    {
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            return;
        }

        rb.linearVelocity = moveInput * body.MoveSpeed;
    }
}