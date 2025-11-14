using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class XRSessionValidator : MonoBehaviour
{
    private static XRSessionValidator _instance;
    public static XRSessionValidator Instance => _instance;

    private const float CHECK_INTERVAL = 0.5f;

    private bool _isXRReady = false;
    private float _nextCheckTime = 0f;

    public bool IsXRReady => _isXRReady;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Time.time >= _nextCheckTime)
        {
            _nextCheckTime = Time.time + CHECK_INTERVAL;
            CheckXRStatus();
        }
    }

    private void CheckXRStatus()
    {
        var xrManager = XRGeneralSettings.Instance?.Manager;
        if (xrManager == null)
        {
            _isXRReady = false;
            return;
        }

        if (!xrManager.isInitializationComplete)
        {
            _isXRReady = false;
            return;
        }

        var activeLoader = xrManager.activeLoader;
        if (activeLoader == null)
        {
            _isXRReady = false;
            return;
        }

        var xrDisplay = activeLoader.GetLoadedSubsystem<XRDisplaySubsystem>();
        _isXRReady = xrDisplay != null && xrDisplay.running;
    }

    public void WaitForXRReady(System.Action onReady)
    {
        if (_isXRReady)
        {
            onReady?.Invoke();
        }
        else
        {
            StartCoroutine(WaitForXRReadyCoroutine(onReady));
        }
    }

    private System.Collections.IEnumerator WaitForXRReadyCoroutine(System.Action onReady)
    {
        while (!_isXRReady)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        onReady?.Invoke();
    }
}
