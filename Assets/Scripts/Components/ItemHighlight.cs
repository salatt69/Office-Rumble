using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
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
        spriteRenderer.sprite = state ? data.highlightedUnequipped : data.unequipped;
    }
}
