# AR Car X-Ray Viewer

An interactive AR application built in Unity that lets users place a 3D car model in their environment, explore its parts (doors, hood, trunk), “explode” individual components or the whole vehicle, and view live OBD-II telemetry gauges.

---

## 📂 Repository Structure

```
/Assets
  /Scripts
    ARTapToPlace.cs
    ExplodeController.cs
    ExplodeModeController.cs
    FullExplodeController.cs
    GaugeViewController.cs
    TelemetryPlayer.cs
    BillboardLabel.cs
  /Prefabs
    newmiata.prefab
    partLabelPrefab.prefab
    GaugePrefab.prefab
  /StreamingAssets
    telemetry.json
  /Materials
  /Scenes
    MainScene.unity
  /Shaders
    CoolantPulse.shader
/Packages
  manifest.json
README.md
```

---

## 🚀 Quick Start

1. **Clone this repo**

   ```bash
   git clone https://github.com/your-org/ar-car-xray.git
   cd ar-car-xray
   ```

2. **Open in Unity**

   * Use **Unity 2022 LTS** with URP.
   * Let Unity resolve packages.

3. **Install Required Packages**

   * **AR Foundation 6.x**
   * **ARKit XR Plugin** (iOS) / **ARCore XR Plugin** (Android)
   * **XR Interaction Toolkit**
   * **TextMeshPro**

4. **Configure Build Settings**

   * **iOS**: Switch platform to iOS, enable **ARKit** in XR Plug-in Management.
   * **Android**: Switch to Android, install **ARCore** support.

5. **Assign References**

   * Open **MainScene**.
   * Select **AR Session Origin**:

     * Drag in your **newmiata** prefab into `ARTapToPlace → Car Prefab`.
     * Drag in **FullExplodeButton**, **ExplodeModeToggle**, **GaugesButton** UI components.
     * Tweak **Hold Threshold**, **Spawn Scale** as needed.
   * In **newmiata** prefab:

     * Assign `bodyPartsParent` and (optionally) `explosionCenter` on **FullExplodeController**.
     * Ensure **ExplodeModeController**, **FullExplodeController**, **ExplodeController**, **GaugeViewController**, and **BillboardLabel** are present on appropriate children.

6. **Run on Device**

   * Build & deploy to your AR-capable device or use Editor’s AR simulation.

---

## 🎮 How to Use

1. **Press & Hold** on a detected plane for \~0.5 s to spawn or move the car.
2. **Explode Mode Toggle**

   * OFF → tap individual parts to lift & label them.
   * ON → doors/hood/trunk open (customizable).
3. **Full Explode Button** → radial “bomb” spread of all parts.
4. **Gauges Button** → show/hide live telemetry gauges (RPM, speed, coolant temp, throttle, intake air temp).
5. **Labels** auto-face the camera via `BillboardLabel`.

---

## 🛠️ Architecture & Key Scripts

* **ARTapToPlace.cs**
  Press-and-hold placement + runtime hookup of UI callbacks.

* **ExplodeController.cs**
  Single-part explode/collapse toggling + material ghosting + label spawning.

* **FullExplodeController.cs**
  Radial explosion from computed pivot → smooth Lerp + ghosting.

* **ExplodeModeController.cs**
  Switch between door-opening mode and parts-explode mode via Toggle.

* **GaugeViewController.cs** & **TelemetryPlayer.cs**
  Bind JSON-streamed OBD-II data to floating gauge prefabs.

* **BillboardLabel.cs**
  Keeps labels upright and facing the main camera.

---

## 📝 Spec Sheet Coverage

This project implements all requirements from the spec sheet:

* **3D Car Anatomy**
* **AR placement** (markerless)
* **Component highlighting & labeling**
* **Full and partial explosions**
* **Live OBD-II telemetry**
* **World-space UI & billboarding**

Plus extras:

* Press-and-hold placement
* Dynamic spawn-time scaling
* Runtime UI wiring (no prefab drag-and-drop hacks)

---



