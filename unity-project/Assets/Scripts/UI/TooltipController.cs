using UnityEngine;
using UnityEngine.UIElements;

public class TooltipController
{
    private readonly VisualElement tooltip;
    private readonly Label tooltipText;
    private readonly VisualElement root;

    public TooltipController(VisualElement root)
    {
        this.root = root;
        tooltip = root.Q<VisualElement>("ability-tooltip");
        tooltipText = root.Q<Label>("ability-tooltip-text");
    }

    public void RegisterSlot(VisualElement slot, string abilityName)
    {
        if (string.IsNullOrEmpty(abilityName)) return;

        slot.RegisterCallback<MouseEnterEvent>(evt => ShowTooltip(abilityName, evt.mousePosition));
        slot.RegisterCallback<MouseMoveEvent>(evt => MoveTooltip(evt.mousePosition));
        slot.RegisterCallback<MouseLeaveEvent>(_ => HideTooltip());
    }

    private void ShowTooltip(string text, Vector2 mousePosition)
    {
        if (tooltip == null) return;

        tooltipText.text = text;
        tooltip.style.display = DisplayStyle.Flex;
        MoveTooltip(mousePosition);
    }

    private void MoveTooltip(Vector2 mousePosition)
    {
        if (tooltip == null) return;

        float x = mousePosition.x + 10f;
        float y = mousePosition.y + 10f;

        // Keep tooltip inside root bounds
        float rootWidth = root.resolvedStyle.width;
        float rootHeight = root.resolvedStyle.height;
        float tipWidth = tooltip.resolvedStyle.width;
        float tipHeight = tooltip.resolvedStyle.height;

        if (x + tipWidth > rootWidth) x = mousePosition.x - tipWidth - 10f;
        if (y + tipHeight > rootHeight) y = mousePosition.y - tipHeight - 10f;

        tooltip.style.left = x;
        tooltip.style.top = y;
    }

    private void HideTooltip()
    {
        if (tooltip == null) return;
        tooltip.style.display = DisplayStyle.None;
    }
}