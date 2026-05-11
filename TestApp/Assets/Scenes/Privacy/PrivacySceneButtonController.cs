using JustTrack;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PrivacyButtonController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI startStopButtonText;

    private void Start()
    {
        UpdateButtonText();
    }

    public void OnClickStartStop()
    {
        if (JustTrackSDK.IsRunning())
        {
            JustTrackSDK.Stop();
        }
        else
        {
            JustTrackSDK.Start();
        }

        OnClickClose();
    }

    public void OnClickAnonymize()
    {
        JustTrackSDK.Anonymize(
            () =>
            {
                Log("Anonymize (Success)");
            },
            (message) =>
            {
                Log("Anonymize (Failure) " + message);
            });
    }

    public void OnClickTest()
    {
#if !UNITY_WEBGL
        JustTrackSDK.GetInstallInstanceId(
            (installInstanceId) =>
            {
                Log("GetInstallInstanceId: (Success) " + installInstanceId);
            },
            (error) =>
            {
                Log("GetInstallInstanceId: (Failure) " + error);
            });

        JustTrackSDK.GetTestGroupId(
            (testGroupId) =>
            {
                Log("GetTestGroupId: (Success) " + testGroupId);
            },
            (error) =>
            {
                Log("GetTestGroupId: (Failure) " + error);
            });
        JustTrackSDK.GetRetargetingParameters(
            (retargetingParams) =>
            {
                Log("GetRetargetingParameters: (Success) " + retargetingParams);
            },
            (error) =>
            {
                Log("GetRetargetingParameters: (Failure) " + error);
            });

        Log("GetPreliminaryRetargetingParameters: " + JustTrackSDK.GetPreliminaryRetargetingParameters());

        JustTrackSDK.GetAdvertiserIdInfo(
            (advertiserIdInfo) =>
            {
                Log($"GetAdvertiserIdInfo: (Success) AdvertiserId: {advertiserIdInfo.AdvertiserId} IsLimitedAdTracking: {advertiserIdInfo.IsLimitedAdTracking}");
            },
            (error) =>
            {
                Log("GetAdvertiserIdInfo: (Failure) " + error);
            });
#endif
        JustTrackSDKBehaviour.GetAttribution(
            (attributionResponse) =>
            {
                Log("GetAttribution: (Success)");
            },
            (error) =>
            {
                Log("GetAttribution: (Failure) " + error);
            });

        JustTrackSDK.Track("privacy_test_event_" + (JustTrackSDK.IsRunning() ? "running" : "stopped"),
            () =>
            {
                Log("PublishEvent: (Success)");
            },
            (error) =>
            {
                Log("PublishEvent: (Failure) " + error);
            });

#if !UNITY_WEBGL
        JustTrackSDK.ForwardAdImpression(new AdImpression(AdUnit.Banner, "AppLovin"),
            () =>
            {
                Log("ForwardAdImpression: (Success)");
            },
            (error) =>
            {
                Log("ForwardAdImpression: (Failure) " + error);
            });

        JustTrackSDK.IntegrateWithIronSource(
            "00000000-0000-IRON-USER-000000000041",
             () =>
            {
                Log("IntegrateWithIronSource: (Success)");
            },
            (error) =>
            {
                Log("IntegrateWithIronSource: (Failure) " + error);
            });

        JustTrackSDK.IntegrateWithFirebase(
            () =>
            {
                Log("IntegrateWithFirebase: (Success)");
            },
            (error) =>
            {
                Log("IntegrateWithFirebase: (Failure) " + error);
            });

        JustTrackSDK.SetUserId("00000000-0000-0000-USER-000000000041");
        JustTrackSDK.SetFirebaseAppInstanceId("FIREBASE-0000-0000-USER-000000000041");
#endif
    }

    public void OnClickClose()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void UpdateButtonText()
    {
        if (startStopButtonText != null)
        {
            startStopButtonText.text = JustTrackSDK.IsRunning() ? "Stop" : "Start";
        }
    }

    private void Log(string message)
    {
        Debug.Log(">Privacy< " + message);
    }
}