using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityBody))]
public class Health : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] bool setFullHealthOnAwake = true;
    [SerializeField] bool keepHealthRatioOnMaxHealthChange = true;

    [Header("Colliders")]
    [SerializeField] HurtboxGroup hurtboxGroup;

    [Header("I-Frames Settings")]
    [SerializeField] bool haveIFrames = true;
    [SerializeField] float invulnerabilityTime = 0.5f;
    [SerializeField] float flickerInterval = 0.1f;
    [SerializeField] bool useAlphaFlicker = true;

    GameObject objectRoot;
    DamageFlash damageFlash;
    EntityBody body;

    float currentHealth;
    bool isInvulnerable;

    Coroutine iFrameRoutine;

    public bool IsAlive => currentHealth > 0f;

    public System.Action<float, float> OnHealthChanged; // (current, max)
    public System.Action OnDied;

    float LastMaxHealth { get; set; }

    void Awake()
    {
        body = GetComponent<EntityBody>();

        objectRoot = transform.root?.gameObject ?? gameObject;
        damageFlash = objectRoot.GetComponent<DamageFlash>();

        float max = GetMaxHealth();

        if (setFullHealthOnAwake)
            currentHealth = max;
        else
            currentHealth = Mathf.Clamp(currentHealth, 0f, max);

        LastMaxHealth = max;
        OnHealthChanged?.Invoke(currentHealth, max);
    }

    void Update()
    {
        if (!IsAlive || body == null) return;

        // Regen
        float regen = body.HealthRegen;
        if (regen != 0f)
        {
            float max = GetMaxHealth();
            currentHealth = Mathf.Min(max, currentHealth + regen * Time.deltaTime);
        }

        // If max health changes due to upgrades/buffs, decide how to adjust current health
        float newMax = GetMaxHealth();
        if (!Mathf.Approximately(newMax, LastMaxHealth))
        {
            if (keepHealthRatioOnMaxHealthChange && LastMaxHealth > 0.0001f)
            {
                float ratio = currentHealth / LastMaxHealth;
                currentHealth = Mathf.Clamp(ratio * newMax, 0f, newMax);
            }
            else
            {
                currentHealth = Mathf.Clamp(currentHealth, 0f, newMax);
            }

            LastMaxHealth = newMax;
            OnHealthChanged?.Invoke(currentHealth, newMax);
        }
    }

    float GetMaxHealth()
    {
        // EntityBody is the source of truth
        return body ? Mathf.Max(1f, body.MaxHealth) : 1f;
    }

    public bool IsHurtbox(Collider2D col)
    {
        if (hurtboxGroup == null || col == null) return false;

        return hurtboxGroup.CompareColliderToHurtbox(col);
    }

    public void TakeDamage(DamageData damageData)
    {
        if (!IsAlive || isInvulnerable) return;

        // Flash
        damageFlash?.Flash();

        // Knockback direction (away from source)
        if (damageData.source != null)
            damageData.direction = (Vector2)(objectRoot.transform.position - damageData.source.transform.position);

        EffectSystem.Knockback(objectRoot, damageData.NormalizedDirection, damageData.knockbackForce);

        // Optional: player-only knockback lock
        if (objectRoot.TryGetComponent(out PlayerController controller))
            controller.ApplyKnockbackLock(0.2f);

        float max = GetMaxHealth();
        currentHealth = Mathf.Max(0f, currentHealth - damageData.amount);
        OnHealthChanged?.Invoke(currentHealth, max);

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        if (haveIFrames)
        {
            if (iFrameRoutine != null)
                StopCoroutine(iFrameRoutine);

            iFrameRoutine = StartCoroutine(InvulnerabilityCoroutine());
        }
    }

    IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;

        float elapsed = 0f;

        // Cache original alpha only (never touch RGB)
        var originalAlpha = new Dictionary<SpriteRenderer, float>();
        SpriteRenderer[] renderers = objectRoot.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (var r in renderers)
        {
            if (r == null) continue;
            originalAlpha[r] = r.color.a;

            // Ensure FULL opacity at start
            if (useAlphaFlicker)
            {
                Color c = r.color;
                c.a = originalAlpha[r];
                r.color = c;
            }
            else
            {
                r.enabled = true;
            }
        }

        yield return new WaitForSecondsRealtime(flickerInterval);
        elapsed += flickerInterval;

        bool visiblePhase = false;

        while (elapsed < invulnerabilityTime)
        {
            visiblePhase = !visiblePhase;

            renderers = objectRoot.GetComponentsInChildren<SpriteRenderer>(true);

            foreach (var r in renderers)
            {
                if (r == null) continue;

                if (!originalAlpha.ContainsKey(r))
                    originalAlpha[r] = r.color.a;

                if (useAlphaFlicker)
                {
                    Color c = r.color;
                    float a0 = originalAlpha[r];
                    c.a = visiblePhase ? a0 : a0 * 0.25f;
                    r.color = c;
                }
                else
                {
                    r.enabled = visiblePhase;
                }
            }

            elapsed += flickerInterval;
            yield return new WaitForSecondsRealtime(flickerInterval);
        }

        // Restore alpha / enabled
        renderers = objectRoot.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (var r in renderers)
        {
            if (r == null) continue;

            if (useAlphaFlicker)
            {
                Color c = r.color;
                c.a = originalAlpha.TryGetValue(r, out float a0) ? a0 : 1f;
                r.color = c;
            }
            else
            {
                r.enabled = true;
            }
        }

        isInvulnerable = false;
    }

    public void Heal(float amount)
    {
        if (!IsAlive) return;

        float max = GetMaxHealth();
        currentHealth = Mathf.Min(max, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, max);
    }

    public void SetHealth(float newHealth)
    {
        float max = GetMaxHealth();
        currentHealth = Mathf.Clamp(newHealth, 0f, max);
        OnHealthChanged?.Invoke(currentHealth, max);
    }

    void Die()
    {
        OnDied?.Invoke();
        Debug.Log($"{name} died!");
    }
}