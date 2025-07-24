using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CarPartToggle : MonoBehaviour
{
    [Header("Mode & Controllers")]
    public ExplodeModeController modeController;    // holds the Toggle UI
    public ExplodeController explodeController;     // does the lift + ghost

    [System.Serializable]
    public class PartEntry
    {
        public string tag;          // e.g. "Hood", "LeftDoor", "Trunk"
        public Animator animator;   // the hinge animator
        public string paramName;    // the bool parameter, e.g. "HoodOpen"
    }

    [Tooltip("One entry per movable part")]
    public List<PartEntry> parts = new List<PartEntry>();

    Camera arCamera;

    void Start()
    {
        arCamera = Camera.main;
        if (modeController == null)
            Debug.LogWarning("ExplodeModeController not assigned!");
        if (explodeController == null)
            Debug.LogWarning("ExplodeController not assigned!");
    }

    void Update()
    {

        if (!Input.GetMouseButtonDown(0)) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
        { 
                return;   
        }
        foreach (var part in parts)
        {
            if (!hit.collider.CompareTag(part.tag)) continue;

            if (modeController != null && modeController.ExplodeMode)
            {
                // Explode‐view mode: lift up & ghost
                explodeController?.ToggleExplode(hit.collider.transform);
                Debug.Log($"Explode‐view: toggled explosion on {part.tag}");
            }
            else
            {
                // Normal mode: hinge open/close
                bool wasOpen = part.animator.GetBool(part.paramName);
                part.animator.SetBool(part.paramName, !wasOpen);
                Debug.Log($"Hinge‐view: toggled {part.tag} → {!wasOpen}");
            }

            return; // only handle one part per click
        }
    }
}
