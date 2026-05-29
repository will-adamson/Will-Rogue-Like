using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class HUDController : MonoBehaviour
{
    [SerializeField] private int logMaxLines = 50;

    public static HUDController Instance { get; private set; }

    public HealthBarsComponent HealthBars { get; private set; }
    public LogFeedComponent    LogFeed    { get; private set; }

    public event System.Action OnHUDReady;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
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
        OnHUDReady?.Invoke();
    }

    private void OnDisable()
    {
        GetComponent<UIDocument>().rootVisualElement
            .UnregisterCallback<GeometryChangedEvent>(OnLayoutReady);
    }
}
