using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    [SerializeField] GameObject playerReference;

    private Item currentItem;
    public Item CurrentItem => currentItem;

    public void Equip(Item newItem)
    {
        if (newItem == null)
        {
            Debug.LogWarning("Tried to equip null item.");
            return;
        }

        Unequip();

        // Parent the item under the holder
        currentItem = newItem;
        currentItem.transform.SetParent(transform);
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;

        currentItem.OnEquip();
    }

    public void Unequip()
    {
        if (currentItem == null)
            return;

        currentItem.OnUnequip();
        Destroy(currentItem.gameObject);
        currentItem = null;
    }

    public void UseCurrentItem()
    {
        if (currentItem is IUsable usable)
            usable.Use(playerReference);
        else
            Debug.Log($"{currentItem?.name ?? "Nothing"} is not usable.");
    }

    public void FlipY(bool flip)
    {
        if (currentItem != null && currentItem.TryGetComponent(out SpriteRenderer sr))
            sr.flipY = flip;
    }
}
