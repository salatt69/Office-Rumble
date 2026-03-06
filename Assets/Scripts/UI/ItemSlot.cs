using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image itemIcon;
    [SerializeField] TMP_Text usageText;

    Sprite EmptySlotIcon => Resources.Load<Sprite>("Art/UI/OR_EmptyItemSlot");
    CanvasGroup canvasGroup;
    float canvasGroupAlphaValue;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroupAlphaValue = canvasGroup.alpha;
    }

    public void SetItem(ItemData data)
    {
        itemIcon.sprite = data != null ? data.icon : EmptySlotIcon;
        usageText.text = "";
    }

    public void SetUses(ItemData data, int usesLeft)
    {
        if (data is ConsumableData && usesLeft > 1)
            usageText.text = usesLeft.ToString();
        else
            usageText.text = "";
    }

    public void SetSelected(bool selected)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = selected ? 1f : canvasGroupAlphaValue;
        }
    }
}