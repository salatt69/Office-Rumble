using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ColorUtility;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UIDocument))]
public class ScrapDisplayUIDocumentBinder : MonoBehaviour
{
    [SerializeField] NotificationSettings settings;

    UIDocument document;
    PlayerWallet wallet;

    Label value;
    Label deltaLabel;

    int prevMoney;
    int delta;

    readonly Dictionary<Label, Coroutine> fadeCoroutines = new();

    void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    public void Bind(PlayerWallet wallet)
    {
        if (this.wallet != null)
            this.wallet.OnMoneyChanged -= OnMoneyChanged;

        this.wallet = wallet;

        if (isActiveAndEnabled && this.wallet != null)
        {
            this.wallet.OnMoneyChanged += OnMoneyChanged;
            prevMoney = this.wallet.Money;
        }

        Refresh();
    }

    void OnEnable()
    {
        var root = document.rootVisualElement;
        value = root.Q<Label>("Value");
        deltaLabel = root.Q<Label>("Delta");

        if (wallet != null)
        {
            wallet.OnMoneyChanged += OnMoneyChanged;
            prevMoney = wallet.Money;
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (wallet != null)
            wallet.OnMoneyChanged -= OnMoneyChanged;
    }

    void OnMoneyChanged()
    {
        if (wallet == null) return;

        delta = wallet.Money - prevMoney;
        prevMoney = wallet.Money;
        Refresh();
    }

    void Refresh()
    {
        if (wallet == null) return;

        value.text = wallet.Money.ToString("0");

        UpdateDelta(deltaLabel, delta);
        delta = 0;
    }

    void UpdateDelta(Label deltaLabel, int delta)
    {
        if (fadeCoroutines.TryGetValue(deltaLabel, out var existing) && existing != null)
            StopCoroutine(existing);

        if (delta == 0)
        {
            deltaLabel.style.opacity = new StyleFloat(0f);
            fadeCoroutines[deltaLabel] = null;
            return;
        }

        deltaLabel.style.display = DisplayStyle.Flex;
        deltaLabel.style.opacity = new StyleFloat(1f);

        bool isIncrease = delta > 0;
        deltaLabel.text = $"{(isIncrease ? "+" : "")}{delta}";

        string colorHex = isIncrease ? "#00FF00" : "#FF0000";
        if (TryParseHtmlString(colorHex, out Color color))
        {
            color.a = 0.5019608f;
            deltaLabel.style.color = color;
        }

        fadeCoroutines[deltaLabel] = StartCoroutine(HideDeltaAfterDelay(deltaLabel));
    }

    IEnumerator HideDeltaAfterDelay(Label deltaLabel)
    {
        yield return new WaitForSeconds(settings.displayDuration);

        float elapsed = 0f;
        while (elapsed < settings.fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / settings.fadeOutDuration;
            deltaLabel.style.opacity = new StyleFloat(Mathf.Lerp(1f, 0f, t));
            yield return null;
        }

        deltaLabel.style.display = DisplayStyle.None;
        deltaLabel.style.opacity = new StyleFloat(1f);
        fadeCoroutines[deltaLabel] = null;
    }
}