using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEditor.Experimental.GraphView;

[RequireComponent(typeof(Camera))]
public class MinimapCameraController : MonoBehaviour
{
    public static MinimapCameraController Instance;
    [SerializeField] private float speed = 30f;
    [SerializeField] private float roomHeight = 40f;
    [SerializeField] private float roomPadding = 2f;
    [SerializeField] private Transform target;
    [SerializeField] private RenderTexture minimapRenderTexture;
    [SerializeField] private UIDocument hudDocument;
    [SerializeField] private InputActionReference toggleMinimap;
    private bool minimapVisible = true;

    [Header("Zoom")] 
    [SerializeField] private InputActionReference zoom;
    [SerializeField] private float zoomMin = 3f;
    [SerializeField] private float zoomMax = 12f;
    [SerializeField] private float zoomStep = 3f;


    private Camera cam;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        if (cam != null)
            cam.orthographicSize = (roomHeight / 2f) + roomPadding;

        if (minimapRenderTexture != null && hudDocument != null)
        {
            var root = hudDocument.rootVisualElement;
            var minimapElement = root.Q<VisualElement>("minimap-container");
            if (minimapElement != null)
                minimapElement.style.backgroundImage = Background.FromRenderTexture(minimapRenderTexture);
        }
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(target.position.x, target.position.y, transform.position.z),
                speed * Time.fixedDeltaTime
            );
        }
    }

    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void OnEnable()
    {
        zoom.action.Enable();
        toggleMinimap.action.Enable();
        zoom.action.performed += OnZoom;
        toggleMinimap.action.performed += OnToggleMinimap;
    }

    void OnDisable()
    {
        zoom.action.performed -= OnZoom;
        toggleMinimap.action.performed -= OnToggleMinimap;
    }

    void OnZoom(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - (value * zoomStep), zoomMin, zoomMax);
    }

    void OnToggleMinimap(InputAction.CallbackContext ctx)
    {
        minimapVisible = !minimapVisible;
        var root = hudDocument.rootVisualElement;
        var minimapElement = root.Q<VisualElement>("minimap-container");
        if (minimapElement != null)
            minimapElement.style.display = minimapVisible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}