using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class HeatlhBarUIDocumentBinder : MonoBehaviour
{
    readonly List<VisualElement> hearts = new();

    UIDocument document;
    Health health;
    VisualElement container;

    void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    public void Bind(Health health)
    {
        if (this.health != null)
            this.health.OnHealthChanged -= RefreshHearts;

        this.health = health;

        if (!isActiveAndEnabled || this.health == null)
            return;

        this.health.OnHealthChanged += RefreshHearts;

        if (container == null && document != null)
            container = document.rootVisualElement.Q<VisualElement>("Container");

        if (container != null)
            BuildHeartsFromMaxHealth();
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