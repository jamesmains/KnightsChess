using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener {
    [SerializeField] [BoxGroup("Settings")]
    private string AndroidAdUnitId;

    [SerializeField] [BoxGroup("Settings")]
    private string IOSAdUnitId;

    [SerializeField] [BoxGroup("Status")] [ReadOnly]
    private string AdUnitId;

    private void Awake() {
        SetAdUnitId();
    }

    private void SetAdUnitId() {
#if UNITY_ANDROID
        AdUnitId = AndroidAdUnitId;
#elif UNITY_IOS
        AdUnitId = IOSAdUnitId;
#endif
    }

    public void LoadInterstitialAd() {
        Advertisement.Load(AdUnitId,this);
    }

    public void ShowInterstitialAd() {
        Advertisement.Show(AdUnitId, this);
        LoadInterstitialAd();
    }

    public void OnUnityAdsAdLoaded(string placementId) {
        
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) {
        
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) {
        
    }

    public void OnUnityAdsShowStart(string placementId) {
        
    }

    public void OnUnityAdsShowClick(string placementId) {
        
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState) {
        
    }
}