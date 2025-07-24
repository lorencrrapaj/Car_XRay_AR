using UnityEngine;

/// <summary>
/// Simple billboard: this GameObject will always face the main camera,
/// while remaining upright (no tilt). Front (+Z) of the object will point toward the camera.
/// </summary>
[DisallowMultipleComponent]
public class BillboardLabel : MonoBehaviour
{
    Transform _cameraTransform;

    void Start()
    {
        // Cache the main camera’s transform
        if (Camera.main != null)
            _cameraTransform = Camera.main.transform;
        else
            Debug.LogWarning("BillboardLabel: No Camera.main found in scene.");
    }

    void LateUpdate()
    {
        if (_cameraTransform == null)
            return;

        // Compute vector from the label to the camera
        Vector3 toCamera = _cameraTransform.position - transform.position;

        // Zero out any vertical component so it stays level
        toCamera.y = 0f;

        if (toCamera.sqrMagnitude < 0.0001f)
            return;

        // Flip the vector so +Z faces the camera
        Vector3 forward = -toCamera.normalized;

        // Apply rotation
        transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }
}
