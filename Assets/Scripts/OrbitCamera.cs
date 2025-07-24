using UnityEngine;

[AddComponentMenu("Camera-Control/Orbit Camera")]
public class OrbitCamera : MonoBehaviour
{
    [Tooltip("Optional initial target to orbit around")]
    public Transform target;

    [Tooltip("Initial distance from the pivot")]
    public float distance = 5.0f;

    [Tooltip("Rotation speed factor for mouse drag")]
    public float rotationSpeed = 120f;

    [Tooltip("Zoom sensitivity (scroll wheel)")]
    public float zoomSpeed = 2.0f;
    [Tooltip("Minimum zoom distance")]
    public float minDistance = 2.0f;
    [Tooltip("Maximum zoom distance")]
    public float maxDistance = 15.0f;

    [Tooltip("Pan speed along local X/Z axes (WASD/Arrow keys)")]
    public float panSpeed = 2.0f;
    [Tooltip("Vertical pan speed (Q/E keys)")]
    public float verticalPanSpeed = 2.0f;

    private float _yaw;
    private float _pitch;
    private Vector3 _pivotPoint;

    void Start()
    {
        // initialize angles from current orientation
        Vector3 angles = transform.eulerAngles;
        _yaw = angles.y;
        _pitch = angles.x;

        // set initial pivot point
        if (target != null)
            _pivotPoint = target.position;
        else
            _pivotPoint = transform.position + transform.forward * distance;

        // lock rigidbody rotation if present
        if (TryGetComponent<Rigidbody>(out var rb)) rb.freezeRotation = true;
    }

    void LateUpdate()
    {
        // On left-click, set pivot point to raycast hit position
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _pivotPoint = hit.point;
            }
        }

        // Mouse drag (right button) rotates around pivot
        if (Input.GetMouseButton(1))
        {
            _yaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            _pitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        }

        // Zoom with scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);

        // Pan pivot horizontally (WASD or arrows)
        float h = Input.GetAxis("Horizontal");   // A/D or Left/Right
        float v = Input.GetAxis("Vertical");     // W/S or Up/Down
        if (Mathf.Abs(h) > 0.001f || Mathf.Abs(v) > 0.001f)
        {
            Vector3 pan = transform.right * h + transform.forward * v;
            _pivotPoint += pan * panSpeed * Time.deltaTime;
        }

        // Pan pivot vertically (Q/E keys)
        float up = 0f;
        if (Input.GetKey(KeyCode.Q)) up += 1f;
        if (Input.GetKey(KeyCode.E)) up -= 1f;
        if (Mathf.Abs(up) > 0.001f)
        {
            _pivotPoint += Vector3.up * up * verticalPanSpeed * Time.deltaTime;
        }

        // Recompute camera transform
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -distance);
        transform.position = _pivotPoint + offset;
        transform.rotation = rotation;
    }
}
