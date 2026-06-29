using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer.positionCount = 5; // 4 corners + closing point
        lineRenderer.loop = false;
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        // Get the 4 corners of the main camera's view in world space
        float z = 0f;
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, -mainCamera.transform.position.z));
        Vector3 topLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, -mainCamera.transform.position.z));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, -mainCamera.transform.position.z));
        Vector3 bottomRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, -mainCamera.transform.position.z));

        // Set z to 0 so it's visible in the 2D scene
        bottomLeft.z = z;
        topLeft.z = z;
        topRight.z = z;
        bottomRight.z = z;

        lineRenderer.SetPositions(new Vector3[]
        {
            bottomLeft,
            topLeft,
            topRight,
            bottomRight,
            bottomLeft // close the rectangle
        });
    }
}