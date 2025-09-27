using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using UnityEngine.XR.Management;

public class MetaOrientationResetBlock : MonoBehaviour
{
    [Header("OpenXR Orientation Reset Settings")]
    [Tooltip("Enable debug logging for orientation reset events")]
    public bool enableDebugLogging = true;

    [Tooltip("Custom actions to perform when orientation is reset")]
    public UnityEngine.Events.UnityEvent OnOrientationReset;

    [Header("Manual Reset Options")]
    [Tooltip("Allow manual reset via double-tap of primary button")]
    public bool enableManualReset = true;

    [Tooltip("Time window for double-tap detection (seconds)")]
    public float doubleTapWindow = 0.5f;

    [Tooltip("Controller button for manual reset")]
    public InputFeatureUsage<bool> resetButton = CommonUsages.primaryButton;

    // Private variables
    private float lastButtonTapTime = 0f;
    private int buttonTapCount = 0;
    private bool wasDeviceActive = false;

    // XR Input Devices
    private InputDevice hmd;
    private InputDevice rightController;
    private InputDevice leftController;

    // Cache the camera reference
    private Camera mainCameraCache;

    void Start()
    {
        mainCameraCache = Camera.main;
        // Cache other frequently accessed objects

        if (!XRSettings.enabled)
        {
            LogDebug("XR is not enabled! This script requires XR to function.");
            enabled = false;
            return;
        }

        LogDebug("Meta Orientation Reset Block initialized successfully");
        StartCoroutine(InitializeXRDevices());
    }

    IEnumerator InitializeXRDevices()
    {
        // Wait for XR to be fully initialized
        while (!XRSettings.isDeviceActive)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Get XR devices
        RefreshInputDevices();
        LogDebug("XR devices initialized");
    }

    void Update()
    {
        if (Time.frameCount % 300 == 0) // Reduced from 120 to 300 frames
        {
            RefreshInputDevices();
        }

        // Refresh devices if needed
        if (Time.frameCount % 120 == 0) // Every 2 seconds at 60 FPS
        {
            RefreshInputDevices();
        }

        // Check for device recentering
        DetectSystemRecenter();

        // Manual reset detection
        if (enableManualReset)
        {
            DetectManualReset();
        }
    }

    #region Device Management

