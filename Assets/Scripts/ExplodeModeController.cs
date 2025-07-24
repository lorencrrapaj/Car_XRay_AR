using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("X-Ray/Explode Mode Controller")]
public class ExplodeModeController : MonoBehaviour
{
    [Tooltip("The UI Toggle that switches explode-mode on/off")]
    public Toggle explodeToggle;

    /// <summary>
    /// Called by the Toggle’s onValueChanged.
    /// </summary>
    public void SetExplodeMode(bool isExplodeMode)
    {
        // keep the field in sync (optional)
        if (explodeToggle != null && explodeToggle.isOn != isExplodeMode)
            explodeToggle.isOn = isExplodeMode;

        if (isExplodeMode)
        {
            // === EXPLODE-PARTS MODE ===
            // Here you might enable your ExplodeController,
            // disable any door-hinge scripts, etc.
            Debug.Log("Explode-Parts mode ON");
        }
        else
        {
            // === OPEN-DOORS MODE ===
            // Call your door/trunk/hood-opening logic here.
            // e.g. DoorController.OpenAll()
            Debug.Log("Open-Doors mode ON");
        }
    }

    /// <summary>
    /// Other scripts can read this property if they need to know the current mode.
    /// </summary>
    public bool ExplodeMode => explodeToggle != null && explodeToggle.isOn;
}
