using UnityEngine;

/// <summary>
/// Singleton that smoothly follows a target Transform using
/// <see cref="Vector3.MoveTowards"/> in <see cref="FixedUpdate"/>.
/// The orthographic size is automatically sized to frame a full room on startup.
/// </summary>
/// <remarks>
/// Attach to the scene's main Camera. Call <see cref="ChangeTarget"/> whenever the
/// player transitions to a new room so the camera re-centers on the player.
/// </remarks>
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    #region Singleton

    /// <summary>Shared instance; set during <see cref="Awake"/>.</summary>
    public static CameraController Instance;

    #endregion

    #region Inspector Fields

    /// <summary>Maximum distance in world units the camera moves per second toward the target.</summary>
    [SerializeField] private float speed = 30f;

    /// <summary>
    /// Vertical extent of a room in world units. Used to compute the camera's
    /// orthographic size so the full room height is visible.
    /// </summary>
    [SerializeField] private float roomHeight = 40f;

    /// <summary>
    /// Extra world units added to the orthographic size beyond half the room height,
    /// ensuring walls are not clipped at the screen edge.
    /// </summary>
    [SerializeField] private float roomPadding = 2f;

    /// <summary>The Transform the camera follows. Assignable at runtime via <see cref="ChangeTarget"/>.</summary>
    [SerializeField] private Transform target;

    #endregion

    #region Private State

    private Camera cam;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Enforces singleton pattern and caches the <see cref="Camera"/> component.
    /// Duplicate instances are destroyed immediately.
    /// </summary>
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        cam = GetComponent<Camera>();
    }

    /// <summary>
    /// Sets the camera's orthographic size to half the room height plus <see cref="roomPadding"/>.
    /// </summary>
    void Start()
    {
        if (cam != null)
            cam.orthographicSize = (roomHeight / 2f) + roomPadding;
    }

    /// <summary>
    /// Moves the camera one step toward the target's XY position each physics tick.
    /// The Z position is preserved so the camera stays in front of the scene.
    /// </summary>
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

    #endregion

    #region Public API

    /// <summary>
    /// Redirects the camera to follow <paramref name="newTarget"/> from the next
    /// <see cref="FixedUpdate"/> onward.
    /// </summary>
    /// <param name="newTarget">The Transform to follow. Pass <see langword="null"/> to stop following.</param>
    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
    }

    #endregion
}