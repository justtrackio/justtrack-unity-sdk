using JustTrack;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AttButtonController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    public void OnClickDisplayAtt()
    {
#if UNITY_IOS
        var attStatus = JustTrackSDK.GetTrackingAuthorizationStatus();
        label.text = $"ATT [{attStatus.ToString()}]";
#endif
    }

    public void OnClickClose()
    {
        SceneManager.LoadScene("MainScene");
    }
}
