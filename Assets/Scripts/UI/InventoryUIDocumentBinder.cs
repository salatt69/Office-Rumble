using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class InventoryUIDocumentBinder : MonoBehaviour
{
    [SerializeField] Sprite emptySlotIcon;
    [SerializeField] Color selectedTint;
    [SerializeField] Color deSelectedTint;

    Inventory inventorySystem;
    UIDocument uiDocument;
    VisualElement[] slotRoots;
    VisualElement[] slotIcons;
    Label[] slotUses;

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
    }

    public void Bind(Inventory newInventory)
    {
        if (inventorySystem != null)
        {
            inventorySystem.OnSlotChanged.RemoveListener(UpdateSlot);
            inventorySystem.OnSlotUsesChanged.RemoveListener(UpdateSlotUses);
            inventorySystem.OnSelectedSlotChanged.RemoveListener(UpdateSelection);
        }

        inventorySystem = newInventory;

        if (!isActiveAndEnabled || inventorySystem == null)
            return;

        EnsureUIRefs();

        inventorySystem.OnSlotChanged.AddListener(UpdateSlot);
        inventorySystem.OnSlotUsesChanged.AddListener(UpdateSlotUses);
        inventorySystem.OnSelectedSlotChanged.AddListener(UpdateSelection);

        RefreshAll();
    }

    void EnsureUIRefs()
    {
        if (!uiDocument)
            uiDocument = GetComponent<UIDocument>();

        if (slotRoots != null && slotIcons != null && slotUses != null)
            return;

        var root = uiDocument.rootVisualElement;

        slotRoots = new VisualElement[3];
        slotIcons = new VisualElement[3];
        slotUses = new Label[3];

        for (int i = 0; i < 3; i++)
        {
            slotRoots[i] = root.Q<VisualElement>($"slot-{i}");
            slotIcons[i] = slotRoots[i].Q<VisualElement>("icon");
            slotUses[i] = slotRoots[i].Q<Label>("uses");
        }
    }

    void OnEnable()
    {
        if (!uiDocument || inventorySystem == null) return;

        var root = uiDocument.rootVisualElement;

        slotRoots = new VisualElement[3];
        slotIcons = new VisualElement[3];
        slotUses = new Label[3];

        for (int i = 0; i < 3; i++)
        {
            slotRoots[i] = root.Q<VisualElement>($"slot-{i}");
            slotIcons[i] = slotRoots[i].Q<VisualElement>("icon");
            slotUses[i] = slotRoots[i].Q<Label>("uses");
        }

        inventorySystem.OnSlotChanged.AddListener(UpdateSlot);
        inventorySystem.OnSlotUsesChanged.AddListener(UpdateSlotUses);
        inventorySystem.OnSelectedSlotChanged.AddListener(UpdateSelection);

        RefreshAll();
    }

    void OnDisable()
    {
        if (inventorySystem == null) return;

        inventorySystem.OnSlotChanged.RemoveListener(UpdateSlot);
        inventorySystem.OnSlotUsesChanged.RemoveListener(UpdateSlotUses);
        inventorySystem.OnSelectedSlotChanged.RemoveListener(UpdateSelection);
    }

    void RefreshAll()
    {
        for (int i = 0; i < slotRoots.Length; i++)
        {
            var slot = inventorySystem.Slots[i];
            UpdateSlot(i, slot.data);
            UpdateSlotUses(i, slot.uses);
        }

        UpdateSelection(inventorySystem.SelectedIndex);
    }

    void UpdateSlot(int slotIndex, ItemData data)
    {
        if (slotIndex < 0 || slotIndex >= slotIcons.Length) return;

        Sprite icon = data != null ? data.icon : emptySlotIcon;

        slotIcons[slotIndex].style.backgroundImage = icon != null
            ? new StyleBackground(icon)
            : StyleKeyword.Null;
    }

    void UpdateSlotUses(int slotIndex, int usesLeft)
    {
        if (slotIndex < 0 || slotIndex >= slotUses.Length) return;

        var slot = inventorySystem.Slots[slotIndex];

        if (slot.data is ConsumableData cd && cd.maxUses > 1)
            slotUses[slotIndex].text = usesLeft.ToString();
        else
            slotUses[slotIndex].text = string.Empty;
    }

    void UpdateSelection(int selectedIndex)
    {
        for (int i = 0; i < slotRoots.Length; i++)
        {
            if (i == selectedIndex)
            {
                slotRoots[i].AddToClassList("selected");
                slotIcons[i].style.unityBackgroundImageTintColor = selectedTint;
                slotUses[i].style.color = selectedTint;
            }
            else
            {
                slotRoots[i].RemoveFromClassList("selected");
                slotIcons[i].style.unityBackgroundImageTintColor = deSelectedTint;
                slotUses[i].style.color = deSelectedTint;
            }
        }
    }
}