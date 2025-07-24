using UnityEngine;

public class GaugeBinder : MonoBehaviour
{
    public TelemetryPlayer player;      // assign your TelemetryPlayer GameObject
    public string gaugeTag;             // e.g. "rpm", "speed", "coolant_temp"
    public Transform needle;            // drag in the needle Transform
    public float minValue = 0f;         // data value at zero-angle
    public float maxValue = 7000f;      // data value at full-angle (change per gauge)
    public float minAngle = 0f;         // needle angle at minValue
    public float maxAngle = -270f;      // needle angle at maxValue

    void OnEnable()
    {
        player.OnSample.AddListener(OnSample);
    }

    void OnDisable()
    {
        player.OnSample.RemoveListener(OnSample);
    }

    void OnSample(TelemetrySample s)
    {
        // pick the right field from the sample
        float val = gaugeTag switch
        {
            "rpm" => s.rpm,
            "speed" => s.speed,
            "coolant_temp" => s.coolant_temp,
            "throttle_pos" => s.throttle_pos,
            "intake_temp" => s.intake_air_temp,
            _ => 0f
        };

        // normalize and clamp
        float t = Mathf.Clamp01((val - minValue) / (maxValue - minValue));
        float angle = Mathf.Lerp(minAngle, maxAngle, t);

        // apply rotation around Z (or whichever axis your needle uses)
        needle.localRotation = Quaternion.Euler(0, -angle, 0);
    }
}
