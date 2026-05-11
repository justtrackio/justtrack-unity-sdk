using UnityEngine;
using UnityEngine.SceneManagement;
using JustTrack;


public class AppLovinButtonController : MonoBehaviour
{
    public void Start()
    {
        MaxSdk.InitializeSdk();
#if !UNITY_WEBGL
        JustTrackSDK.IntegrateWithAppLovin(TestAppCredentials.JustTrackMediationUserId, () =>
            {
                Debug.LogWarning("IntegrateWithAppLovin: (Success) ");
            },
            (error) =>
            {
                Debug.LogWarning("IntegrateWithAppLovin: (Failure) " + error);
            }
        );
#endif
    }

    public void OnClickMediationDebugger()
    {
        MaxSdk.ShowMediationDebugger();
    }

    public void OnClickClose()
    {
        SceneManager.LoadScene("MainScene");
    }
}