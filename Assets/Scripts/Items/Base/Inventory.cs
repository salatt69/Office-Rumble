using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InventoryItemSlot
{
    public ItemData data;
    public int uses;
    public bool IsEmpty => data == null;
}

public class Inventory : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ItemHolder itemHolder;

    [Header("Events (UI)")]
    public UnityEvent<int, ItemData> OnSlotChanged;
    public UnityEvent<int, int> OnSlotUsesChanged;
    public UnityEvent<int> OnSelectedSlotChanged;
    public UnityEvent OnInventoryFull;

    [SerializeField] int slotCount = 3;

    InventoryItemSlot[] slots;

    int selectedIndex = 0;
    Item currentEquippedInstance;
    bool useLatch;

    public int SelectedIndex => selectedIndex;
    public InventoryItemSlot[] Slots => slots;

    void Awake()
    {
        if (slots == null || slots.Length != slotCount)
        {
            slots = new InventoryItemSlot[slotCount];
            for (int i = 0; i < slotCount; i++)
                slots[i] = new InventoryItemSlot();
        }

        if (itemHolder == null)
            itemHolder = GetComponentInChildren<ItemHolder>(true);

        ForceSelectSlot(0);
    }

    #region Public API

    public bool Add(ItemData data)
    {
        if (data == null) return false;

        for (int i = 0; i < slotCount; i++)
        {
            if (slots[i].IsEmpty)
            {
                slots[i].data = data;
                slots[i].uses = GetDefaultUsesFor(data);

                OnSlotChanged?.Invoke(i, slots[i].data);
                OnSlotUsesChanged?.Invoke(i, slots[i].uses);

                if (i == selectedIndex)
                    EquipSelected();

                return true;
            }
        }

        OnInventoryFull?.Invoke();
        return false;
    }

    public ItemData ReplaceSelected(ItemData newData)
    {
        ValidateSlotIndex(selectedIndex);

        ItemData old = slots[selectedIndex].data;

        slots[selectedIndex].data = newData;
        slots[selectedIndex].uses = newData != null ? GetDefaultUsesFor(newData) : 0;

        OnSlotChanged?.Invoke(selectedIndex, slots[selectedIndex].data);
        OnSlotUsesChanged?.Invoke(selectedIndex, slots[selectedIndex].uses);

        EquipSelected();

        return old;
    }

    public void Drop(Vector3 dropCoords)
    {
        var slot = slots[SelectedIndex];
        if (slot.data == null) return;
        if (slot.data.prefab == null) return;

        GameObject droppedObj = Instantiate(slot.data.prefab, dropCoords, Quaternion.identity);

        ItemPickup pickup = droppedObj.GetComponentInChildren<ItemPickup>(true);
        if (pickup)
            pickup.SetFreeItem(true);

        Remove();
    }

    public ItemData Remove()
    {
        ValidateSlotIndex(selectedIndex);

        ItemData old = slots[selectedIndex].data;

        slots[selectedIndex].data = null;
        slots[selectedIndex].uses = 0;

        OnSlotChanged?.Invoke(selectedIndex, null);
        OnSlotUsesChanged?.Invoke(selectedIndex, 0);

        UnequipCurrent();

        return old;
    }

    public void SelectSlot(int slotIndex)
    {
        ValidateSlotIndex(slotIndex);
        if (selectedIndex == slotIndex) return;

        selectedIndex = slotIndex;
        OnSelectedSlotChanged?.Invoke(selectedIndex);
        EquipSelected();
    }

    public void SelectNext()
    {
        int next = selectedIndex + 1;
        if (next >= slotCount) next = 0;
        SelectSlot(next);
    }

    public void SelectPrevious()
    {
        int prev = selectedIndex - 1;
        if (prev < 0) prev = slotCount - 1;
        SelectSlot(prev);
    }

    public void SetUseHeld(bool held)
    {
        if (!held)
            useLatch = false;
    }

    public void TryUseSelected()
    {
        var slot = slots[selectedIndex];
        if (slot.data == null || currentEquippedInstance == null)
            return;

        if (currentEquippedInstance is not IUsable usable)
            return;

        bool isConsumable = slot.data is ConsumableData;

        // Consumables: once per press
        if (isConsumable)
        {
            if (useLatch) return;
            useLatch = true;
        }

        usable.Use();

        if (isConsumable)
        {
            slot.uses = Mathf.Max(0, slot.uses - 1);
            OnSlotUsesChanged?.Invoke(selectedIndex, slot.uses);

            if (slot.uses <= 0)
                Remove();
        }
    }

    #endregion

    #region Equip / Instance management

    void EquipSelected()
    {
        UnequipCurrent();

        var slot = slots[selectedIndex];
        if (slot.data == null || itemHolder == null) return;
        if (slot.data.prefab == null)
        {
            Debug.LogWarning($"ItemData {slot.data.itemName} has no prefab.");
            return;
        }

        GameObject inst = Instantiate(slot.data.prefab, itemHolder.transform);
        Item itemComp = inst.GetComponent<Item>();

        if (itemComp == null)
        {
            Debug.LogError("Item prefab does not contain Item component!");
            Destroy(inst);
            return;
        }

        itemComp.Initialize(slot.data);
        itemHolder.Equip(itemComp);
        currentEquippedInstance = itemComp;
    }

    void UnequipCurrent()
    {
        if (currentEquippedInstance == null) return;

        itemHolder.Unequip();
        currentEquippedInstance = null;
    }

    void ForceSelectSlot(int slotIndex)
    {
        ValidateSlotIndex(slotIndex);
        selectedIndex = slotIndex;
        OnSelectedSlotChanged?.Invoke(selectedIndex);
        EquipSelected();
    }

    #endregion

    #region Utilities

    int GetDefaultUsesFor(ItemData data)
    {
        if (data is ConsumableData cd)
            return Mathf.Max(1, cd.maxUses);

        return 0;
    }

    void ValidateSlotIndex(int i)
    {
        if (i < 0 || i >= slotCount)
            throw new ArgumentOutOfRangeException(nameof(i), $"Slot index {i} is out of range");
    }

    #endregion
}