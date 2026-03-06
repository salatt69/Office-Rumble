using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Inventory inventorySystem;
    [SerializeField] ItemSlot[] slots;

    void Awake()
    {
        if (slots == null || slots.Length == 0)
            slots = GetComponentsInChildren<ItemSlot>();
    }

    void OnEnable()
    {
        if (inventorySystem == null) return;

        inventorySystem.OnSlotChanged.AddListener(UpdateSlot);
        inventorySystem.OnSlotUsesChanged.AddListener(UpdateSlotUses);
        inventorySystem.OnSelectedSlotChanged.AddListener(UpdateSelection);

        for (int i = 0; i < slots.Length; i++)
        {
            var slot = inventorySystem.Slots[i];
            slots[i].SetItem(slot.data);
            slots[i].SetUses(slot.data, slot.uses);
        }

        UpdateSelection(inventorySystem.SelectedIndex);
    }

    void OnDisable()
    {
        if (inventorySystem == null) return;

        inventorySystem.OnSlotChanged.RemoveListener(UpdateSlot);
        inventorySystem.OnSlotUsesChanged.RemoveListener(UpdateSlotUses);
        inventorySystem.OnSelectedSlotChanged.RemoveListener(UpdateSelection);
    }

    void UpdateSlot(int slotIndex, ItemData data)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;

        slots[slotIndex].SetItem(data);

        var invSlot = inventorySystem.Slots[slotIndex];
        slots[slotIndex].SetUses(invSlot.data, invSlot.uses);
    }

    void UpdateSlotUses(int slotIndex, int usesLeft)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;

        var invSlot = inventorySystem.Slots[slotIndex];
        slots[slotIndex].SetUses(invSlot.data, usesLeft);
    }

    void UpdateSelection(int selectedIndex)
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].SetSelected(i == selectedIndex);
    }
}