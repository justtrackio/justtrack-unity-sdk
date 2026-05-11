using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PostponedInitSceneButtonController : MonoBehaviour
{
    public GameObject runWithTrackingButton;
    public GameObject runWithSDKTrackingButton;
    public GameObject runWithPostponedTrackingButton;
    public GameObject runWithSDKPostponedTrackingButton;
    public GameObject runWithSDKWithTrackingIfNeededButton;

    private void Start()
    {
#if !UNITY_IOS
        runWithTrackingButton.SetActive(false);
        runWithSDKTrackingButton.SetActive(false);
        runWithPostponedTrackingButton.SetActive(false);
        runWithSDKPostponedTrackingButton.SetActive(false);
        runWithSDKWithTrackingIfNeededButton.SetActive(false);
#endif
    }

    public void OnClickRunWithNoTracking()
    {
        InitSdk();
    }

    public void OnClickRunWithTracking()
    {
        PostponedInitSceneController.Instance.RequestAuthorizationTracking();
        Invoke(nameof(InitSdk), 7);
    }

    public void OnClickRunWithSDKTracking()
    {
        PostponedInitSceneController.Instance.RequestAuthorizationTrackingWithSDK();
        Invoke(nameof(InitSdk), 7);
    }

    public void OnClickRunWithPostponedTracking()
    {
        PostponedInitSceneController.Instance.RequestAuthorizationTrackingWithDelay(10);
        InitSdk();
    }

    public void OnClickRunWithPostponedSDKTracking()
    {
        PostponedInitSceneController.Instance.RequestAuthorizationTrackingViaSDKWithDelay(10);
        InitSdk();
    }

    public void OnClickRequestTrackingIfNeeded()
    {
        SceneManager.LoadScene("TrackingScene");
    }

    private void InitSdk()
    {
        SceneManager.LoadScene("MainScene");
    }
}