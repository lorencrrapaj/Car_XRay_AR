using UnityEngine;
using System.Collections;

[AddComponentMenu("XRay/Gauge View Controller")]
public class GaugeViewController : MonoBehaviour
{
    [Header("Gauge Cluster")]
    [Tooltip("Parent of all gauge parts to lift up")]
    public Transform gaugeContainer;
    [Tooltip("Vertical lift distance for the gauge cluster")]
    public float liftDistance = 0.2f;

    [Header("Camera Settings")]
    [Tooltip("Camera to move and rotate")]
    public Camera targetCamera;
    [Tooltip("Optional transform representing final camera position & rotation")]
    public Transform zoomInPoint;
    [Header("Fallback Camera Offset")]
    [Tooltip("Distance from gauge center if zoomInPoint is null")]
    public float fallbackZoomDistance = 1.0f;
    [Tooltip("Vertical camera offset if zoomInPoint is null")]
    public float fallbackZoomHeight = 0.5f;

    [Header("Animation Settings")]
    [Tooltip("Seconds to animate lift & camera move")]
    public float animationDuration = 1.0f;

    // internal state
    private Vector3 origContainerPos;
    private Vector3 origCamPos;
    private Quaternion origCamRot;
    private bool zoomed = false;

    void Start()
    {
        // Validate gauge container
        if (gaugeContainer == null)
        {
            Debug.LogError("GaugeViewController: gaugeContainer not set.");
            enabled = false;
            return;
        }
        // Default camera if not assigned
        if (targetCamera == null)
        {
            if (Camera.main != null)
            {
                targetCamera = Camera.main;
                Debug.LogWarning("GaugeViewController: targetCamera not set, defaulting to Camera.main.");
            }
            else
            {
                Debug.LogError("GaugeViewController: targetCamera not set and no Camera.main found.");
                enabled = false;
                return;
            }
        }
        // Cache originals
        origContainerPos = gaugeContainer.localPosition;
        origCamPos = targetCamera.transform.position;
        origCamRot = targetCamera.transform.rotation;
    }

    /// <summary>
    /// Toggles the gauge zoom/lift view. Hook to a UI Button OnClick.
    /// </summary>
    public void ToggleGaugesView()
    {
        StopAllCoroutines();
        if (zoomed)
            StartCoroutine(CollapseRoutine());
        else
            StartCoroutine(ExplodeRoutine());
    }

    private IEnumerator ExplodeRoutine()
    {
        // Record start states
        Vector3 startContainer = gaugeContainer.localPosition;
        Vector3 startCam = targetCamera.transform.position;
        Quaternion startRot = targetCamera.transform.rotation;

        // Compute end states
        Vector3 endContainer = origContainerPos + Vector3.up * liftDistance;
        Vector3 endCam;
        Quaternion endRot;
        if (zoomInPoint != null)
        {
            endCam = zoomInPoint.position;
            endRot = zoomInPoint.rotation;
        }
        else
        {
            // Fallback: move camera closer and look at gauge
            Vector3 direction = (origCamPos - gaugeContainer.position).normalized;
            endCam = gaugeContainer.position + direction * liftDistance * 2 + Vector3.up * liftDistance;
            endRot = Quaternion.LookRotation(gaugeContainer.position - endCam, Vector3.up);
        }

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            float t = elapsed / animationDuration;
            gaugeContainer.localPosition = Vector3.Lerp(startContainer, endContainer, t);
            targetCamera.transform.position = Vector3.Lerp(startCam, endCam, t);
            targetCamera.transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Snap final
        gaugeContainer.localPosition = endContainer;
        targetCamera.transform.position = endCam;
        targetCamera.transform.rotation = endRot;
        zoomed = true;
    }

    private IEnumerator CollapseRoutine()
    {
        // Record start
        Vector3 startContainer = gaugeContainer.localPosition;
        Vector3 startCam = targetCamera.transform.position;
        Quaternion startRot = targetCamera.transform.rotation;

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            float t = elapsed / animationDuration;
            gaugeContainer.localPosition = Vector3.Lerp(startContainer, origContainerPos, t);
            targetCamera.transform.position = Vector3.Lerp(startCam, origCamPos, t);
            targetCamera.transform.rotation = Quaternion.Slerp(startRot, origCamRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Snap back
        gaugeContainer.localPosition = origContainerPos;
        targetCamera.transform.position = origCamPos;
        targetCamera.transform.rotation = origCamRot;
        zoomed = false;
    }
}
