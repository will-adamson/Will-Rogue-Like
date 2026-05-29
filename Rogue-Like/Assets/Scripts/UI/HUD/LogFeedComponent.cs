using UnityEngine.UIElements;

public class LogFeedComponent : IHUDComponent
{
    private ScrollView scroll;
    private VisualElement content;
    private int lineCount;
    private int maxLines;

    public void Init(VisualElement root) => Init(root, 50);

    public void Init(VisualElement root, int maxLines)
    {
        this.maxLines = maxLines;
        scroll  = root.Q<ScrollView>("log-scroll");
        content = root.Q<VisualElement>("log-content");
    }

    public void AddLog(string message, LogType type = LogType.Normal)
    {
        if (content == null) return;

        if (lineCount >= maxLines && content.childCount > 0)
        {
            content.RemoveAt(0);
            lineCount--;
        }

        if (content.childCount > 0)
        {
            VisualElement prev = content[content.childCount - 1];
            prev.RemoveFromClassList("log-line-latest");
            ApplyTypeClass(prev, type);
        }

        Label line = new Label { text = message };
        line.AddToClassList("log-line");
        line.AddToClassList("log-line-latest");
        content.Add(line);
        lineCount++;

        scroll.schedule.Execute(() =>
            scroll.ScrollTo(content[content.childCount - 1])
        ).StartingIn(10);
    }

    public void LogDamage(string message) => AddLog(message, LogType.Damage);
    public void LogGood(string message)   => AddLog(message, LogType.Good);
    public void LogGold(string message)   => AddLog(message, LogType.Gold);
    public void LogSystem(string message) => AddLog(message, LogType.System);

    public void Clear()
    {
        content?.Clear();
        lineCount = 0;
    }

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

    public enum LogType { Normal, Damage, Good, Gold, System }
}
