using JustTrack;
using UnityEngine;
using TMPro;

public class SdkInfoSceneController : MonoBehaviour
{
    public TextMeshProUGUI sdkVersionText;
    public TextMeshProUGUI advertiserIdText;
    public TextMeshProUGUI installIdText;
    public TextMeshProUGUI testGroupIdText;

    private void Start()
    {
        Debug.Log("Sdk info screen start");
        sdkVersionText.text = JustTrackSDK.GetVersion();
#if !UNITY_WEBGL
        JustTrackSDK.GetAdvertiserIdInfo((info) =>
        {
            advertiserIdText.text = info.AdvertiserId == null ? "null" : info.AdvertiserId;
        }, (error) =>
        {
            advertiserIdText.text = error;
        });
        JustTrackSDK.GetInstallInstanceId((installInstanceId) =>
        {
            installIdText.text = installInstanceId;
        }, (error) =>
        {
            installIdText.text = error;
        });
        JustTrackSDK.GetTestGroupId((testGroupId) =>
        {
            testGroupIdText.text = $"{testGroupId}";
        }, (error) =>
        {
            testGroupIdText.text = error;
        });
#endif
    }
}
