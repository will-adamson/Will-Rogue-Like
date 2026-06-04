using UnityEngine.UIElements;

/// <summary>
/// HUD component that manages a scrollable combat log feed, supporting colour-coded
/// message categories and automatic trimming when the line cap is reached.
/// </summary>
/// <remarks>
/// Implements <see cref="IHUDComponent"/>. The most-recently added line is styled with
/// the <c>log-line-latest</c> USS class; older lines of the same type receive their
/// category class (e.g. <c>log-line-damage</c>) once a newer line arrives.
/// The scroll view automatically snaps to the bottom after each addition using a
/// 10 ms deferred <see cref="IVisualElementScheduledItem"/> call.
/// </remarks>
public class LogFeedComponent : IHUDComponent
{
    #region Private State

    /// <summary>The scroll container that wraps <see cref="content"/>.</summary>
    private ScrollView scroll;

    /// <summary>Direct parent of all log line labels.</summary>
    private VisualElement content;

    /// <summary>Current number of log lines present in <see cref="content"/>.</summary>
    private int lineCount;

    /// <summary>Maximum number of lines retained before the oldest is removed.</summary>
    private int maxLines;

    #endregion

    #region IHUDComponent / Init

    /// <summary>
    /// Initialises the component with the default maximum of 50 lines.
    /// </summary>
    /// <param name="root">Root <see cref="VisualElement"/> of the HUD UI Document.</param>
    public void Init(VisualElement root) => Init(root, 50);

    /// <summary>
    /// Initialises the component, caching the scroll view and content container
    /// from <paramref name="root"/> and setting the line cap.
    /// </summary>
    /// <param name="root">Root <see cref="VisualElement"/> of the HUD UI Document.</param>
    /// <param name="maxLines">Maximum number of log entries retained simultaneously.</param>
    public void Init(VisualElement root, int maxLines)
    {
        this.maxLines = maxLines;
        scroll = root.Q<ScrollView>("log-scroll");
        content = root.Q<VisualElement>("log-content");
    }

    #endregion

    #region Public API

    /// <summary>
    /// Appends a new log entry, removing the oldest line if the cap is exceeded,
    /// then scrolls the view to the bottom.
    /// </summary>
    /// <param name="message">Text content of the log entry.</param>
    /// <param name="type">Category controlling the USS colour class applied to the entry.</param>
    public void AddLog(string message, LogType type)
    {
        if (content == null) return;

        if (lineCount >= maxLines && content.childCount > 0)
        {
            content.RemoveAt(0);
            lineCount--;
        }

        // Demote the previous latest line to its type class.
        if (content.childCount > 0)
        {
            VisualElement prev = content[content.childCount - 1];
            prev.RemoveFromClassList("log-line-latest");
            ApplyTypeClass(prev, (LogType)prev.userData);
        }

        Label line = new Label
        {
            text = message,
            userData = type
        };
        line.AddToClassList("log-line");
        line.AddToClassList("log-line-latest");
        content.Add(line);
        lineCount++;

        // Defer scroll so the layout pass completes first.
        scroll.schedule.Execute(() =>
            scroll.ScrollTo(content[content.childCount - 1])
        ).StartingIn(10);
    }

    /// <summary>Appends a damage event entry (enemy attacks, hazard hits, etc.).</summary>
    /// <param name="message">Formatted damage description.</param>
    public void LogDamage(string message) => AddLog(message, LogType.Damage);

    /// <summary>Appends a positive event entry (healing, buffs, level-up, etc.).</summary>
    /// <param name="message">Formatted positive event description.</param>
    public void LogGood(string message) => AddLog(message, LogType.Good);

    /// <summary>Appends a gold/currency event entry (pickup, purchase, etc.).</summary>
    /// <param name="message">Formatted gold event description.</param>
    public void LogGold(string message) => AddLog(message, LogType.Gold);

    /// <summary>Appends a system or narrative event entry (room cleared, boss spawn, etc.).</summary>
    /// <param name="message">Formatted system event description.</param>
    public void LogSystem(string message) => AddLog(message, LogType.System);

    /// <summary>Removes all log entries and resets the line counter.</summary>
    public void Clear()
    {
        content?.Clear();
        lineCount = 0;
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Applies the USS class that corresponds to <paramref name="type"/> to <paramref name="el"/>.
    /// Called when a line is demoted from the "latest" state.
    /// </summary>
    /// <param name="el">The log line element to style.</param>
    /// <param name="type">The <see cref="LogType"/> stored in the element's <c>userData</c>.</param>
    private void ApplyTypeClass(VisualElement el, LogType type)
    {
        switch (type)
        {
            case LogType.Damage: el.AddToClassList("log-line-damage"); break;
            case LogType.Good:   el.AddToClassList("log-line-good");   break;
            case LogType.Gold:   el.AddToClassList("log-line-gold");   break;
            case LogType.System: el.AddToClassList("log-line-system"); break;
        }
    }

    #endregion

    #region Nested Types

    /// <summary>
    /// Classifies a log entry for colour-coded styling in the feed.
    /// </summary>
    public enum LogType
    {
        /// <summary>Plain white text; no special category class applied.</summary>
        Normal,

        /// <summary>Damage events: attacks received, hazard hits.</summary>
        Damage,

        /// <summary>Positive events: heals, buffs, level-ups.</summary>
        Good,

        /// <summary>Gold or currency events: pickups, purchases.</summary>
        Gold,

        /// <summary>System or narrative events: room cleared, boss spawned.</summary>
        System
    }

    #endregion
}