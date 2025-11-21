using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemName;
    [SerializeField] Sprite emptySlot;

    public void SetItem(ItemData data)
    {
        if (data != null)
        {
            itemName.enabled = true;

            itemIcon.sprite = data.icon;
            itemName.text = data.itemName;
        }
        else
        {
            itemName.enabled = false;
            
            itemIcon.sprite = emptySlot;
            itemName.text = null;
        }
    }

    public void SetSelected(bool selected)
    {
        float alpha = selected ? 1f : 0.35f;

        var nameColor = itemName.color;
        nameColor.a = alpha;
        itemName.color = nameColor;

        var iconColor = itemIcon.color;
        iconColor.a = alpha;
        itemIcon.color = iconColor;
    }
}
