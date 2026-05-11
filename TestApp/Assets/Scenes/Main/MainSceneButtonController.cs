using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneButtonController : MonoBehaviour
{
    public void OnClickSdk()
    {
        SceneManager.LoadScene("SdkInfoScene");
    }

    public void OnClickPrivacy()
    {
        SceneManager.LoadScene("PrivacyScene");
    }

    public void OnClickGameEvents()
    {
        SceneManager.LoadScene("GameEventsScene");
    }

    public void OnClickMiscellaneousEvents()
    {
        SceneManager.LoadScene("MiscellaneousEventsScene");
    }

    public void OnClickIronSource()
    {
        SceneManager.LoadScene("IronSourceScene");
    }

    public void OnClickAppLovin()
    {
        SceneManager.LoadScene("AppLovinScene");
    }

    public void OnClickInAppPurchases()
    {
        SceneManager.LoadScene("InAppPurchasesScene");
    }

    public void OnClickCrashes()
    {
        SceneManager.LoadScene("CrashesScene");
    }

    public void OnClickGoogleAds()
    {
        SceneManager.LoadScene("GoogleAdsScene");
    }

    public void OnClickUnityAds()
    {
        SceneManager.LoadScene("UnityAdsScene");
    }

    public void OnClickAtt()
    {
        SceneManager.LoadScene("AttScene");
    }

    public void OnClickRemoteConfig()
    {
        SceneManager.LoadScene("RemoteConfigScene");
    }
}
