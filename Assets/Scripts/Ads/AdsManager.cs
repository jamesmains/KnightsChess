using System;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public InitializeAds InitializeAds;
    public InterstitialAds InterstitialAds;
    public BannerAds BannerAds;
    
    public static AdsManager Singleton { get; private set; }

    private void Awake() {
        if (Singleton != null && Singleton != this) {
            Destroy(this.gameObject);
            return;
        }
        Singleton = this;
        DontDestroyOnLoad(this.gameObject);
        InterstitialAds.LoadInterstitialAd();
        BannerAds.LoadBannerAd();
    }
}
