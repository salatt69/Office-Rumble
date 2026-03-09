using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeatlhBarUIDocumentBinder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] UIDocument document;
    [SerializeField] Health health;

    readonly List<VisualElement> hearts = new();

    VisualElement container;

    void Awake()
    {
        if (!document)
            document = GetComponent<UIDocument>();

        if (!health)
            health = FindFirstObjectByType<Health>();
    }

    void OnEnable()
    {
        if (!document || !health) return;

        container = document.rootVisualElement.Q<VisualElement>("Container");
        if (container == null)
        {
            Debug.LogError("HealthBarUI: Could not find Container in UIDocument.");
            return;
        }
        container.Clear(); // removes all preview children

        health.OnHealthChanged += RefreshHearts;

        BuildHeartsFromMaxHealth();
    }

    void OnDisable()
    {
        if (health != null)
            health.OnHealthChanged -= RefreshHearts;
    }

    void BuildHeartsFromMaxHealth()
    {
        container.Clear();
        hearts.Clear();

        int maxHp = Mathf.CeilToInt(health.MaxHealth);

        for (int i = 0; i < maxHp; i++)
        {
            var heart = new VisualElement();
            heart.AddToClassList("heart");
            heart.AddToClassList("heart-full");

            container.Add(heart);
            hearts.Add(heart);
        }

        RefreshHearts(health.CurrentHealth, health.MaxHealth);
    }

    void RefreshHearts(float currentHealth, float maxHealth)
    {
        int current = Mathf.CeilToInt(currentHealth);
        int max = Mathf.CeilToInt(maxHealth);

        if (hearts.Count != max)
            BuildHeartsFromMaxHealth();

        for (int i = 0; i < hearts.Count; i++)
        {
            bool isFull = i < current;

            hearts[i].EnableInClassList("heart-full", isFull);
            hearts[i].EnableInClassList("heart-empty", !isFull);
        }
    }
}