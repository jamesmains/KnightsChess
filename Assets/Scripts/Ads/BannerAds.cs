using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener {
    [SerializeField] [BoxGroup("Settings")]
    private string AndroidAdUnitId;

    [SerializeField] [BoxGroup("Settings")]
    private string IOSAdUnitId;

    [SerializeField] [BoxGroup("Status")] [ReadOnly]
    private string AdUnitId;

    private void Awake() {
        SetAdUnitId();
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
    }

    private void SetAdUnitId() {
#if UNITY_ANDROID
        AdUnitId = AndroidAdUnitId;
#elif UNITY_IOS
        AdUnitId = IOSAdUnitId;
#endif
    }

    public void LoadBannerAd() {
        BannerLoadOptions options = new BannerLoadOptions {
            loadCallback = BannerLoaded,
            errorCallback = BannerLoadedError,
        };
        Advertisement.Banner.Load(AdUnitId, options);
    }

    public void ShowBannerAd() {
        BannerOptions options = new BannerOptions {
            showCallback = BannerShown,
            clickCallback = BannerClicked,
            hideCallback = BannerHidden
        };
        Advertisement.Banner.Show(AdUnitId, options);
    }

    public void HideBannerAd() {
        Advertisement.Banner.Hide();
    }

    private void BannerHidden() { }

    private void BannerClicked() { }

    private void BannerShown() { }

    private void BannerLoadedError(string message) { }

    private void BannerLoaded() { }

    public void OnUnityAdsAdLoaded(string placementId) { }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) { }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) { }

    public void OnUnityAdsShowStart(string placementId) { }

    public void OnUnityAdsShowClick(string placementId) { }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState) { }
}