// TouchManipulation.cs
using UnityEngine;

public class TouchManipulation : MonoBehaviour
{
    [Header("Speeds")]
    [SerializeField] float rotationSpeed = 0.2f;
    [SerializeField] float zoomSpeed = 0.05f;

    [Header("Scale Limits")]
    [SerializeField] float minScale = 0.1f;
    [SerializeField] float maxScale = 2.0f;

    Vector2 lastMousePos;

    void Update()
    {
        // ----------------------
        // Editor / Mouse Debug
        // ----------------------
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
            Debug.Log("Mouse down at " + lastMousePos);
        }
        if (Input.GetMouseButton(0))
        {
            Vector2 curr = (Vector2)Input.mousePosition;
            Vector2 delta = curr - lastMousePos;
            lastMousePos = curr;

            Debug.Log("Mouse drag delta: " + delta);
            // apply yaw just to prove it’s working:
            transform.Rotate(0f, delta.x * rotationSpeed * 0.5f, 0f, Space.Self);
        }
#endif

        // ----------------------
        // Touch Debug
        // ----------------------
        int tc = Input.touchCount;
        if (tc > 0)
        {
            Debug.Log("TouchCount = " + tc);
        }

        // single‑finger rotate
        if (tc == 1)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
                Debug.Log("Touch began at " + t.position);
            if (t.phase == TouchPhase.Moved)
            {
                Debug.Log($"Touch moved. delta: {t.deltaPosition}");
                float yaw = t.deltaPosition.x * rotationSpeed;
                transform.Rotate(0f, yaw, 0f, Space.Self);
            }
            if (t.phase == TouchPhase.Ended)
                Debug.Log("Touch ended");
        }
       
        }
    }

