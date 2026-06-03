using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private float speed = 30f;
    [SerializeField] private float roomHeight = 40f;
    [SerializeField] private float roomPadding = 2f;
    [SerializeField] private Transform target;

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
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            // Z is preserved so the camera stays in front of the 2D scene.
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
}