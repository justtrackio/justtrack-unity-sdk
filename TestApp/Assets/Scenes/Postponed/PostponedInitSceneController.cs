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
        }
    }
}
