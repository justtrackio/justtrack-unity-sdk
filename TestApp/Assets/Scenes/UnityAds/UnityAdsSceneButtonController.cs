using JustTrack;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class UnityAdsButtonController : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private bool isInitialized = false;
    private string gameId;

#if UNITY_ANDROID
    private const string rewardedVideoPlacementId = "Rewarded_Android";
    private const string interstitialPlacementId = "Interstitial_Android";
    private const string bannerPlacementId = "Banner_Android";
#elif UNITY_IOS
    private const string rewardedVideoPlacementId = "Rewarded_iOS";
    private const string interstitialPlacementId = "Interstitial_iOS";
    private const string bannerPlacementId = "Banner_iOS";
#else
    private const string rewardedVideoPlacementId = "";
    private const string interstitialPlacementId = "";
    private const string bannerPlacementId = "";
#endif

    private void Start()
    {
        InitializeUnityAds();
    }

    private void InitializeUnityAds()
    {
#if UNITY_ANDROID
        gameId = TestAppCredentials.UnityAdsAndroidGameId;
#elif UNITY_IOS
        gameId = TestAppCredentials.UnityAdsIosGameId;
#else
        gameId = "";
#endif
        if (!string.IsNullOrEmpty(gameId) && !isInitialized)
        {
            Debug.Log("Initializing Unity Ads with game ID: " + gameId);
            Advertisement.Initialize(gameId, true, this);
            isInitialized = true;
        }
        else if (string.IsNullOrEmpty(gameId))
        {
            Debug.LogError("Unity Ads game ID is empty!");
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        Advertisement.Load(rewardedVideoPlacementId, this);
        Advertisement.Load(interstitialPlacementId, this);
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
#if !UNITY_WEBGL
        JustTrackSDK.IntegrateWithUnityAds(() =>
        {
            Debug.LogWarning("IntegrateWithUnityAds: (Success) ");
        },
        (error) =>
        {
            Debug.LogWarning("IntegrateWithUnityAds: (Failure) " + error);
        });
#endif
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"Ad Loaded: {placementId}");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Failed to load Ad Unit {placementId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Failed to show Ad Unit {placementId}: {error.ToString()} - {message}");
        Advertisement.Load(placementId, this);
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"Ad Started: {placementId}");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"Ad Clicked: {placementId}");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"Ad Completed: {placementId} with state: {showCompletionState}");
        Advertisement.Load(placementId, this);
    }

    public void OnClickRewarded()
    {
        Advertisement.Show(rewardedVideoPlacementId, this);
    }

    public void OnClickInterstitial()
    {
        Advertisement.Show(interstitialPlacementId, this);
    }

    public void OnClickBanner()
    {
        Advertisement.Banner.Load(bannerPlacementId,
            new BannerLoadOptions
            {
                loadCallback = () =>
                {
                    Debug.Log("Banner loaded");
                    Advertisement.Banner.Show(bannerPlacementId);
                },
                errorCallback = (error) => Debug.LogError($"Banner Error: {error}")
            });
    }

    public void OnClickClose()
    {
        Advertisement.Banner.Hide();
        SceneManager.LoadScene("MainScene");
    }
}
