#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif
using UnityEngine;

internal class PostponedInitSceneController : MonoBehaviour
{
    public static PostponedInitSceneController Instance { get; private set; }

    public static void InstantiateSDKFromResources()
    {
        var prefab = Resources.Load<GameObject>("JustTrackSDK");

        if (prefab != null)
        {
            Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else
        {
            Debug.LogError("Failed to load the SDK prefab from Resources!");
        }
    }

    public void RequestAuthorizationTracking()
    {
#if UNITY_IOS
        var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
        if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED) 
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif
    }

    public void RequestAuthorizationTrackingWithSDK()
    {
#if UNITY_IOS
        JustTrack.JustTrackSDK.RequestTrackingAuthorization((success) =>
        {
            Debug.Log("Authorization succeeded: " + success);
        });
#endif
    }

    public void RequestAuthorizationTrackingWithDelay(float delay)
    {
        Invoke(nameof(RequestAuthorizationTracking), delay);
    }

    public void RequestAuthorizationTrackingViaSDKWithDelay(float delay)
    {
        Invoke(nameof(RequestAuthorizationTrackingWithSDK), delay);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        IronSourceAdQuality.Initialize(TestAppCredentials.IronSourceAdQualityAppKey);

        var settings = Resources.Load<JustTrack.JustTrackSettings>("JustTrackSettings");
        if (settings != null && settings.UseRuntimeConstructor)
        {
            JustTrack.JustTrackSDK.Init(
#if UNITY_IOS
                pApiKey: TestAppCredentials.JustTrackTestAppIosToken,
#elif UNITY_WEBGL
                pApiKey: TestAppCredentials.JustTrackTestAppWebGLToken,
#else
                pApiKey: TestAppCredentials.JustTrackTestAppAndroidToken,
#endif
                pTrackingId: null,
                pTrackingProvider: null,
                pCustomUserId: null,
                pAutomaticInAppPurchaseTracking: true,
                pManualStart: false,
                pEnableConsoleLogging: true,
                pEnableConnectionTracking: true,
                pServerUrl: TestAppCredentials.JustTrackTestAppServerUrl,
#if UNITY_WEBGL
                pBundleId: "io.justtrack.app",
#elif UNITY_ANDROID
                pBundleId: "io.justtrack.test.unity",
#else
                pBundleId: UnityEngine.Application.identifier,
#endif
                pApplicationVersion: null
            );
        }
    }
}
