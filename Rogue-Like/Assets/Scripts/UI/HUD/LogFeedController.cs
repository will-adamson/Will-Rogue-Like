using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class LogFeedController : MonoBehaviour
{
    [SerializeField] private int maxLines = 50;

    private ScrollView scroll;
    private VisualElement content;
    private int lineCount;

    public static LogFeedController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Sample logs for testing
    private void Start()
    {
        AddLog("You descend into the dungeon.", LogType.System);
        AddLog("A goblin warrior notices you.");
        AddLog("The goblin warrior attacks and hits! (Pierce: 4 = 7 - res 3)", LogType.Damage);
        AddLog("You strike the goblin warrior for 12 damage.", LogType.Good);
        AddLog("The goblin warrior shoots an arrow at you but misses.");
        AddLog("You slay the goblin warrior.", LogType.Good);
        AddLog("You find 24 gold.", LogType.Gold);
        AddLog("A goblin shaman enters the room.", LogType.System);
        AddLog("The goblin shaman casts a curse on you!", LogType.Damage);
        AddLog("You resist the curse.", LogType.Good);
    }

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        scroll = root.Q<ScrollView>("log-scroll");
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
    public void LogGood(string message) => AddLog(message, LogType.Good);
    public void LogGold(string message) => AddLog(message, LogType.Gold);
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
            case LogType.Good: el.AddToClassList("log-line-good"); break;
            case LogType.Gold: el.AddToClassList("log-line-gold"); break;
            case LogType.System: el.AddToClassList("log-line-system"); break;
        }
    }

    public enum LogType { Normal, Damage, Good, Gold, System }
}