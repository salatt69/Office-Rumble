using TMPro;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [Header("Item")]
    [SerializeField] ItemData data;
    [SerializeField] bool requiresPurchase;

    GameObject priceTagPrefab;
    GameObject priceTagInstance;
    TMP_Text priceText;

    public ItemData Data => data;
    public bool RequiresPurchase => requiresPurchase;
    public int Price => data.price;

    void Awake()
    {
        priceTagPrefab = Resources.Load<GameObject>("Prefabs/UI/PriceTag");
        priceTagInstance = Instantiate(priceTagPrefab, transform);

        if (priceTagInstance)
        {
            priceTagInstance.transform.localPosition = new Vector3(0f, -0.3f, 0f);
            priceText = priceTagInstance.GetComponentInChildren<TMP_Text>();
        }

        RefreshVisuals();
    }

    public void SetFreeItem(bool isFreeItem)
    {
        requiresPurchase = !isFreeItem;
        RefreshVisuals();
    }

    void RefreshVisuals()
    {
        if (priceTagInstance)
            priceTagInstance.SetActive(requiresPurchase);

        if (priceText)
            priceText.text = requiresPurchase ? data.price.ToString() : "";
    }

    public void TryInteract(PlayerController player)
    {
        if (!player.CanPickup)
        {
            Debug.Log("Player cannot pick up items right now.");
            return;
        }

        if (data == null)
        {
            Debug.LogWarning("ItemPickup has no ItemData assigned.");
            return;
        }

        var inventory = player.GetComponent<Inventory>();
        if (!inventory)
        {
            Debug.LogWarning("Player has no Inventory.");
            return;
        }

        if (requiresPurchase)
        {
            var wallet = player.GetComponent<PlayerWallet>();
            if (!wallet)
            {
                Debug.LogWarning("Player has no PlayerMoney component.");
                return;
            }

            if (!wallet.TrySpend(data.price))
            {
                Debug.Log("Not enough money.");
                return;
            }
        }

        if (inventory.Add(data))
        {
            inventory.SelectSlot(inventory.SelectedIndex);
        }
        else
        {
            inventory.Drop(player.GetItemDropPosition());
            inventory.ReplaceSelected(data);
        }

        player.AddItemPickupCooldown();
        Destroy(transform.parent.gameObject);
    }
}