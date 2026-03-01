using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Header("Basics")]
    [SerializeField] float speed;
    [SerializeField] float damage;
    [SerializeField] float knockbackMultiplier = 1f;
    [SerializeField] float lifetime;

    [Header("Collision")]
    [SerializeField] bool destroyOnWallHit = true;
    [SerializeField] LayerMask wallMask;

    [Header("Appearance")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] sprites;
    [SerializeField] GameObject deathEffect;
    [SerializeField] ParticleSystem trailParticles;

    [Header("Motion (Optional)")]
    [SerializeField] bool useSpeedCurve = false;
    [SerializeField] AnimationCurve speedOverLife = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.2f);

    Rigidbody2D rb;
    Collider2D col;

    GameObject owner;
    int ownerLayer;

    DamageData damageData;

    float dieAt;
    float spawnTime;
    float usedLifetime;

    bool initialized;
    bool dying;

    Vector2 moveDir;
    float initialSpeed;

    public float Speed
    {
        get => speed;
        set => speed = Mathf.Max(0f, value);
    }

    public float Lifetime
    {
        get => lifetime;
        set => lifetime = Mathf.Max(0.01f, value);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
    }

    public void Init(GameObject owner, Vector2 direction, float? speedOverride = null, float? lifetimeOverride = null)
    {
        float finalSpeed = speedOverride ?? speed;
        float finalLifetime = lifetimeOverride ?? lifetime;

        this.owner = owner;
        ownerLayer = owner ? owner.layer : -1;

        Vector2 dir = direction.sqrMagnitude < 0.0001f ? Vector2.right : direction.normalized;

        damageData = new DamageData(owner, damage, dir, DamageType.Projectile)
        {
            knockbackForceMultiplier = knockbackMultiplier
        };

        if (spriteRenderer != null && sprites != null && sprites.Length > 0)
            spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        moveDir = dir;
        initialSpeed = finalSpeed;

        spawnTime = Time.time;
        usedLifetime = Mathf.Max(0.01f, finalLifetime);
        dieAt = spawnTime + usedLifetime;

        rb.linearVelocity = moveDir * initialSpeed;

        if (owner != null && col != null)
        {
            foreach (var c in owner.GetComponentsInChildren<Collider2D>())
                Physics2D.IgnoreCollision(col, c, true);
        }

        if (trailParticles != null)
        {
            if (trailParticles.transform.parent != transform)
            {
                var trail = Instantiate(trailParticles, transform);
                trail.transform.localPosition = Vector3.zero;
                trail.Play();
            }
        }

        initialized = true;
    }

    void Update()
    {
        if (!initialized || dying) return;

        if (Time.time >= dieAt)
        {
            DestroyProjectile();
            return;
        }

        if (useSpeedCurve)
        {
            float t = Mathf.Clamp01((Time.time - spawnTime) / usedLifetime); // 0..1
            float mult = speedOverLife.Evaluate(t);
            rb.linearVelocity = moveDir * (initialSpeed * mult);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!initialized || dying) return;

        if (owner && other.transform.IsChildOf(owner.transform))
            return;

        if (destroyOnWallHit && ((1 << other.gameObject.layer) & wallMask.value) != 0)
        {
            DestroyProjectile();
            return;
        }

        if (ownerLayer != -1 && other.gameObject.layer == ownerLayer)
            return;

        var damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageData.direction = rb.linearVelocity;
            damageable.TakeDamage(damageData);
            DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
        if (dying) return;
        dying = true;

        rb.linearVelocity = Vector2.zero;
        col.enabled = false;

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, transform.rotation);

        DetachAndStopParticles();
        Destroy(gameObject);
    }

    void DetachAndStopParticles()
    {
        var systems = GetComponentsInChildren<ParticleSystem>(true);

        for (int i = 0; i < systems.Length; i++)
        {
            var ps = systems[i];
            if (!ps) continue;

            ps.transform.SetParent(null, true);
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            var main = ps.main;
            float killAfter = main.duration + main.startLifetime.constantMax;
            if (main.loop) killAfter = main.startLifetime.constantMax;

            Destroy(ps.gameObject, Mathf.Max(0.05f, killAfter));
        }
    }
}