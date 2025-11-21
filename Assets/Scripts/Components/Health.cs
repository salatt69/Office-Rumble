using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] float maxHealth = 100f;

    [Header("I-Frames Settings")]
    [SerializeField] float invulnerabilityTime = 0.5f;
    [SerializeField] float flickerInterval = 0.1f;
    [Tooltip("If true, use alpha flicker (recommended). If false, will toggle SpriteRenderer.enabled.")]
    [SerializeField] bool useAlphaFlicker = true;

    [Header("References")]
    [Tooltip("Assign the root player object manually if not found automatically.")]
    [SerializeField] GameObject playerRoot;

    float currentHealth;
    public bool IsAlive => currentHealth > 0;
    public System.Action<float, float> OnHealthChanged;
    public System.Action OnDied;

    private bool isInvulnerable = false;
    private bool debugLoggedThisFlicker = false;

    void Awake()
    {
        currentHealth = maxHealth;

        if (playerRoot == null)
            playerRoot = transform.root?.gameObject ?? gameObject;
    }

    public void TakeDamage(DamageData damageData)
    {
        if (!IsAlive || isInvulnerable) return;



        EffectSystem.FreezeFrame(0.25f);

        damageData.direction = playerRoot.transform.position - damageData.source.transform.position;

        EffectSystem.Knockback(playerRoot, damageData.NormalizedDirection, damageData.knockbackForce);
        
        if (playerRoot.TryGetComponent(out PlayerController controller))
            controller.ApplyKnockbackLock(0.2f);

        currentHealth = Mathf.Max(0, currentHealth - damageData.amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"{name} took {damageData.amount} damage from "
            + $"{damageData.source?.name ?? "Unknown"}");

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(InvulnerabilityCoroutine());
    }
    IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        debugLoggedThisFlicker = false;

        float elapsed = 0f;
        bool visiblePhase = true;
        bool firstFrame = true;

        // Cache original colors so we can restore them exactly afterwards
        var originalColors = new Dictionary<SpriteRenderer, Color>();

        while (elapsed < invulnerabilityTime)
        {
            visiblePhase = !visiblePhase;

            // Re-query renderers every tick to include newly attached items (we intentionally include inactive)
            SpriteRenderer[] renderers = playerRoot.GetComponentsInChildren<SpriteRenderer>(true);

            if (!debugLoggedThisFlicker)
            {
                Debug.Log($"[Health] Flicker started for '{playerRoot.name}'. Found {renderers.Length} SpriteRenderers.");
                debugLoggedThisFlicker = true;
            }

            foreach (var r in renderers)
            {
                if (r == null) continue;

                // store original color once
                if (!originalColors.ContainsKey(r))
                    originalColors[r] = r.color;

                if (useAlphaFlicker)
                {
                    // safe alpha flicker: set alpha to either 1 or 0.25
                    Color c = r.color;
                    if (firstFrame)
                    {
                        c.g = .85f;
                        c.b = .85f;
                    }
                    else
                    {
                        float targetAlpha = visiblePhase ? originalColors[r].a : originalColors[r].a * 0.25f;
                        c.g = originalColors[r].g;
                        c.b = originalColors[r].b;
                        c.a = targetAlpha;
                    }
                    r.color = c;
                }
                else
                {
                    // fallback: toggle enabled (less smooth, but works)
                    r.enabled = visiblePhase;
                }
            }
            firstFrame = false;

            elapsed += flickerInterval;
            yield return new WaitForSeconds(flickerInterval);
        }

        // restore everything to original color / enabled state
        SpriteRenderer[] finalRenderers = playerRoot.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var r in finalRenderers)
        {
            if (r == null) continue;
            if (originalColors.TryGetValue(r, out var orig))
            {
                r.color = orig;
            }
            else
            {
                // If renderer appeared after we started flicker, ensure it's fully visible
                Color c = r.color;
                c.a = 1f;
                r.color = c;
            }

            if (!useAlphaFlicker)
                r.enabled = true;
        }

        isInvulnerable = false;
        yield break;
    }

    public void Heal(float amount)
    {
        if (!IsAlive) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"Healed for {amount}!");
    }

    private void StartInvisibility()
    {

    }

    private void Die()
    {
        OnDied?.Invoke();
        Debug.Log($"{name} died!");

        // TODO: Animations, disable components, blah blah blah
    }
}
