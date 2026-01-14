using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] ItemData data;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public string GetInteractionText() => $"Pick up {data.itemName}";

    public void Interact(PlayerController player)
    {
        var inventory = player.GetComponent<Inventory>();
        ItemData oldItem;

        if (inventory.Add(data))
        {
            inventory.SelectSlot(inventory.SelectedIndex);
        }
        else
        {
            oldItem = inventory.ReplaceSelected(data);
            Instantiate(oldItem.prefab, player.transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    public void SetHighlight(bool state)
    {
        spriteRenderer.sprite = state ? data.highlightedUnequipped : data.unequipped;
    }
}
