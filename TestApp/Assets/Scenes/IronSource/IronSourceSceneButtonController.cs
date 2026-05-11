using JustTrack;
using UnityEngine;
using UnityEngine.SceneManagement;
public class IronSourceButtonController : MonoBehaviour
{
    private bool isInitialized = false;

    private void Start()
    {
        IronSourceEvents.onSdkInitializationCompletedEvent += OnSdkInitialized;

        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailableEvent;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailableEvent;

        InitializeIronSource();
    }

    private void InitializeIronSource()
    {
#if UNITY_ANDROID
        string appKey = TestAppCredentials.IronSourceAndroidAppKey;
#elif UNITY_IOS
        string appKey = TestAppCredentials.IronSourceIosAppKey;
#else
        string appKey = "";
#endif
        if (!string.IsNullOrEmpty(appKey) && !isInitialized)
        {
            Debug.Log("Initializing IronSource with app key: " + appKey);
            IronSource.Agent.init(appKey);
            isInitialized = true;
        }
        else if (string.IsNullOrEmpty(appKey))
        {
            Debug.LogError("IronSource app key is empty!");
        }
    }

    private void OnDestroy()
    {
        IronSourceEvents.onSdkInitializationCompletedEvent -= OnSdkInitialized;
        IronSourceRewardedVideoEvents.onAdAvailableEvent -= RewardedVideoOnAdAvailableEvent;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent -= RewardedVideoOnAdUnavailableEvent;
    }

    private void OnSdkInitialized()
    {
        IronSource.Agent.loadInterstitial();
        IronSource.Agent.loadRewardedVideo();
#if !UNITY_WEBGL
        JustTrackSDK.IntegrateWithIronSource(TestAppCredentials.JustTrackMediationUserId, () =>
        {
            Debug.LogWarning("IntegrateWithIronSource: (Success) ");
        },
        (error) =>
        {
            Debug.LogWarning("IntegrateWithIronSource: (Failure) " + error);
        });
#endif
    }

    private void RewardedVideoOnAdAvailableEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Rewarded video ad is available");
    }

    private void RewardedVideoOnAdUnavailableEvent()
    {
        Debug.Log("Rewarded video ad is unavailable");
    }

    public void OnClickVideo()
    {
        IronSource.Agent.showRewardedVideo();
    }

    public void OnClickInterstitial()
    {
        IronSource.Agent.showInterstitial();
    }

    public void OnClickBanner()
    {
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
    }

    public void OnClickClose()
    {
        SceneManager.LoadScene("MainScene");
    }

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }
}
