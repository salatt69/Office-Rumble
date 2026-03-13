using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class ScrapDisplayUIDocumentBinder : MonoBehaviour
{
    UIDocument document;
    PlayerWallet wallet;

    Label value;

    void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    public void Bind(PlayerWallet wallet)
    {
        if (this.wallet != null)
            this.wallet.OnMoneyChanged -= Refresh;

        this.wallet = wallet;

        if (isActiveAndEnabled && this.wallet != null)
            this.wallet.OnMoneyChanged += Refresh;

        Refresh();
    }

    void OnEnable()
    {
        var root = document.rootVisualElement;
        value = root.Q<Label>("Value");

        if (wallet != null)
            wallet.OnMoneyChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (wallet != null)
            wallet.OnMoneyChanged -= Refresh;
    }

    void Refresh()
    {
        if (wallet == null) return;

        value.text = wallet.Money.ToString("0");
    }
}