    void RefreshInputDevices()
    {
        var inputDevices = new System.Collections.Generic.List<InputDevice>();
        InputDevices.GetDevices(inputDevices);

        foreach (var device in inputDevices)
        {
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.HeadMounted))
            {
                hmd = device;
            }
            else if (device.characteristics.HasFlag(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller))
            {
                rightController = device;
            }
            else if (device.characteristics.HasFlag(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller))
            {
                leftController = device;
            }
        }
    }

    #endregion

    #region Recenter Detection

    void DetectSystemRecenter()
    {
        // Check if XR device became active (can indicate recenter)
        bool isDeviceActive = XRSettings.isDeviceActive;

        if (!wasDeviceActive && isDeviceActive)
        {
            LogDebug("XR device became active - potential recenter detected");
            ExecuteOrientationReset("XR Device Activation");
        }

        wasDeviceActive = isDeviceActive;

        // Alternative: Check for tracking state changes
        if (hmd.isValid)
        {
            if (hmd.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked))
            {
                // You can add logic here for tracking state changes
            }
        }
    }

    void DetectManualReset()
    {
        bool buttonPressed = false;

        // Check right controller primary button
        if (rightController.isValid)
        {
            rightController.TryGetFeatureValue(resetButton, out buttonPressed);
        }

        // If right controller failed, try left controller
        if (!buttonPressed && leftController.isValid)
        {
            leftController.TryGetFeatureValue(resetButton, out buttonPressed);
        }

        if (buttonPressed)
        {
            float currentTime = Time.time;

            if (currentTime - lastButtonTapTime < doubleTapWindow)
            {
                buttonTapCount++;
                if (buttonTapCount >= 2)
                {
                    LogDebug("Manual orientation reset triggered by double-tap");
                    ExecuteManualOrientationReset();
                    buttonTapCount = 0;
                }
            }
            else
            {
                buttonTapCount = 1;
            }

            lastButtonTapTime = currentTime;
        }

        // Reset tap count if window expires
        if (Time.time - lastButtonTapTime > doubleTapWindow)
        {
            buttonTapCount = 0;
        }
    }

    #endregion

    #region Reset Execution

    void ExecuteOrientationReset(string method)
    {
        LogDebug($"Executing orientation reset (triggered by: {method})");

        // Custom reset logic
        ResetPlayerOrientation();

        // Invoke custom events
        OnOrientationReset?.Invoke();

        // Show visual feedback
        StartCoroutine(ShowResetFeedback());
    }

    void ExecuteManualOrientationReset()
    {
        LogDebug("Executing manual orientation reset");

        // Try to trigger system recenter using OpenXR
        TriggerSystemRecenter();

        // Custom reset logic
        ResetPlayerOrientation();

        // Invoke custom events
        OnOrientationReset?.Invoke();

        // Show feedback
        StartCoroutine(ShowResetFeedback());
    }

    void TriggerSystemRecenter()
    {
        // For OpenXR, we need to use the XR subsystem
        var xrSubsystem = XRGeneralSettings.Instance?.Manager?.activeLoader?.GetLoadedSubsystem<XRInputSubsystem>();
        if (xrSubsystem != null)
        {
            // Some XR runtimes support recentering through input subsystem
            LogDebug("Attempting to trigger system recenter via XR subsystem");
            // Note: Specific recenter methods depend on the XR runtime
        }
        else
        {
            LogDebug("XR subsystem not available for recentering");
        }
    }

    void ResetPlayerOrientation()
    {
        // Find the camera rig (using the Meta Building Block structure)
        GameObject cameraRig = GameObject.Find("[BuildingBlock] Camera Rig");
        if (cameraRig == null)
        {
            LogDebug("Camera Rig not found - looking for XR Origin");
            cameraRig = FindObjectOfType<Camera>()?.transform.root.gameObject;
        }

        if (cameraRig != null)
        {
            Transform trackingSpace = cameraRig.transform.Find("TrackingSpace");
            if (trackingSpace == null)
            {
                trackingSpace = cameraRig.transform; // Fallback to camera rig itself
            }

            // Get current head position and rotation
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Vector3 currentHeadPos = mainCamera.transform.position;
                Quaternion currentHeadRot = mainCamera.transform.rotation;

                // Reset tracking space rotation to match current head direction (Y-axis only)
                Vector3 resetDirection = Vector3.ProjectOnPlane(currentHeadRot * Vector3.forward, Vector3.up);
                if (resetDirection != Vector3.zero)
                {
                    Quaternion resetRotation = Quaternion.LookRotation(resetDirection, Vector3.up);
                    trackingSpace.rotation = resetRotation;

                    LogDebug($"Player orientation reset - New forward direction: {resetDirection}");
                }
            }
        }

        // Reset any game-specific orientation data
        ResetGameSpecificOrientation();
    }

    void ResetGameSpecificOrientation()
    {
        // Add any game-specific reset logic here
        LogDebug("Game-specific orientation reset completed");
    }

    #endregion

    #region Visual Feedback

    IEnumerator ShowResetFeedback()
    {
        LogDebug("Orientation Reset Complete!");

        // Flash any UI canvases briefly
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            if (canvas.gameObject.activeSelf && canvas.worldCamera != null)
            {
                StartCoroutine(FlashCanvas(canvas));
            }
        }

        yield return null;
    }

    IEnumerator FlashCanvas(Canvas canvas)
    {
        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
        }

        float originalAlpha = canvasGroup.alpha;

        // Brief flash effect
        canvasGroup.alpha = 0.7f;
        yield return new WaitForSeconds(0.1f);
        canvasGroup.alpha = originalAlpha;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Manually trigger orientation reset from external scripts
    /// </summary>
    public void TriggerOrientationReset()
    {
        ExecuteManualOrientationReset();
    }

    /// <summary>
    /// Check if XR is properly initialized
    /// </summary>
    public bool IsXRReady()
    {
        return XRSettings.isDeviceActive && hmd.isValid;
    }

    #endregion

    #region Debug Methods

    void LogDebug(string message)
    {
        if (enableDebugLogging)
        {
            Debug.Log($"[Meta Orientation Reset] {message}");
        }
    }

    [ContextMenu("Test Manual Reset")]
    public void TestManualReset()
    {
        TriggerOrientationReset();
    }

    [ContextMenu("Log XR Status")]
    public void LogXRStatus()
    {
        LogDebug($"XR Enabled: {XRSettings.enabled}");
        LogDebug($"XR Device Active: {XRSettings.isDeviceActive}");
        LogDebug($"HMD Valid: {hmd.isValid}");
        LogDebug($"Right Controller Valid: {rightController.isValid}");
        LogDebug($"Left Controller Valid: {leftController.isValid}");
    }

    #endregion
}
