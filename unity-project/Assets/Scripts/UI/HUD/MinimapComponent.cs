using UnityEngine;
using UnityEngine.UIElements;

public class MinimapComponent : IHUDComponent
{
    #region Cached UI Elements

    /// <summary>Outermost container; toggled by <see cref="SetVisible"/>.</summary>
    private VisualElement minimapContainer;

    /// <summary>Label displaying the current zone or area name.</summary>
    private Label minimapZoneLabel;

    #endregion

    #region IHUDComponent

    /// <summary>
    /// Resolves and caches all UI element references from <paramref name="root"/>.
    /// Must be called before any other method.
    /// </summary>
    /// <param name="root">Root <see cref="VisualElement"/> of the HUD UI Document.</param>
    public void Init(VisualElement root)
    {
        minimapContainer = root.Q<VisualElement>("minimap-container");
        minimapZoneLabel = root.Q<Label>("minimap-zone-label");
    }

    #endregion

    #region Public API

    /// <summary>
    /// Shows or hides the entire minimap panel.
    /// </summary>
    /// <param name="visible"><see langword="true"/> to show; <see langword="false"/> to hide.</param>
    public void SetVisible(bool visible)
    {
        if (minimapContainer == null) return;
        minimapContainer.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    /// <summary>
    /// Updates the zone name label.
    /// </summary>
    /// <param name="zoneName">Human-readable area or zone name to display.</param>
    public void SetZoneLabel(string zoneName)
    {
        if (minimapZoneLabel == null) return;
        minimapZoneLabel.text = zoneName.ToUpper();
    }

    #endregion
}