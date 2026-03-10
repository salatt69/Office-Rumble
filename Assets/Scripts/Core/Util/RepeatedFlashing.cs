using UnityEngine;
using UnityEngine.UI;

public class RepeatedFlashing : MonoBehaviour
{
    [SerializeField] float flashingSpeed = 4f;
    [SerializeField] Color flashingColor = Color.white;
    [SerializeField] bool flashingEnabled;

    SpriteRenderer spriteRenderer;
    Image image;

    Color initColor;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
        image = GetComponentInChildren<Image>(true);

        if (spriteRenderer)
            initColor = spriteRenderer.color;
        else if (image)
            initColor = image.color;
        else
            Debug.LogWarning($"{name}: RepeatedFlashing requires SpriteRenderer or Image.");
    }

    void Update()
    {
        if (!spriteRenderer && !image)
            return;

        if (flashingEnabled)
        {
            float t = (Mathf.Sin(Time.time * flashingSpeed) + 1f) * 0.5f;
            Color flash = Color.Lerp(initColor, flashingColor, t);
            ApplyColor(flash);
        }
        else
        {
            ApplyColor(initColor);
        }
    }

    void ApplyColor(Color c)
    {
        if (spriteRenderer)
            spriteRenderer.color = c;

        if (image)
            image.color = c;
    }

    public void SetFlashing(bool enabled)
    {
        flashingEnabled = enabled;

        if (!enabled)
            ApplyColor(initColor);
    }
}