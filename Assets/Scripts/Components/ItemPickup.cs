using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] ItemData data;

    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public string GetInteractionText() => $"Pick up {data.itemName}";

    public void Interact(PlayerController player)
    {
        var inventory = player.GetComponent<InventorySystem>();
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
        animator.SetBool("Interactable", state);
    }
}
