using UnityEngine;
using UnityEngine.UIElements;

public class MinimapComponent : IHUDComponent
{
    private VisualElement minimapContainer;
    private Label minimapZoneLabel;

    public void Init(VisualElement root)
    {
        minimapContainer = root.Q<VisualElement>("minimap-container");
        minimapZoneLabel = root.Q<Label>("minimap-zone-label");
    }

    public void SetVisible(bool visible)
    {
        if (minimapContainer == null) return;

        minimapContainer.style.display =
            visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void SetZoneLabel(string zoneName)
    {
        if (minimapZoneLabel == null) return;

        minimapZoneLabel.text = zoneName.ToUpper();
    }
}