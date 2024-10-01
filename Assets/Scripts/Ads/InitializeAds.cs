using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Advertisements;

public class InitializeAds : MonoBehaviour, IUnityAdsInitializationListener {
    [SerializeField] [BoxGroup("Settings")]
    private string AndroidGameId;

    [SerializeField] [BoxGroup("Settings")]
    private string IOSGameId;

    [SerializeField] [FoldoutGroup("Debug")]
    private bool IsDebugMode;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private string GameId;

    private void Awake() {
        SetGameId();
        if (!Advertisement.isInitialized && Advertisement.isSupported) {
            Advertisement.Initialize(GameId, IsDebugMode, this);
        }
    }

    private void SetGameId() {
#if UNITY_ANDROID
        GameId = AndroidGameId;
#elif UNITY_IOS
        GameId = IOSGameId;
#elif UNITY_EDITOR
        GameId = AndroidGameId;
#endif
    }

    public void OnInitializationComplete() {
        print("Ads initialized");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message) {
    }
}