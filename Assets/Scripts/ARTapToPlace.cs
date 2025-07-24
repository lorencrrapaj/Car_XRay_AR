using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

[RequireComponent(typeof(ARRaycastManager))]
public class ARTapToPlace : MonoBehaviour
{
    [Header("AR Placement")]
    [Tooltip("Drag your car prefab (e.g. newmiata) here")]
    public GameObject carPrefab;

    [Header("Press & Hold Settings")]
    [Tooltip("Seconds you must hold before placement")]
    public float holdThreshold = 0.5f;
    [Tooltip("Max finger-move (in pixels) allowed during hold")]
    public float maxMoveDistance = 10f;

    [Header("UI Elements")]
    [Tooltip("Drag your Full Explode UI Button here")]
    public Button fullExplodeButton;
    [Tooltip("Drag your Explode Mode UI Toggle here")]
    public Toggle explodeModeToggle;
    [Tooltip("Drag your Gauges UI Button here")]
    public Button gaugesButton;

    ARRaycastManager _raycast;
    GameObject _spawned;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    // For press‐and‐hold
    bool _trackingHold = false;
    Vector2 _holdStartPos;
    float _holdStartTime;

    // Spawned‐car controllers
    FullExplodeController fullExplodeCtrl;
    ExplodeModeController explodeModeCtrl;
    GaugeViewController gaugeViewCtrl;

    void Awake()
    {
        _raycast = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (Input.touchCount != 1)
        {
            // any other gesture cancels hold
            _trackingHold = false;
            return;
        }

        Touch t = Input.GetTouch(0);

        switch (t.phase)
        {
            case TouchPhase.Began:
                // start tracking
                _trackingHold = true;
                _holdStartPos = t.position;
                _holdStartTime = Time.time;
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (!_trackingHold) break;
                // if finger moved too far, cancel
                if (Vector2.Distance(t.position, _holdStartPos) > maxMoveDistance)
                {
                    _trackingHold = false;
                    break;
                }
                // if held long enough, place
                if (Time.time - _holdStartTime >= holdThreshold)
                {
                    TryPlaceAt(t.position);
                    _trackingHold = false;  // only once
                }
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                // user lifted too soon
                _trackingHold = false;
                break;
        }
    }

    void TryPlaceAt(Vector2 screenPos)
    {
        // Raycast against AR planes
        if (!_raycast.Raycast(screenPos, s_Hits, TrackableType.PlaneWithinPolygon))
            return;

        Pose hitPose = s_Hits[0].pose;

        if (_spawned == null)
        {
            // 1) Spawn
            _spawned = Instantiate(carPrefab, hitPose.position, hitPose.rotation);
            _spawned.SetActive(true);

            // 2) Grab controllers
            fullExplodeCtrl = _spawned.GetComponentInChildren<FullExplodeController>();
            explodeModeCtrl = _spawned.GetComponentInChildren<ExplodeModeController>();
            gaugeViewCtrl = _spawned.GetComponentInChildren<GaugeViewController>();

            // sanity
            if (fullExplodeCtrl == null) Debug.LogError("Missing FullExplodeController!");
            if (explodeModeCtrl == null) Debug.LogError("Missing ExplodeModeController!");
            if (gaugeViewCtrl == null) Debug.LogError("Missing GaugeViewController!");

            // 3) Hook UI
            fullExplodeButton.onClick.RemoveAllListeners();
            fullExplodeButton.onClick.AddListener(fullExplodeCtrl.ToggleFullExplode);

            explodeModeToggle.onValueChanged.RemoveAllListeners();
            explodeModeToggle.onValueChanged.AddListener(explodeModeCtrl.SetExplodeMode);

            gaugesButton.onClick.RemoveAllListeners();
            gaugesButton.onClick.AddListener(gaugeViewCtrl.ToggleGaugesView);
        }
        else
        {
            // reposition existing
            _spawned.transform.SetPositionAndRotation(
                hitPose.position, hitPose.rotation
            );
        }
    }
}
