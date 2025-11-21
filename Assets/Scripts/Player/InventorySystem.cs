using System;
using UnityEngine;
using UnityEngine.Events;

public class InventorySystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ItemHolder itemHolder;

    [Header("Events (UI)")]
    public UnityEvent<int, ItemData> OnSlotChanged; // (slotIndex, newItemData)
    public UnityEvent<int> OnSelectedSlotChanged;   // (newSelectedIndex)
    public UnityEvent OnInventoryFull;
    int slotCount = 3;
    ItemData[] slots;

    // runtime state
    int selectedIndex = 1;
    Item currentEquippedInstance; // instantiated prefab currently in holder (null if nothing)

    public int SelectedIndex => selectedIndex;
    public ItemData[] Slots => slots; // read-only exposure

    void Awake()
    {
        if (slots == null || slots.Length != slotCount)
            slots = new ItemData[slotCount];

        // auto-find holder if not assigned
        if (itemHolder == null)
            itemHolder = GetComponentInChildren<ItemHolder>();

        SelectSlot(0);
    }

    #region Public API

    public bool Add(ItemData data)
    {
        if (data == null) return false;

        for (int i = 0; i < slotCount; i++)
        {
            if (slots[i] == null)
            {
                Debug.Log($"{data.itemName} added to slot: {i}");

                slots[i] = data;

                if (i == SelectedIndex)
                    EquipSelected();

                SelectSlot(i);

                OnSlotChanged?.Invoke(i, slots[i]);
                return true;
            }
        }

        OnInventoryFull?.Invoke();
        return false;
    }

    public ItemData ReplaceSelected(ItemData newData)
    {
        ValidateSlotIndex(SelectedIndex);
        ItemData old = slots[SelectedIndex];
        slots[SelectedIndex] = newData;
        OnSlotChanged?.Invoke(SelectedIndex, slots[SelectedIndex]);

        EquipSelected();

        Debug.Log($"Replaced slot: {SelectedIndex}");

        return old;
    }

    public void Drop(Vector3 dropCoords)
    {
        ItemData data = slots[SelectedIndex];
        Instantiate(data.prefab, dropCoords, Quaternion.identity);

        Remove();
    }

    public ItemData Remove()
    {
        ValidateSlotIndex(SelectedIndex);
        ItemData old = slots[SelectedIndex];
        slots[SelectedIndex] = null;
        OnSlotChanged?.Invoke(SelectedIndex, null);

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

        Debug.Log($"Selected slot: {selectedIndex}");
    }

    public void SelectNext()
    {
        int nextSlot = SelectedIndex + 1; 
        int final = nextSlot > 2 ? 0 : nextSlot;

        SelectSlot(final);
    }
    
    public void SelectPrevious()
    {
        int nextSlot = SelectedIndex - 1; 
        int final = nextSlot < 0 ? 2 : nextSlot;

        SelectSlot(final);
    }

    #endregion

    #region Equip / Instance management

    void EquipSelected()
    {
        UnequipCurrent();

        var data = slots[selectedIndex];
        if (data == null || itemHolder == null) return;

        // instantiate prefab and parent to holder
        if (data.prefab == null)
        {
            Debug.LogWarning($"ItemData {data.itemName} has no prefab.");
            return;
        }

        GameObject inst = Instantiate(data.prefab, itemHolder.transform);
        Item itemComp = inst.GetComponent<Item>();
        if (itemComp == null)
        {
            Debug.LogError("Item prefab does not contain Item component!");
            Destroy(inst);
            return;
        }

        itemComp.Initialize(data);
        itemHolder.Equip(itemComp); // itemHolder will parent and call OnEquip internally
        currentEquippedInstance = itemComp;
    }

    void UnequipCurrent()
    {
        if (currentEquippedInstance != null)
        {
            itemHolder.Unequip(); // this destroys the instance internally
            currentEquippedInstance = null;
        }
    }

    #endregion

    #region Utilities

    void ValidateSlotIndex(int i)
    {
        if (i < 0 || i >= slotCount)
            throw new ArgumentOutOfRangeException(nameof(i), $"Slot index {i} is out of range");
    }

    #endregion
}
