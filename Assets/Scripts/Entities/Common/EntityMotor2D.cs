using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EntityBody))]
public class EntityMotor2D : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] EntityBody body;

    Vector2 rawMoveInput;
    Vector2 smoothedMoveInput;
    Vector2 inputVelocityRef;

    float knockbackTimer;
    public bool IsKnockedBack => knockbackTimer > 0f;

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!body) body = GetComponent<EntityBody>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    public void SetMoveInput(Vector2 input)
    {
        rawMoveInput = Vector2.ClampMagnitude(input, 1f);
    }

    public void ApplyKnockbackLock(float duration)
    {
        knockbackTimer = Mathf.Max(knockbackTimer, duration);
        inputVelocityRef = Vector2.zero;
    }

    public void FixedTick()
    {
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            return;
        }

        float smoothTime = 1f / Mathf.Max(0.0001f, body.Acceleration);

        smoothedMoveInput = Vector2.SmoothDamp(
            smoothedMoveInput,
            rawMoveInput,
            ref inputVelocityRef,
            smoothTime,
            Mathf.Infinity,
            Time.fixedDeltaTime
        );

        // cannot exceed max input magnitude of 1 after smoothing
        smoothedMoveInput = Vector2.ClampMagnitude(smoothedMoveInput, 1f);

        if ((smoothedMoveInput - rawMoveInput).sqrMagnitude <= 0.0001f)
        {
            smoothedMoveInput = rawMoveInput;
            inputVelocityRef = Vector2.zero;
        }

        rb.linearVelocity = smoothedMoveInput * body.MoveSpeed;
    }
}