using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Item : MonoBehaviour
{
    protected ItemData data;
    protected SpriteRenderer spriteRenderer;
    protected ItemHolder holder;

    public ItemData Data => data;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void Initialize(ItemData newData)
    {
        data = newData;
    }

    public virtual void OnEquip()
    {
        if (spriteRenderer)
        {
            spriteRenderer.sortingLayerName = "Held Item";
            spriteRenderer.sprite = data.equipped;
        }
    }

    public virtual void OnUnequip()
    {
        if (spriteRenderer)
        {
            spriteRenderer.sortingLayerName = "Item";
            spriteRenderer.sprite = data.unequipped;
        }
    }
}
