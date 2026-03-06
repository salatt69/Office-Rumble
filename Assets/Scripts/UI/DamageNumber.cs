using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    [Header("Lifetime / Motion")]
    [SerializeField] float lifetime = 0.8f;
    [SerializeField] float floatSpeed = 1.6f;
    [SerializeField] float randomX = 0.25f;
    [SerializeField] Gradient colorOverLifetime;

    [Header("Scale from damage")]
    [SerializeField] AnimationCurve dmgAmountToScaleRelationCurve;
    [Tooltip("Damage value that maps to curve time=0")]
    [SerializeField] float minDamage = 0f;
    [Tooltip("Damage value that maps to curve time=1")]
    [SerializeField] float maxDamage = 1000f;
    [Tooltip("Extra multiplier on top of curve output")]
    [SerializeField] float baseScale = 1f;
    [SerializeField] float minScale = 0.0015f;
    [SerializeField] float maxScale = 0.005f;

    public Vector2 spawnJitter = new(0.15f, 0.15f);

    float spawnTime;
    float dieAt;
    Vector3 vel;

    void Awake()
    {
        if (!text)
        {
            text = GetComponentInChildren<TMP_Text>(true);
        }
    }

    public void Init(float amount)
    {
        text.text = amount.ToString();

        spawnTime = Time.time;
        dieAt = spawnTime + lifetime;

        vel = new Vector3(Random.Range(-randomX, randomX), 1f, 0f) * floatSpeed;

        // Normalize damage to 0..1
        float t = Mathf.InverseLerp(minDamage, maxDamage, amount);
        float curveValue = dmgAmountToScaleRelationCurve.Evaluate(t);
        float scale = Mathf.Lerp(minScale, maxScale, curveValue) * baseScale;
        transform.localScale = Vector3.one * scale;
    }

    void Update()
    {
        transform.position += vel * Time.deltaTime;

        float lifeT = (Time.time - spawnTime) / lifetime;

        text.color = colorOverLifetime.Evaluate(lifeT);

        if (Time.time >= dieAt)
            Destroy(gameObject);
    }
}