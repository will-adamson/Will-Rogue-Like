using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// HUD component that manages the quick slot bar: item icons, stack counts,
/// cooldown overlays, keybind labels, slot selection, and progressive slot unlocking.
/// </summary>
/// <remarks>
/// Implements <see cref="IHUDComponent"/>. Call <see cref="Init"/> once after the
/// UI Document layout is ready. The bar always renders all <see cref="TotalSlots"/>
/// slots; locked slots display a lock indicator until unlocked via <see cref="UnlockSlot"/>.
/// Drive per-frame cooldown updates via <see cref="SetCooldown"/> and item changes
/// via <see cref="SetSlotItem"/>. The selected slot highlight is managed by
/// <see cref="SetSelectedSlot"/>.
/// </remarks>
public class QuickSlotsComponent : IHUDComponent
{
    #region Constants

    /// <summary>Total number of slots in the bar, including locked ones.</summary>
    public const int TotalSlots = 10;

    /// <summary>Number of slots unlocked at the start without any progression.</summary>
    public const int InitialUnlockedSlots = 4;

    #endregion

    #region Private Fields

    /// <summary>Ordered array of all slot root elements, indexed 0-<see cref="TotalSlots"/>.</summary>
    private VisualElement[] slots;

    /// <summary>Icon child elements for each active slot; null for locked slots.</summary>
    private VisualElement[] slotIcons;

    /// <summary>Stack count label for each active slot; null for locked slots.</summary>
    private Label[] slotCounts;

    /// <summary>Cooldown overlay element for each active slot; null for locked slots.</summary>
    private VisualElement[] slotCooldowns;

    /// <summary>Index of the currently selected slot; -1 if none selected.</summary>
    private int selectedSlot = -1;

    #endregion

    #region IHUDComponent

    /// <summary>
    /// Resolves and caches all slot element references from <paramref name="root"/>.
    /// Must be called before any other method.
    /// </summary>
    /// <param name="root">Root <see cref="VisualElement"/> of the HUD UI Document.</param>
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

            if (slot == null || slot.ClassListContains("quickslot-locked")) continue;

            slotIcons[i] = slot.Q<VisualElement>("quickslot-icon");
            slotCounts[i] = slot.Q<Label>("quickslot-count");
            slotCooldowns[i] = slot.Q<VisualElement>("quickslot-cooldown");
        }
    }

    #endregion

    #region Public API

    /// <summary>
    /// Assigns an item to the given slot, updating its icon and stack count.
    /// Pass <see langword="null"/> for <paramref name="icon"/> to clear the slot.
    /// </summary>
    /// <param name="slotIndex">Zero-based slot index (0-<see cref="TotalSlots"/> - 1).</param>
    /// <param name="icon">The item's <see cref="Sprite"/>, or <see langword="null"/> to empty the slot.</param>
    /// <param name="count">Stack size; hidden when less than 2.</param>
    public void SetSlotItem(int slotIndex, Sprite icon, int count = 1)
    {
        if (!IsActiveSlot(slotIndex)) return;

        slotIcons[slotIndex].style.backgroundImage =
            icon != null ? new StyleBackground(icon) : new StyleBackground();

        slotCounts[slotIndex].text = count > 1 ? count.ToString() : "";
    }

    /// <summary>
    /// Updates the cooldown overlay height for the given slot.
    /// An overlay height of 100 % means fully on cooldown; 0 % means ready.
    /// </summary>
    /// <param name="slotIndex">Zero-based slot index.</param>
    /// <param name="fraction">
    /// Cooldown progress as a fraction (0-1) where 1 is fully on cooldown and 0 is ready.
    /// Values outside this range are clamped.
    /// </param>
    public void SetCooldown(int slotIndex, float fraction)
    {
        if (!IsActiveSlot(slotIndex)) return;
        float clamped = Mathf.Clamp01(fraction);
        slotCooldowns[slotIndex].style.height = new Length(clamped * 100f, LengthUnit.Percent);
    }

    /// <summary>
    /// Moves the selection highlight to the given slot index.
    /// Passing -1 clears the selection without highlighting any slot.
    /// </summary>
    /// <param name="slotIndex">Zero-based slot index, or -1 to deselect all.</param>
    public void SetSelectedSlot(int slotIndex)
    {
        if (selectedSlot >= 0 && selectedSlot < TotalSlots && slots[selectedSlot] != null)
            slots[selectedSlot].RemoveFromClassList("quickslot-selected");

        selectedSlot = slotIndex;

        if (selectedSlot >= 0 && selectedSlot < TotalSlots && slots[selectedSlot] != null)
            slots[selectedSlot].AddToClassList("quickslot-selected");
    }

    /// <summary>
    /// Unlocks a previously locked slot, replacing the lock indicator with the
    /// standard active slot layout (icon, count, cooldown overlay, keybind label).
    /// </summary>
    /// <param name="slotIndex">Zero-based index of the slot to unlock.</param>
    /// <remarks>
    /// TODO: Rebuilding child elements at runtime is a placeholder approach.
    /// Consider pre-building all slot content in UXML and toggling visibility
    /// instead, once the progression system is more defined.
    /// </remarks>
    public void UnlockSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= TotalSlots) return;

        VisualElement slot = slots[slotIndex];
        if (slot == null || !slot.ClassListContains("quickslot-locked")) return;

        slot.Clear();
        slot.RemoveFromClassList("quickslot-locked");

        Label keybind = new Label { text = (slotIndex + 1).ToString() };
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

    #endregion

    #region Private Helpers

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="slotIndex"/> is in range
    /// and the slot has been unlocked and its child elements resolved.
    /// </summary>
    /// <param name="slotIndex">Zero-based slot index to validate.</param>
    private bool IsActiveSlot(int slotIndex) =>
        slotIndex >= 0 && slotIndex < TotalSlots && slotIcons[slotIndex] != null;

    #endregion
}