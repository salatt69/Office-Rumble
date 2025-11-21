using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected ItemData data;
    protected SpriteRenderer sprite;
    protected Animator animator;
    protected ItemHolder holder;

    public ItemData Data => data;

    protected virtual void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public virtual void Initialize(ItemData newData)
    {
        data = newData;
    }

    public virtual void OnEquip()
    {
        if (sprite)
            sprite.sortingLayerName = "Held Item";

        if (animator)
            animator.SetBool("Equipped", true);

        Debug.Log($"[{data.itemName}] equipped.");
    }

    public virtual void OnUnequip()
    {
        if (sprite)
            sprite.sortingLayerName = "Item";

        if (animator)
            animator.SetBool("Equipped", false);

        Debug.Log($"[{data.itemName}] unequipped.");
    }
}
