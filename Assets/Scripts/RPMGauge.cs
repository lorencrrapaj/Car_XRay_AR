using UnityEngine;

public class RPMGauge : MonoBehaviour
{
    [Tooltip("Reference to the TelemetryPlayer in the scene")]
    public TelemetryPlayer player;
    [Tooltip("Needle transform to rotate")]
    public RectTransform needle;

    [Tooltip("Maximum RPM for full sweep")]
    public float maxRPM = 7000f;
    [Tooltip("Angle (deg) at zero and at maxRPM")]
    public float zeroAngle = 0f, maxAngle = -270f;

    void OnEnable()
    {
        player.OnSample.AddListener(UpdateNeedle);
    }

    void OnDisable()
    {
        player.OnSample.RemoveListener(UpdateNeedle);
    }

    void UpdateNeedle(TelemetrySample s)
    {
        var t = Mathf.Clamp01(s.rpm / maxRPM);
        var angle = Mathf.Lerp(zeroAngle, maxAngle, t);
        needle.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
