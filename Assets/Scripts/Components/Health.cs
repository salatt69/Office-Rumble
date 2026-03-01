using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] float maxHealth = 100f;

    [Header("I-Frames Settings")]
    [SerializeField] bool haveIFrames = true;
    [SerializeField] float invulnerabilityTime = 0.5f;
    [SerializeField] float flickerInterval = 0.1f;
    [SerializeField] bool useAlphaFlicker = true;

    [Header("References")]
    [SerializeField] GameObject objectRoot;

    private DamageFlash damageFlash;

    float currentHealth;
    bool isInvulnerable = false;

    public bool IsAlive => currentHealth > 0;
    public System.Action<float, float> OnHealthChanged;
    public System.Action OnDied;

    Coroutine iFrameRoutine;

    void Awake()
    {
        currentHealth = maxHealth;

        if (objectRoot == null)
            objectRoot = transform.root?.gameObject ?? gameObject;

        damageFlash = objectRoot.GetComponent<DamageFlash>()
                  ?? objectRoot.GetComponentInChildren<DamageFlash>(true);
    }

    public void TakeDamage(DamageData damageData)
    {
        if (!IsAlive || isInvulnerable) return;

        // Flash
        damageFlash?.Flash();

        // Knockback direction (away from source)
        if (damageData.source != null)
            damageData.direction = objectRoot.transform.position - damageData.source.transform.position;

        EffectSystem.Knockback(objectRoot, damageData.NormalizedDirection, damageData.knockbackForce);

        if (objectRoot.TryGetComponent(out PlayerController controller))
            controller.ApplyKnockbackLock(0.2f);

        currentHealth = Mathf.Max(0, currentHealth - damageData.amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        if (haveIFrames)
        {
            // Restart i-frames cleanly if hit again
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

            // 🔥 Ensure FULL opacity at start
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

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Die()
    {
        OnDied?.Invoke();
        Debug.Log($"{name} died!");
    }
}