using Monaverse.Api.Configuration;
using UnityEngine;

namespace Monaverse.Core
{
    public class MonaverseManager : MonoBehaviour
    {
        [Tooltip("Whether the SDK should initialize on awake or not")]
        [SerializeField] private bool _initializeOnAwake;
        
        [Tooltip(" Monaverse Application ID.")]
        public string monaApplicationId = null;

        [Tooltip("The Monaverse API environment to use")]
        public ApiEnvironment apiEnvironment = ApiEnvironment.Staging;
        
        [Tooltip("Whether to show the sdk debug logs")]
        public bool showDebugLogs;
        
        [Tooltip("The default chain to use")]
        public MonaWalletSDK.SupportedChainId defaultChain = MonaWalletSDK.SupportedChainId.Ethereum;

        public static MonaverseManager Instance { get; private set; }
        public MonaWalletSDK SDK { get; private set; }


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.LogWarning("Two MonaverseManager instances were found, removing this one.");
                Destroy(gameObject);
                return;
            }

            if (_initializeOnAwake)
                Initialize();
        }
        
        /// <summary>
        /// This must be called before using the SDK
        /// If initializeOnAwake is set to true, this will be called automatically.
        /// If initializeOnAwake is set to false, this must be called manually from your code
        /// Please do not call any SDK functions from within the Awake function, including this.
        /// </summary>
        public static void Initialize()
        {
            if (Instance == null)
            {
                MonaDebug.LogError("A MonaverseManager component must be attached to a GameObject in a scene");
                return;
            }
            
            var sdkOptions = new MonaWalletSDK.SDKOptions
            {
                applicationId = Instance.monaApplicationId,
                apiEnvironment = Instance.apiEnvironment,
                showDebugLogs = Instance.showDebugLogs,
                defaultChain = Instance.defaultChain
            };

            MonaDebug.IsEnabled = Instance.showDebugLogs;
            Instance.SDK = new MonaWalletSDK(sdkOptions);
        }

        public void ResetLogin()
        {
            SDK.Logout();
        }
    }
}