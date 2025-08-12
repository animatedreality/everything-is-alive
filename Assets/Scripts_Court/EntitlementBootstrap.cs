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

    private bool _completed;

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
        try
        {
            Core.Initialize();
        }
        catch (System.Exception e)
        {
            Debug.LogError("[EntitlementBootstrap] Platform Core.Initialize() failed: " + e.Message);
            yield return FailAndQuit("Initialization failed.");
            yield break;
        }

        var request = Entitlements.IsUserEntitledToApplication();
        request.OnComplete(OnEntitlementResult);

        float elapsed = 0f;
        while (!_completed && elapsed < timeoutSeconds)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (!_completed)
        {
            Debug.LogError("[EntitlementBootstrap] Entitlement timed out.");
            yield return FailAndQuit("Entitlement timed out.");
        }
    }

    private void OnEntitlementResult(Message msg)
    {
        _completed = true;

        if (msg.IsError)
        {
            var error = msg.GetError();
            Debug.LogError($"[EntitlementBootstrap] Failed: {error?.Message ?? "Unknown error"} ({error?.Code})");
            StartCoroutine(FailAndQuit("Entitlement failed."));
            return;
        }

        Debug.Log("[EntitlementBootstrap] Entitlement success. Continuing app load.");
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

        var rt = text.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.1f, 0.1f);
        rt.anchorMax = new Vector2(0.9f, 0.9f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        float t = 0f;
        while (t < 1.5f) { t += Time.unscaledDeltaTime; yield return null; }

        if (quitOnFailure) Application.Quit();
    }
#endif
}
