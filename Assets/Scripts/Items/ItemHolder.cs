using UnityEngine;

public class ItemHolder : MonoBehaviour
{
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

        currentItem = newItem;
        currentItem.transform.SetParent(transform);
        currentItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
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

    public void FlipY(bool flip)
    {
        if (currentItem != null && currentItem.TryGetComponent(out SpriteRenderer sr))
            sr.flipY = flip;
    }
}