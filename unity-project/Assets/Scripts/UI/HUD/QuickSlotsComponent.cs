using UnityEngine;
using UnityEngine.UIElements;

public class QuickSlotsComponent : IHUDComponent
{
    public const int TotalSlots = 10;
    public const int InitialUnlockedSlots = 4;

    private VisualElement[] slots;
    private VisualElement[] slotIcons;
    private Label[] slotCounts;
    private VisualElement[] slotCooldowns;

    private int selectedSlot = -1;

    public void Init(VisualElement root)
    {
        slots = new VisualElement[TotalSlots];
        slotIcons = new VisualElement[TotalSlots];
        slotCounts = new Label[TotalSlots];
        slotCooldowns = new VisualElement[TotalSlots];

        for (int i = 0; i < TotalSlots; i++)
        {
            VisualElement slot = root.Q<VisualElement>($"slot-{i}");
            slots[i] = slot;

            if (slot == null || slot.ClassListContains("quickslot-locked"))
                continue;

            slotIcons[i] = slot.Q<VisualElement>("quickslot-icon");
            slotCounts[i] = slot.Q<Label>("quickslot-count");
            slotCooldowns[i] = slot.Q<VisualElement>("quickslot-cooldown");
        }
    }

    public void SetSlotItem(int slotIndex, Sprite icon, int count = 1)
    {
        if (!IsActiveSlot(slotIndex)) return;

        slotIcons[slotIndex].style.backgroundImage =
            icon != null ? new StyleBackground(icon) : new StyleBackground();

        slotCounts[slotIndex].text = count > 1 ? count.ToString() : "";
    }

    public void SetCooldown(int slotIndex, float fraction)
    {
        if (!IsActiveSlot(slotIndex)) return;

        float clamped = Mathf.Clamp01(fraction);
        slotCooldowns[slotIndex].style.height =
            new Length(clamped * 100f, LengthUnit.Percent);
    }

    public void SetSelectedSlot(int slotIndex)
    {
        if (selectedSlot >= 0 &&
            selectedSlot < TotalSlots &&
            slots[selectedSlot] != null)
        {
            slots[selectedSlot].RemoveFromClassList("quickslot-selected");
        }

        selectedSlot = slotIndex;

        if (selectedSlot >= 0 &&
            selectedSlot < TotalSlots &&
            slots[selectedSlot] != null)
        {
            slots[selectedSlot].AddToClassList("quickslot-selected");
        }
    }

    public void UnlockSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= TotalSlots)
            return;

        VisualElement slot = slots[slotIndex];

        if (slot == null || !slot.ClassListContains("quickslot-locked"))
            return;

        slot.Clear();
        slot.RemoveFromClassList("quickslot-locked");

        Label keybind = new Label
        {
            text = (slotIndex + 1).ToString()
        };
        keybind.AddToClassList("quickslot-keybind");

        VisualElement icon = new VisualElement();
        icon.AddToClassList("quickslot-icon");

        Label count = new Label { text = "" };
        count.AddToClassList("quickslot-count");

        VisualElement cooldown = new VisualElement();
        cooldown.AddToClassList("quickslot-cooldown");

        slot.Add(keybind);
        slot.Add(icon);
        slot.Add(count);
        slot.Add(cooldown);

        slotIcons[slotIndex] = icon;
        slotCounts[slotIndex] = count;
        slotCooldowns[slotIndex] = cooldown;
    }

    private bool IsActiveSlot(int slotIndex) =>
        slotIndex >= 0 &&
        slotIndex < TotalSlots &&
        slotIcons[slotIndex] != null;
}