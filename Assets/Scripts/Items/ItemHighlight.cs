using UnityEngine;

public class ItemHighlight : MonoBehaviour, IHighlight
{
    SpriteRenderer spriteRenderer;
    ItemData data;

    void Awake()
    {
        data = GetItemDataFromRoot();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
    }

    public string GetInteractionText()
    {
        throw new System.NotImplementedException();
    }

    public void SetHighlight(bool state)
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning("ItemHighlight: No SpriteRenderer assigned.", this);
            return;
        }
        spriteRenderer.sprite = state ? data.highlightedUnequipped : data.unequipped;
    }

    private ItemData GetItemDataFromRoot()
    {
        var itemComp = GetComponentInParent<Item>();
        if (itemComp != null)
        {
            return itemComp.Data;
        }
        else
        {
            Debug.LogWarning("No Item Component found in parent");
            return null;
        }
    }
}
