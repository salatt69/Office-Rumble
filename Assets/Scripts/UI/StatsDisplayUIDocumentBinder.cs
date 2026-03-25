using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ColorUtility;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(UIDocument))]
public class StatsDisplayUIDocumentBinder : MonoBehaviour
{
    [SerializeField] NotificationSettings settings;

    UIDocument document;
    EntityBody body;

    Label damageValue;
    Label moveSpeedValue;
    Label attackSpeedValue;
    Label critChanceValue;
    Label critMultValue;

    Label damageDelta;
    Label moveSpeedDelta;
    Label attackSpeedDelta;
    Label critChanceDelta;
    Label critMultDelta;

    readonly Dictionary<Label, Coroutine> fadeCoroutines = new();

    void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    public void Bind(EntityBody body)
    {
        if (this.body != null)
            this.body.OnStatsChanged -= Refresh;

        this.body = body;

        if (isActiveAndEnabled && this.body != null)
            this.body.OnStatsChanged += Refresh;

        Refresh();
    }

    void OnEnable()
    {
        var root = document.rootVisualElement;

        damageValue = root.Q<Label>("Value_Damage");
        moveSpeedValue = root.Q<Label>("Value_MoveSpeed");
        attackSpeedValue = root.Q<Label>("Value_AttackSpeed");
        critChanceValue = root.Q<Label>("Value_CritChance");
        critMultValue = root.Q<Label>("Value_CritMult");

        damageDelta = root.Q<Label>("Delta_Damage");
        moveSpeedDelta = root.Q<Label>("Delta_MoveSpeed");
        attackSpeedDelta = root.Q<Label>("Delta_AttackSpeed");
        critChanceDelta = root.Q<Label>("Delta_CritChance");
        critMultDelta = root.Q<Label>("Delta_CritMult");

        if (body != null)
            body.OnStatsChanged += Refresh;

        Refresh();
    }

    void OnDisable()
    {
        if (body != null)
            body.OnStatsChanged -= Refresh;
    }

    void Refresh()
    {
        if (body == null) return;

        damageValue.text = body.Damage.ToString("0.0");
        moveSpeedValue.text = body.MoveSpeed.ToString("0.0");
        attackSpeedValue.text = body.AttackSpeed.ToString("0.00");
        critChanceValue.text = body.CritChance.ToString("0.00");
        critMultValue.text = body.CritDamage.ToString("0.00");

        UpdateDelta(damageDelta, body.Damage - body.BaseDamage);
        UpdateDelta(moveSpeedDelta, body.MoveSpeed - body.BaseMoveSpeed);
        UpdateDelta(attackSpeedDelta, body.AttackSpeed - body.BaseAttackSpeed);
        UpdateDelta(critChanceDelta, body.CritChance - body.BaseCritChance);
        UpdateDelta(critMultDelta, body.CritDamage - body.BaseCritDamage);
    }

    void UpdateDelta(Label deltaLabel, float delta)
    {
        if (fadeCoroutines.TryGetValue(deltaLabel, out var existing) && existing != null)
            StopCoroutine(existing);

        if (Mathf.Approximately(delta, 0f))
        {
            deltaLabel.style.display = DisplayStyle.None;
            fadeCoroutines[deltaLabel] = null;
            return;
        }

        deltaLabel.style.display = DisplayStyle.Flex;
        deltaLabel.style.opacity = new StyleFloat(1f);
        
        bool isIncrease = delta > 0;
        string sign = isIncrease ? "+" : "";
        deltaLabel.text = $"{sign}{delta:0.0}";
        
        string colorHex = isIncrease ? "#00FF00" : "#FF0000";
        if (TryParseHtmlString(colorHex, out Color color))
        {
            color.a = 0.5019608f;
            deltaLabel.style.color = color;
        }

        fadeCoroutines[deltaLabel] = StartCoroutine(HideDeltaAfterDelay(deltaLabel, settings.displayDuration));
    }

    IEnumerator HideDeltaAfterDelay(Label deltaLabel, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        float elapsed = 0f;
        float fadeDuration = settings.fadeOutDuration;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            deltaLabel.style.opacity = new StyleFloat(Mathf.Lerp(1f, 0f, t));
            yield return null;
        }
        
        deltaLabel.style.display = DisplayStyle.None;
        deltaLabel.style.opacity = new StyleFloat(1f);
        fadeCoroutines[deltaLabel] = null;
    }
}