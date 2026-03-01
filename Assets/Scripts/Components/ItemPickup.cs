using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] ItemData data;

    public void TryInteract(PlayerController player)
    {
        if (player.canPickup)
        {
            var inventory = player.GetComponent<Inventory>();

            if (inventory.Add(data))
            {
                //inventory.SelectSlot(inventory.SelectedIndex);
            }
            else
            {
                inventory.Drop(player.GetItemDropPosition());
                inventory.ReplaceSelected(data);
            }
            player.AddItemPickupCooldown();

            Destroy(transform.parent.gameObject);
        }
        else
        {
            Debug.Log("Player cannot pick up items right now.");
        }
    }
}
