using TMPro;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [Header("Item")]
    [SerializeField] bool requiresPurchase;

    ItemData data;
    Item itemRef;
    GameObject priceTagPrefab;
    GameObject priceTagInstance;
    TMP_Text priceText;
    Transform vfx;
    GameObject spawnedVfx;

    public ItemData Data => data;
    public bool RequiresPurchase => requiresPurchase;
    public int Price => data.price;

    void Awake()
    {
        itemRef = GetComponentInParent<Item>();

        vfx = itemRef?.VFXChild;
        data = GetItemDataFromRoot();

        priceTagPrefab = Resources.Load<GameObject>("Prefabs/UI/PriceTag");
        priceTagInstance = Instantiate(priceTagPrefab, transform);

        if (priceTagInstance)
        {
            priceTagInstance.transform.localPosition = new Vector3(0f, -0.3f, 0f);
            priceText = priceTagInstance.GetComponentInChildren<TMP_Text>();
        }
        SpawnVFX();

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
            priceText.text = requiresPurchase ? data?.price.ToString() : "";
    }

    void ShowPickupNotification()
    {
        if (data is ConsumableData consumableData)
        {
            var notifier = Object.FindAnyObjectByType<ConsumableNotificationUI>();
            if (notifier != null)
            {
                string effects = "";
                foreach (var b in consumableData.buffs)
                {
                    if (b)
                    {
                        effects += (string.IsNullOrEmpty(effects) ? "" : ", ") + b.GetDescription();
                    }
                }
                notifier.Show(consumableData, effects);
            }
        }
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
            ShowPickupNotification();
            inventory.SelectSlot(inventory.SelectedIndex);
        }
        else
        {
            inventory.Drop(player.GetItemDropPosition());
            inventory.ReplaceSelected(data);
        }

        player.AddItemPickupCooldown();

        if (spawnedVfx != null)
        {
            var pss = spawnedVfx.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in pss)
            {
                if (ps != null)
                {
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
        }

        Destroy(transform.parent.gameObject);
    }

    void SpawnVFX()
    {
        if (data == null || vfx == null) return;

        string vfxName = data.tier switch
        {
            Tier.Common => "Prefabs/VFX/Tier_Common",
            Tier.Rare => "Prefabs/VFX/Tier_Rare",
            Tier.Epic => "Prefabs/VFX/Tier_Epic",
            Tier.Legendary => "Prefabs/VFX/Tier_Legendary",
            _ => null
        };

        if (vfxName == null) return;

        GameObject vfxPrefab = Resources.Load<GameObject>(vfxName);
        if (vfxPrefab != null)
        {
            spawnedVfx = Instantiate(vfxPrefab, vfx.position, Quaternion.identity, vfx);
        }
    }

    private ItemData GetItemDataFromRoot()
    {
        var itemComp = GetComponentInParent<Item>();
        if (itemComp != null)
        {
            return itemComp.Data;
        }
        else
        {
            Debug.LogWarning("No Item Component found in parent");
            return null;
        }
    }
}