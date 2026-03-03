using UnityEngine;

public class ItemHighlight : MonoBehaviour, IHighlight
{
    [SerializeField] ItemData data;
    [SerializeField] SpriteRenderer spriteRenderer;

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
}
