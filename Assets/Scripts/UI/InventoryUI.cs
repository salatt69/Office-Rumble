using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private ItemSlot[] slots;

    void Awake()
    {
        if (slots == null || slots.Length == 0)
            slots = GetComponentsInChildren<ItemSlot>();
    }

    void OnEnable()
    {
        if (inventorySystem == null) return;

        inventorySystem.OnSlotChanged.AddListener(UpdateSlot);
        inventorySystem.OnSelectedSlotChanged.AddListener(UpdateSelection);

        // initialize UI with current state
        for (int i = 0; i < slots.Length; i++)
        {
            var data = inventorySystem.Slots[i];
            slots[i].SetItem(data);
        }

        UpdateSelection(inventorySystem.SelectedIndex);
    }

    void OnDisable()
    {
        if (inventorySystem == null) return;

        inventorySystem.OnSlotChanged.RemoveListener(UpdateSlot);
        inventorySystem.OnSelectedSlotChanged.RemoveListener(UpdateSelection);
    }

    void UpdateSlot(int slotIndex, ItemData data)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;
        slots[slotIndex].SetItem(data);
    }

void UpdateSelection(int selectedIndex)
{
    for (int i = 0; i < slots.Length; i++)
        slots[i].SetSelected(i == selectedIndex);
}
}
