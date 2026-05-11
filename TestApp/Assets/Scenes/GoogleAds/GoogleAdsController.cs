using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;

#if !UNITY_WEBGL
public class GoogleAdsController : MonoBehaviour
{

#if UNITY_IOS
    private static readonly string BannerId = TestAppCredentials.GoogleAdsIosBannerId;
    private static readonly string InterId = TestAppCredentials.GoogleAdsIosInterstitialId;
    private static readonly string RewardId = TestAppCredentials.GoogleAdsIosRewardedId;
#elif UNITY_ANDROID
    private static readonly string BannerId = TestAppCredentials.GoogleAdsAndroidBannerId;
    private static readonly string InterId = TestAppCredentials.GoogleAdsAndroidInterstitialId;
    private static readonly string RewardId = TestAppCredentials.GoogleAdsAndroidRewardedId;
#endif

    public Button bannerButton;
    public Button interstitialButton;
    public Button rewardButton;

    BannerView bannerView;
    InterstitialAd interstitialView;
    RewardedAd rewardView;

    private bool isAdsInitialized = false;

    // Start is called before the first frame update
    private void Start()
    {
        SetButtonInteractable(false);
    }

    public void InitAds()
    {
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Init Google ads " + initStatus);
            isAdsInitialized = true;
            SetButtonInteractable(true);
        });
    }

    private void SetButtonInteractable(bool isInteractable)
    {
        bannerButton.interactable = isInteractable;
        interstitialButton.interactable = isInteractable;
        rewardButton.interactable = isInteractable;

        UpdateButtonColors(bannerButton, isInteractable);
        UpdateButtonColors(interstitialButton, isInteractable);
        UpdateButtonColors(rewardButton, isInteractable);
    }

    private static void UpdateButtonColors(Button button, bool isEnabled)
    {
        var colors = button.colors;
        if (isEnabled)
        {
            colors.normalColor = Color.white;
            colors.disabledColor = Color.gray;
        }
        else
        {
            colors.normalColor = Color.gray;
        }
        button.colors = colors;
    }

    public void CreateBannerView()
    {
        if (!isAdsInitialized) return;

        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (bannerView != null)
        {
            DestroyBannerAd();
        }

        // Create a 320x50 banner at top of the screen
        bannerView = new BannerView(BannerId, AdSize.Banner, AdPosition.Bottom);

        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };

        bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + bannerView.GetResponseInfo());
        };

        LoadBannerAd();
    }

    private void LoadBannerAd()
    {
        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        bannerView.LoadAd(adRequest);
    }

    private void DestroyBannerAd()
    {
        if (bannerView == null) return;
        Debug.Log("Destroying banner view.");
        bannerView.Destroy();
        bannerView = null;
    }

    public void CreateInterstitialView()
    {
        if (!isAdsInitialized) return;

        // Clean up the old ad before loading a new one.
        if (interstitialView != null)
        {
            interstitialView.Destroy();
            interstitialView = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(InterId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                Debug.LogError("interstitial ad failed to load an ad " + "with error : " + error);
                return;
            }

            Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());

            interstitialView = ad;

            if (interstitialView != null && interstitialView.CanShowAd())
            {
                Debug.Log("Showing interstitial ad.");
                interstitialView.Show();
            }
            else
            {
                Debug.LogError("Interstitial ad is not ready yet.");
            }
        });
    }

    public void CreateRewardView()
    {
        if (!isAdsInitialized) return;

        // Clean up the old ad before loading a new one.
        if (rewardView != null)
        {
            rewardView.Destroy();
            rewardView = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(RewardId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                        "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                    + ad.GetResponseInfo());

                rewardView = ad;

                const string rewardMsg = "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

                if (rewardView != null && rewardView.CanShowAd())
                {
                    rewardView.Show((Reward reward) =>
                    {
                        // TODO: Reward the user.
                        Debug.Log(string.Format(rewardMsg, reward.Type, reward.Amount));
                    });
                }
            });
    }
}
#endif