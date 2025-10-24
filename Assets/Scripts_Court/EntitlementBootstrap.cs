// EntitlementBootstrap.cs
#if UNITY_ANDROID && !UNITY_EDITOR
#define QUEST_RUNTIME
#endif

using System.Collections;
using UnityEngine;
using Oculus.Platform;
using Oculus.Platform.Models;

public class EntitlementBootstrap : MonoBehaviour
{
    public string failMessage = "Entitlement check failed.\nPlease make sure this account is entitled to the app.";
    public float timeoutSeconds = 10f;
    public bool quitOnFailure = true;

    private bool _entitlementCompleted;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
#if QUEST_RUNTIME
        StartCoroutine(RunEntitlementGate());
#else
        Debug.Log("[EntitlementBootstrap] Editor mode: Skipping entitlement check.");
#endif
    }

#if QUEST_RUNTIME
    private IEnumerator RunEntitlementGate()
    {
        bool initializationComplete = false;
        bool initializationFailed = false;
    
        try
        {
            Core.AsyncInitialize().OnComplete(msg =>
            {
                initializationComplete = true;
                if (msg.IsError)
                {
                    Debug.LogError($"[EntitlementBootstrap] AsyncInitialize failed: {msg.GetError().Message}");
                    initializationFailed = true;
                }
                else
                {
                    Debug.Log("[EntitlementBootstrap] Platform initialized successfully");
                }
            });
        }
        catch (System.Exception e)
        {
            Debug.LogError("[EntitlementBootstrap] Platform Core.AsyncInitialize() failed: " + e.Message);
            yield return FailAndQuit("Initialization failed.");
            yield break;
        }

        // Wait for initialization with timeout
        float elapsed = 0f;
        while (!initializationComplete && elapsed < timeoutSeconds)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (!initializationComplete)
        {
            Debug.LogError("[EntitlementBootstrap] Initialization timed out.");
            yield return FailAndQuit("Initialization timed out.");
            yield break;
        }

        if (initializationFailed)
        {
            Debug.LogError("[EntitlementBootstrap] Initialization failed.");
            yield return FailAndQuit("Initialization failed.");
            yield break;
        }

        // Start entitlement check
        Debug.Log("[EntitlementBootstrap] Starting entitlement check...");
        _entitlementCompleted = false;
        
        var request = Entitlements.IsUserEntitledToApplication();
        request.OnComplete(OnEntitlementResult);

        // Wait for entitlement check to complete with timeout
        elapsed = 0f;
        while (!_entitlementCompleted && elapsed < timeoutSeconds)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (!_entitlementCompleted)
        {
            Debug.LogError("[EntitlementBootstrap] Entitlement check timed out.");
            yield return FailAndQuit("Entitlement check timed out.");
        }
    }

    private void OnEntitlementResult(Message msg)
    {
        _entitlementCompleted = true;

        if (msg.IsError)
        {
            var error = msg.GetError();
            Debug.LogError($"[EntitlementBootstrap] Entitlement failed: {error?.Message ?? "Unknown error"} (Code: {error?.Code})");
            StartCoroutine(FailAndQuit("Entitlement failed."));
            return;
        }

        Debug.Log("[EntitlementBootstrap] Entitlement check passed. User is entitled to use this app.");
    }

    private IEnumerator FailAndQuit(string reason)
    {
        var go = new GameObject("EntitlementFailureOverlay");
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        go.AddComponent<UnityEngine.UI.CanvasScaler>();
        go.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        var textGO = new GameObject("Message");
        textGO.transform.SetParent(go.transform, false);
        var text = textGO.AddComponent<UnityEngine.UI.Text>();
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.text = failMessage + "\n(" + reason + ")";
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 18;
        text.resizeTextMaxSize = 36;
        text.color = Color.white;

        var rt = text.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.1f, 0.1f);
        rt.anchorMax = new Vector2(0.9f, 0.9f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        float t = 0f;
        while (t < 3.0f) 
        { 
            t += Time.unscaledDeltaTime; 
            yield return null; 
        }

        if (quitOnFailure) 
        {
            Debug.LogError("[EntitlementBootstrap] Quitting application due to entitlement failure.");
            UnityEngine.Application.Quit();
        }
    }
#endif
}
