using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private GridManager grid;

    [Header("Camera movement settings")]
    [SerializeField] private float cameraMovementSpeed = 2.5f;
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    [Header("Camera zoom settings")]
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 8f;
    private float zoom;
    private float zoomMultiplier = 4f;
    private float velocity = 0f;
    private float smoothTime = 0.25f;

    private void Start() {
        zoom = (maxZoom + minZoom) / 2;

        Vector2 gridSize = grid.GridSize;
        transform.position = new Vector3(gridSize.x / 2, gridSize.y / 2, transform.position.z);
    }

    private void FixedUpdate() {
        Movement();
        Zoom();
    }

    private void Zoom() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoom -= scroll * zoomMultiplier;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocity, smoothTime);
    }

    private void Movement() {
        float tempCameraMovementSpeed = cam.orthographicSize * cameraMovementSpeed;

        float newX = transform.position.x + Input.GetAxis("Horizontal") * tempCameraMovementSpeed * Time.deltaTime;
        float newY = transform.position.y + Input.GetAxis("Vertical") * tempCameraMovementSpeed * Time.deltaTime;

        float clampedX = Mathf.Clamp(newX, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(newY, minBounds.y, maxBounds.y);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
