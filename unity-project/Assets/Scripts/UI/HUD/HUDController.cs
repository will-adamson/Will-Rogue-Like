using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Singleton MonoBehaviour that owns and initialises all HUD components,
/// exposes them for game systems to write to, and fires <see cref="OnHUDReady"/>
/// once the UI Toolkit layout pass has completed.
/// </summary>
/// <remarks>
/// Attach to the GameObject that carries the scene's <see cref="UIDocument"/>.
/// Other systems should subscribe to <see cref="OnHUDReady"/> or check
/// <see cref="IsReady"/> before writing to <see cref="HealthBars"/> or
/// <see cref="LogFeed"/>, since UI Toolkit elements have no valid geometry
/// until after the first layout pass.
/// </remarks>
[RequireComponent(typeof(UIDocument))]
public class HUDController : MonoBehaviour
{
    #region Singleton

    /// <summary>Shared instance; set during <see cref="Awake"/>.</summary>
    public static HUDController Instance { get; private set; }

    #endregion

    #region Inspector Fields

    /// <summary>Maximum number of lines the combat log retains before trimming the oldest.</summary>
    [SerializeField] private int logMaxLines = 50;

    #endregion

    #region Public Properties

    /// <summary>
    /// Component managing the HP, stamina, experience bars, and stat labels.
    /// Valid after <see cref="OnHUDReady"/> fires.
    /// </summary>
    public HealthBarsComponent HealthBars { get; private set; }

    /// <summary>
    /// Component managing the scrollable combat log feed.
    /// Valid after <see cref="OnHUDReady"/> fires.
    /// </summary>
    public LogFeedComponent LogFeed { get; private set; }

    /// <summary>
    /// Raised once on the frame when the UI Toolkit layout pass completes
    /// and all element geometry is valid.
    /// </summary>
    public event System.Action OnHUDReady;

    /// <summary>
    /// <see langword="true"/> after the first layout pass; <see langword="false"/>
    /// while the document is loading or after the component is disabled.
    /// </summary>
    public bool IsReady { get; private set; }

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Enforces singleton pattern, instantiates HUD components, binds them to the
    /// UI Document root, and registers a one-shot <see cref="GeometryChangedEvent"/>
    /// callback to detect layout readiness.
    /// Duplicate instances are destroyed immediately.
    /// </summary>
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

    /// <summary>
    /// Unregisters the geometry callback and marks the HUD as not ready
    /// when the component is disabled (e.g. on scene unload).
    /// </summary>
    private void OnDisable()
    {
        UIDocument doc = GetComponent<UIDocument>();
        if (doc?.rootVisualElement != null)
            doc.rootVisualElement.UnregisterCallback<GeometryChangedEvent>(OnLayoutReady);
        IsReady = false;
    }

    #endregion

    #region Private Callbacks

    /// <summary>
    /// Fired once when the UI Toolkit layout pass completes.
    /// Unregisters itself, sets <see cref="IsReady"/>, and raises <see cref="OnHUDReady"/>.
    /// </summary>
    /// <param name="evt">The geometry-changed event from UI Toolkit (unused beyond triggering).</param>
    private void OnLayoutReady(GeometryChangedEvent evt)
    {
        GetComponent<UIDocument>().rootVisualElement
            .UnregisterCallback<GeometryChangedEvent>(OnLayoutReady);
        IsReady = true;
        OnHUDReady?.Invoke();
    }

    #endregion
}