using System.Collections.Generic;
using UnityEngine;
using TMPro;

[AddComponentMenu("X-Ray/Explode Controller")]
public class ExplodeController : MonoBehaviour
{
    [Header("Body Parts Container")]
    public Transform bodyPartsParent;    // drag in your BodyParts container

    [Header("Materials")]
    public Material ghostBodyMaterial;
    public Material outlineBodyMaterial; // optional

    [Header("Explode Settings")]
    public float explodeDistance = 0.1f;

    [Header("Label Prefab")]
    [Tooltip("Prefab for part labels (World-Space Canvas with TMP text)")]
    public GameObject partLabelPrefab;
    [Tooltip("Vertical offset above part for label placement (in meters)")]
    public float labelHeight = 0.15f;

    // runtime collections
    private List<Renderer> _bodyRenderers;
    private Dictionary<Renderer, Material[]> _originalMats;
    private Dictionary<Transform, Vector3> _originalPos;
    private HashSet<Transform> _explodableRoots;
    private bool _isExploded;
    private Transform _currentPart;
    private GameObject _currentLabel;

    void Awake()
    {
        // gather all renderers under the parent
        _bodyRenderers = new List<Renderer>(bodyPartsParent.GetComponentsInChildren<Renderer>());

        _originalMats = new Dictionary<Renderer, Material[]>();
        _originalPos = new Dictionary<Transform, Vector3>();

        // cache each renderer’s original mats & positions
        foreach (var r in _bodyRenderers)
        {
            _originalMats[r] = r.materials;
            _originalPos[r.transform] = r.transform.localPosition;
        }

        // set of valid roots for explode/collapse
        _explodableRoots = new HashSet<Transform>(_originalPos.Keys);
    }

    /// <summary>
    /// Toggle explode on a part by finding its registered root.
    /// Use this when you have a collider on a child mesh.
    /// </summary>
    public void ToggleExplodeFromChild(Transform child)
    {
        if (child == null) return;

        // climb up until we find a transform that was cached in Awake()
        Transform p = child;
        while (p != null && !_explodableRoots.Contains(p))
            p = p.parent;

        if (p != null)
            ToggleExplode(p);
    }

    /// <summary>Core explode/collapse logic on a direct root transform.</summary>
    public void ToggleExplode(Transform part)
    {
        if (_isExploded && _currentPart == part) Collapse();
        else
        {
            if (_isExploded) Collapse();
            Explode(part);
        }
    }

    void Explode(Transform part)
    {
        _currentPart = part;
        // lift the selected part
        part.localPosition = _originalPos[part] + Vector3.up * explodeDistance;

        // ghost & outline all other parts
        foreach (var r in _bodyRenderers)
        {
            if (r.transform == part)
            {
                // restore the exploded part’s original mats
                r.materials = _originalMats[r];
            }
            else
            {
                var mats = new List<Material> { ghostBodyMaterial };
                if (outlineBodyMaterial != null) mats.Add(outlineBodyMaterial);
                r.materials = mats.ToArray();
            }
        }

        // spawn the name label
        SpawnLabel(part);
        _isExploded = true;
    }

    void Collapse()
    {
        // snap the part back
        if (_currentPart != null)
            _currentPart.localPosition = _originalPos[_currentPart];

        // restore every part’s original mats
        foreach (var r in _bodyRenderers)
            r.materials = _originalMats[r];

        // destroy the label
        if (_currentLabel != null)
            Destroy(_currentLabel);

        _isExploded = false;
        _currentPart = null;
    }

    void SpawnLabel(Transform part)
    {
        if (_currentLabel != null) Destroy(_currentLabel);
        if (partLabelPrefab == null)
        {
            Debug.LogWarning("PartLabel prefab not set on ExplodeController.");
            return;
        }

        // Instantiate the label
        _currentLabel = Instantiate(partLabelPrefab);
        var textComp = _currentLabel.GetComponentInChildren<TextMeshProUGUI>();
        if (textComp != null)
            textComp.text = part.name;

        // Determine a better spawn position using renderer bounds
        Vector3 spawnPos;
        var rend = part.GetComponentInChildren<Renderer>();
        if (rend != null)
            spawnPos = rend.bounds.center + Vector3.up * labelHeight;
        else
            spawnPos = part.position + Vector3.up * labelHeight;

        _currentLabel.transform.position = spawnPos;

        // Face the camera
        var cam = Camera.main;
        if (cam != null)
        {
            _currentLabel.transform.rotation =
                Quaternion.LookRotation(spawnPos - cam.transform.position);
        }
    }
}
