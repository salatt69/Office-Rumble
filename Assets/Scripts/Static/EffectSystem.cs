using UnityEngine;

public static class EffectSystem
{
    public static void Knockback(GameObject target, Vector2 direction, float force)
    {
        if (target.TryGetComponent(out Rigidbody2D rb))
        {
            // rb.linearVelocity = Vector2.zero;
            rb.AddForce(direction * force, ForceMode2D.Impulse);
        }
    }   

    public static void FreezeFrame(float duration)
    {
        GameManager.Instance?.FreezeFrame(duration);
    }

    public static void CameraShake(float intensity, float duration)
    {
        // TODO: Camera shake
    }

    // public static void ApplyBuff(GameObject target, BuffData buff)
    // {
    //     // TODO: modify stats, regen, etc.
    // }

    // public static void ApplyDebuff(GameObject target, DebuffData debuff)
    // {
    //     // TODO: slow, poison, etc.
    // }
}
