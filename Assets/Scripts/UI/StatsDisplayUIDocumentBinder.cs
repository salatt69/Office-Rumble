using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class StatsDisplayUIDocumentBinder : MonoBehaviour
{
    UIDocument document;
    EntityBody body;

    Label damageValue;
    Label moveSpeedValue;
    Label attackSpeedValue;
    Label critChanceValue;
    Label critMultValue;

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
    }
}