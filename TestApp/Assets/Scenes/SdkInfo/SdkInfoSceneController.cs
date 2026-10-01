using JustTrack;
using UnityEngine;
using TMPro;

public class SdkInfoSceneController : MonoBehaviour
{
    public TextMeshProUGUI sdkVersionText;
    public TextMeshProUGUI advertiserIdText;
    public TextMeshProUGUI installIdText;
    public TextMeshProUGUI attributionListenerText;

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
#endif
        JustTrackSDK.OnAttributionResponse += OnAttributionResponse;
        JustTrackSDK.GetAttribution(OnAttributionResponse, (error) =>
        {
            attributionListenerText.text = $"Error: {error}";
        });
    }

    private void OnDestroy()
    {
        JustTrackSDK.OnAttributionResponse -= OnAttributionResponse;
    }

    private void OnAttributionResponse(AttributionResponse attribution)
    {
        attributionListenerText.text = $"UserType={attribution.UserType}, Campaign={attribution.Campaign.Name}, Channel={attribution.Channel.Name}";
    }
}
