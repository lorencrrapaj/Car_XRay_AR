using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("XRay/Full Explode Controller")]
public class FullExplodeController : MonoBehaviour
{
    [Header("Body Parts Container")]
    [Tooltip("Direct children should be hinge parents (e.g. Hood_Hinge)")]
    public Transform bodyPartsParent;

    [Header("Explosion Center (optional)")]
    [Tooltip("If assigned, parts will explode radially from this transform's position; otherwise uses computed geometric center.")]
    public Transform explosionCenter;

    [Header("Materials")]
    public Material ghostBodyMaterial;
    public Material outlineBodyMaterial; // optional

    [Header("Animation Settings")]
    [Tooltip("Radial distance parts move when exploded")]
    public float explodeDistance = 0.5f;
    [Tooltip("Duration of explode/collapse animation in seconds")]
    public float explodeDuration = 1.0f;

    // runtime
    private Vector3 _pivotLocal;
    private List<Renderer> _allRenderers = new List<Renderer>();
    private Dictionary<Renderer, Material[]> _originalMats = new Dictionary<Renderer, Material[]>();
    private List<Transform> _targets = new List<Transform>();
    private Dictionary<Transform, Vector3> _origLocalPos = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Vector3> _sphereDir = new Dictionary<Transform, Vector3>();
    private bool _exploded = false;

    void Awake()
    {
        if (bodyPartsParent == null)
        {
            Debug.LogError("FullExplodeController: BodyPartsParent not set.");
            return;
        }

        // --- 1) Compute world-space pivot ---
        Vector3 worldPivot;
        if (explosionCenter != null)
        {
            worldPivot = explosionCenter.position;
        }
        else
        {
            Bounds b = new Bounds();
            bool first = true;
            foreach (Renderer r in bodyPartsParent.GetComponentsInChildren<Renderer>())
            {
                if (first) { b = r.bounds; first = false; }
                else { b.Encapsulate(r.bounds); }
            }
            worldPivot = b.center;
        }

        // Convert pivot into the local space of bodyPartsParent
        _pivotLocal = bodyPartsParent.InverseTransformPoint(worldPivot);

        // --- 2) Cache renderers & original materials ---
        foreach (Renderer r in bodyPartsParent.GetComponentsInChildren<Renderer>())
        {
            _allRenderers.Add(r);
            _originalMats[r] = r.materials;
        }

        // --- 3) Find each hinge parent, cache positions & directions ---
        foreach (Transform hinge in bodyPartsParent)
        {
            if (hinge.name == "Other") continue;

            // find the mesh under this hinge
            Renderer meshRend = hinge.GetComponentInChildren<Renderer>();
            if (meshRend == null) continue;
            Transform meshChild = meshRend.transform;

            _targets.Add(hinge);
            _origLocalPos[hinge] = hinge.localPosition;

            // world-space direction from pivot to mesh center
            Vector3 meshWorldCenter = meshRend.bounds.center;
            Vector3 worldDir = (meshWorldCenter - worldPivot).normalized;

            // convert that into bodyPartsParent’s local direction
            Vector3 localDir = bodyPartsParent.InverseTransformDirection(worldDir);
            _sphereDir[hinge] = localDir;
        }
    }

    /// <summary>Hook to UI Button to toggle full explode/collapse.</summary>
    public void ToggleFullExplode()
    {
        StopAllCoroutines();
        if (_exploded) StartCoroutine(CollapseRoutine());
        else StartCoroutine(ExplodeRoutine());
    }

    private IEnumerator ExplodeRoutine()
    {
        // ghost non-targets
        foreach (Renderer r in _allRenderers)
        {
            if (!_sphereDir.ContainsKey(r.transform.parent))
            {
                var mats = new List<Material> { ghostBodyMaterial };
                if (outlineBodyMaterial != null) mats.Add(outlineBodyMaterial);
                r.materials = mats.ToArray();
            }
        }

        float elapsed = 0f;
        while (elapsed < explodeDuration)
        {
            float t = elapsed / explodeDuration;
            foreach (Transform hinge in _targets)
            {
                Vector3 from = _origLocalPos[hinge];
                Vector3 to = from + _sphereDir[hinge] * explodeDistance;
                hinge.localPosition = Vector3.Lerp(from, to, t);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        // finalize positions
        foreach (Transform hinge in _targets)
            hinge.localPosition = _origLocalPos[hinge] + _sphereDir[hinge] * explodeDistance;

        _exploded = true;
    }

    private IEnumerator CollapseRoutine()
    {
        float elapsed = 0f;
        while (elapsed < explodeDuration)
        {
            float t = elapsed / explodeDuration;
            foreach (Transform hinge in _targets)
            {
                Vector3 from = _origLocalPos[hinge] + _sphereDir[hinge] * explodeDistance;
                Vector3 to = _origLocalPos[hinge];
                hinge.localPosition = Vector3.Lerp(from, to, t);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        // reset positions & materials
        foreach (Transform hinge in _targets)
            hinge.localPosition = _origLocalPos[hinge];
        foreach (Renderer r in _allRenderers)
            r.materials = _originalMats[r];

        _exploded = false;
    }
}
