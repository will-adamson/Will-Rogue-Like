using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class HUDController : MonoBehaviour
{
    public static HUDController Instance { get; private set; }

    [SerializeField] private int logMaxLines = 50;

    public HealthBarsComponent HealthBars { get; private set; }
    public LogFeedComponent LogFeed { get; private set; }
    public TelegraphComponent Telegraph { get; private set; }

    public event System.Action OnHUDReady;

    public bool IsReady { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        HealthBars = new HealthBarsComponent();
        HealthBars.Init(root);

        LogFeed = new LogFeedComponent();
        LogFeed.Init(root, logMaxLines);

        Telegraph = new TelegraphComponent();
        Telegraph.Init(root);

        root.RegisterCallback<GeometryChangedEvent>(OnLayoutReady);
    }

    private void OnDisable()
    {
        UIDocument doc = GetComponent<UIDocument>();

        if (doc?.rootVisualElement != null)
            doc.rootVisualElement.UnregisterCallback<GeometryChangedEvent>(OnLayoutReady);

        IsReady = false;
    }

    private void OnLayoutReady(GeometryChangedEvent evt)
    {
        GetComponent<UIDocument>().rootVisualElement
            .UnregisterCallback<GeometryChangedEvent>(OnLayoutReady);

        IsReady = true;
        OnHUDReady?.Invoke();
    }
}