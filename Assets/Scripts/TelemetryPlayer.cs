using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TelemetrySample
{
    public float time;
    public float rpm;
    public float speed;
    public float coolant_temp;
    public float throttle_pos;
    public float intake_air_temp;
}

[Serializable]
public class SampleWrapper
{
    public List<TelemetrySample> samples;
}

public class TelemetryPlayer : MonoBehaviour
{
    [Tooltip("Name of the JSON in StreamingAssets")]
    public string fileName = "telemetry.json";
    [Tooltip("Playback rate in Hz")]
    public float sampleRate = 10f;

    [Serializable]
    public class SampleEvent : UnityEvent<TelemetrySample> { }
    public SampleEvent OnSample = new SampleEvent();

    private List<TelemetrySample> _samples;
    private int _index;

    void Start()
    {
        LoadSamples();
        StartCoroutine(PlayRoutine());
    }

    void LoadSamples()
    {
        var path = Path.Combine(Application.streamingAssetsPath, fileName);
        var json = File.ReadAllText(path);
        var wrapper = JsonUtility.FromJson<SampleWrapper>(json);
        _samples = wrapper.samples;
        Debug.Log($"Loaded {_samples.Count} telemetry samples.");
    }

    IEnumerator PlayRoutine()
    {
        var delay = new WaitForSeconds(1f / sampleRate);
        while (_index < _samples.Count)
        {
            OnSample.Invoke(_samples[_index]);
            _index++;
            yield return delay;
        }
    }
}
