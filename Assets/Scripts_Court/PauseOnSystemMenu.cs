using UnityEngine;

public class PauseOnSystemMenu : MonoBehaviour
{
    [Header("Meta XR Settings")]
    [Tooltip("Should the app pause when input focus is lost (recommended for single-player)")]
    public bool pauseOnInputFocusLost = true;

    [Tooltip("Should the app pause when VR focus is lost (when user exits to Home)")]
    public bool pauseOnVRFocusLost = true;

    [Header("Audio Management")]
    [Tooltip("Pause AudioManager.globalPlay instead of AudioListener")]
    public bool useAudioManagerInstead = true;

    [Header("Debug")]
    public bool enableDebugLogging = true;

    // State tracking
    private bool wasGlobalPlayActive = false;
    private float previousTimeScale = 1f;
    private bool isPaused = false;

    void Start()
    {
        // Verify OVRManager exists
        if (OVRManager.instance == null)
        {
            LogDebug("OVRManager not found! This script requires OVRManager to function properly.");
            enabled = false;
            return;
        }

        LogDebug("Meta XR System Pause initialized successfully");
    }

    void OnEnable()
    {
        // Subscribe to Meta XR specific focus events
        if (pauseOnInputFocusLost)
        {
            OVRManager.InputFocusLost += OnInputFocusLost;
            OVRManager.InputFocusAcquired += OnInputFocusAcquired;
        }

        if (pauseOnVRFocusLost)
        {
            OVRManager.VrFocusLost += OnVRFocusLost;
            OVRManager.VrFocusAcquired += OnVRFocusAcquired;
        }

        LogDebug("Meta XR focus event listeners registered");
    }

    void OnDisable()
    {
        // Unsubscribe from Meta XR events
        if (pauseOnInputFocusLost)
        {
            OVRManager.InputFocusLost -= OnInputFocusLost;
            OVRManager.InputFocusAcquired -= OnInputFocusAcquired;
        }

        if (pauseOnVRFocusLost)
        {
            OVRManager.VrFocusLost -= OnVRFocusLost;
            OVRManager.VrFocusAcquired -= OnVRFocusAcquired;
        }

        LogDebug("Meta XR focus event listeners removed");
    }

    #region Meta XR Focus Event Handlers

    private void OnInputFocusLost()
    {
        LogDebug("Meta XR Input Focus Lost - User opened system menu");
        PauseApp("Input Focus Lost");
    }

    private void OnInputFocusAcquired()
    {
        LogDebug("Meta XR Input Focus Acquired - User returned from system menu");
        ResumeApp("Input Focus Acquired");
    }

    private void OnVRFocusLost()
    {
        LogDebug("Meta XR VR Focus Lost - User exited to Home or switched apps");
        PauseApp("VR Focus Lost");
    }

    private void OnVRFocusAcquired()
    {
        LogDebug("Meta XR VR Focus Acquired - User returned to app");
        ResumeApp("VR Focus Acquired");
    }

    #endregion

    #region Pause/Resume Logic

    private void PauseApp(string reason)
    {
        if (isPaused) return; // Already paused

        isPaused = true;
        previousTimeScale = Time.timeScale;

        // Handle AudioManager if available and preferred
        if (useAudioManagerInstead && AudioManager.i != null)
        {
            wasGlobalPlayActive = AudioManager.i.globalPlay;
            if (wasGlobalPlayActive)
            {
                AudioManager.i.globalPlay = false;
                LogDebug("AudioManager.globalPlay set to false");
            }
        }
        else
        {
            // Fallback to AudioListener
            AudioListener.pause = true;
            LogDebug("AudioListener.pause set to true");
        }

        // Pause time
        Time.timeScale = 0f;

        LogDebug($"App paused due to: {reason}");

        // Optional: Notify other systems
        OnAppPaused?.Invoke();
    }

    private void ResumeApp(string reason)
    {
        if (!isPaused) return; // Not paused

        isPaused = false;

        // Restore time scale
        Time.timeScale = previousTimeScale;

        // Handle AudioManager if available and preferred
        if (useAudioManagerInstead && AudioManager.i != null)
        {
            if (wasGlobalPlayActive)
            {
                AudioManager.i.globalPlay = true;
                LogDebug("AudioManager.globalPlay restored to true");
            }
        }
        else
        {
            // Fallback to AudioListener
            AudioListener.pause = false;
            LogDebug("AudioListener.pause set to false");
        }

        LogDebug($"App resumed due to: {reason}");

        // Optional: Notify other systems
        OnAppResumed?.Invoke();
    }

    #endregion

    #region Public Events (Optional)

    public System.Action OnAppPaused;
    public System.Action OnAppResumed;

    #endregion

    #region Debug and Status

    private void LogDebug(string message)
    {
        if (enableDebugLogging)
        {
            Debug.Log($"[Meta XR Pause] {message}");
        }
    }

    // Public method to check current focus states
    public void LogCurrentFocusStates()
    {
        if (OVRManager.instance != null)
        {
            LogDebug($"Current Focus States:");
            LogDebug($"  Has Input Focus: {OVRManager.hasInputFocus}");
            LogDebug($"  Has VR Focus: {OVRManager.hasVrFocus}");
            LogDebug($"  Is Paused: {isPaused}");
            LogDebug($"  Time Scale: {Time.timeScale}");

            if (AudioManager.i != null)
            {
                LogDebug($"  AudioManager.globalPlay: {AudioManager.i.globalPlay}");
            }
        }
    }

    // Context menu for testing in editor
    [ContextMenu("Log Focus States")]
    public void LogFocusStatesFromMenu()
    {
        LogCurrentFocusStates();
    }

    [ContextMenu("Test Pause")]
    public void TestPause()
    {
        PauseApp("Manual Test");
    }

    [ContextMenu("Test Resume")]
    public void TestResume()
    {
        ResumeApp("Manual Test");
    }

    #endregion
}
