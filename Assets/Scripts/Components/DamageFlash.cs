using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] Color flashColor = Color.red;
    [SerializeField] float flashDuration = 0.1f;

    SpriteRenderer[] renderers;
    Color[] originalColors;
    float flashEndTime;
    bool flashing;

    void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].color;
    }

    public void Flash()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].color = flashColor;

        flashEndTime = Time.time + flashDuration;
        flashing = true;
    }

    void Update()
    {
        if (!flashing) return;

        if (Time.time >= flashEndTime)
        {
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].color = originalColors[i];

            flashing = false;
        }
    }
}