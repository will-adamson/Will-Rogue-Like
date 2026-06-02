using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class HUDController : MonoBehaviour
{
    [SerializeField] private int logMaxLines = 50;

    public static HUDController Instance { get; private set; }

    public HealthBarsComponent HealthBars { get; private set; }
    public LogFeedComponent LogFeed { get; private set; }

    public event System.Action OnHUDReady;
    public bool IsReady { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        HealthBars = new HealthBarsComponent();
        HealthBars.Init(root);

        LogFeed = new LogFeedComponent();
        LogFeed.Init(root, logMaxLines);

        root.RegisterCallback<GeometryChangedEvent>(OnLayoutReady);
    }

    private void OnLayoutReady(GeometryChangedEvent evt)
    {
        GetComponent<UIDocument>().rootVisualElement
            .UnregisterCallback<GeometryChangedEvent>(OnLayoutReady);
        IsReady = true;
        OnHUDReady?.Invoke();
    }

    private void OnDisable()
    {
        UIDocument doc = GetComponent<UIDocument>();
        if (doc?.rootVisualElement != null)
            doc.rootVisualElement.UnregisterCallback<GeometryChangedEvent>(OnLayoutReady);
        IsReady = false;
    }
}