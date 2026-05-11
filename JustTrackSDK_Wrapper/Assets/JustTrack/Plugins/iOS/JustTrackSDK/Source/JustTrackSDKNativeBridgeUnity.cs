#if UNITY_IOS
using UnityEngine;
using System;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;
using JustTrack;

internal class JustTrackSDKNativeBridgeUnity : MonoBehaviour {

#pragma warning disable SA1309 // Field names should not begin with underscore (required for native interop)
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_free_string(IntPtr s);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_init(string apiToken, string trackingId, string trackingProvider, string customUserId, int inactivityTimeFrameHours, int reAttributionTimeFrameDays, int reFetchAttributionDelaySeconds, int attributionRetryDelaySeconds, int automaticInAppPurchaseTracking, int manualStart, int enableConsoleLogging, string customBundleId, string customAppVersion, string customAppCode, string? customServerUrl);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_start();
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_stop();
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_anonymize();
    [DllImport("__Internal")]
    private static extern bool _justtrack_sdk_rp_is_running();
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_get_retargeting_parameters();
    [DllImport("__Internal")]
    private static extern IntPtr _justtrack_sdk_rp_get_preliminary_retargeting_parameters();
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_get_advertiser_id_info();
    [DllImport("__Internal")]
    private static extern int _justtrack_sdk_rp_get_test_group_id();
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_set_user_id(string customUserId);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_set_automatic_in_app_purchase_tracking(bool automaticInAppPurchaseTracking);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_set_firebase_app_instance_id(string firebaseAppInstanceId);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_publish_event(string name, string dimensions, double value, string? unit, string? currency, string? requestId);
    [DllImport("__Internal")]
    private static extern bool _justtrack_sdk_rp_forward_ad_impression(string adFormat, string adSdkName, string? adNetwork, string? placement, string? testGroup, string? segmentName, string? instanceName, string? bundleId, double revenue, string? currency);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_integrate_with_app_lovin(string? customUserId);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_integrate_with_firebase();
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_integrate_with_iron_source(string? customUserId);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_integrate_with_unity_ads();
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_integrate_with_google_odm();
    [DllImport("__Internal")]
    private static extern int _justtrack_sdk_rp_request_tracking_authorization();
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_forward_transaction_id(string transactionId, string productId, int quantity);
    [DllImport("__Internal")]
    private static extern int _justtrack_sdk_rp_get_tracking_authorization_status();
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_set_experiment_variant(string experiment, string variant, string tags, string happenedAt);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_fetch_remote_config();
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_activate_remote_config(string experimentsJson);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_fetch_and_activate_remote_config();
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_set_remote_config_settings(long minimumFetchIntervalInSeconds);
    [DllImport("__Internal")]
    private static extern IntPtr _justtrack_sdk_rp_get_all_assignments();
    [DllImport("__Internal")]
    private static extern IntPtr _justtrack_sdk_rp_get_remote_config_string(string configKey);
#pragma warning restore SA1309

    private static string ReadStringFromPointer(IntPtr ptr) {
        if (ptr == IntPtr.Zero) {
            return "";
        }

        // Custom implementation for Marshal.PtrToStringUTF8 which is not available
        // on the .NET version we are using:

        int length = 0;
        while (Marshal.ReadByte(ptr, length) != 0) {
            length++;
        }

        byte[] bytes = new byte[length];
        Marshal.Copy(ptr, bytes, 0, length);
        string str = Encoding.UTF8.GetString(bytes);

        // don't forget to free the memory (otherwise we wouldn't need to do this dance)
        _justtrack_sdk_rp_free_string(ptr);

        return str;
    }

    private static string? ReadNullableStringFromPointer(IntPtr ptr) {
        if (ptr == IntPtr.Zero) {
            return null;
        }

        return ReadStringFromPointer(ptr);
    }

    
    
    private readonly System.Collections.Generic.Dictionary<string, System.Tuple<Action?, Action<string>?>> publishEventCallbacks = new System.Collections.Generic.Dictionary<string, System.Tuple<Action?, Action<string>?>>();
    private readonly object publishEventCallbacksLock = new object();

    
    
    internal void Init(
        string apiToken,
        string? trackingId,
        string? trackingProvider,
        string? customUserId,
        bool automaticInAppPurchaseTracking,
        bool manualStart,
        bool enableConsoleLogging,
        string customBundleId,
        string customAppVersion,
        string customAppCode,
        string? customServerUrl,
        Action<string> onInitialized,
        Action<string> onSuccess,
        Action<string> onError)
    {
        onSdkInitialized += onInitialized;
        onAttributionDone += onSuccess;
        onAttributionError += onError;
        _justtrack_sdk_rp_init(
            apiToken,
            trackingId != null ? trackingId : "",
            trackingProvider != null ? trackingProvider : "",
            customUserId != null ? customUserId : "",
            48,
            14,
            5,
            120,
            (automaticInAppPurchaseTracking ? 1 : 0),
            (manualStart ? 1 : 0),
            (enableConsoleLogging ?  1 : 0),
            customBundleId ?? "",
            customAppVersion ?? "",
            customAppCode ?? "",
            customServerUrl
        );
    }

    internal void StartSDK()
    {
        _justtrack_sdk_rp_start();
    }

    internal void StopSDK()
    {
        _justtrack_sdk_rp_stop();
    }

    internal void Anonymize(Action pOnSuccess, Action<string> pOnFailure)
    {
        onAnonymizeDone += pOnSuccess;
        onAnonymizeError += pOnFailure;
        _justtrack_sdk_rp_anonymize();
    }

    internal bool IsRunning()
    {
        return _justtrack_sdk_rp_is_running();
    }

    internal void GetRetargetingParameters(Action<string> onSuccess, Action<string> onError)
    {
        onGetRetargetingParametersDone += onSuccess;
        onGetRetargetingParametersError += onError;
        _justtrack_sdk_rp_get_retargeting_parameters();
    }

    internal string GetPreliminaryRetargetingParameters()
    {
        return ReadStringFromPointer(_justtrack_sdk_rp_get_preliminary_retargeting_parameters());
    }

    internal void ForwardAdImpression(
        string adFormat,
        string adSdkName,
        string? adNetwork,
        string? placement,
        string? testGroup,
        string? segmentName,
        string? instanceName,
        string? bundleId,
        double revenue,
        string? currency,
        Action onSuccess,
        Action<string> onError
    )
    {
        onForwardAdImpressionDone += onSuccess;
        onForwardAdImpressionError += onError;    
        _justtrack_sdk_rp_forward_ad_impression(
            adFormat,
            adSdkName,
            adNetwork, 
            placement,
            testGroup,
            segmentName,
            instanceName,
            bundleId,
            revenue,
            currency
        );
    }

    internal void SetCustomUserId(string customUserId)
    {
        _justtrack_sdk_rp_set_user_id(customUserId != null ? customUserId : "");
    }

    internal void SetAutomaticInAppPurchaseTracking(bool automaticInAppPurchaseTracking)
    {
        _justtrack_sdk_rp_set_automatic_in_app_purchase_tracking(automaticInAppPurchaseTracking);
    }

    internal void PublishEvent(AppEvent pEvent, Action? pOnSuccess = null, Action<string>? pOnFailure = null)
    {
        string? requestId = null;
        
        if (pOnSuccess != null || pOnFailure != null)
        {
            requestId = System.Guid.NewGuid().ToString();

            lock (publishEventCallbacksLock)
            {
                publishEventCallbacks[requestId] = new System.Tuple<Action?, Action<string>?>(pOnSuccess, pOnFailure);
            }
        }
        
        // encode our dimensions as a JSON object to make it easier to pass them through the C-API.
        string dimensions = pEvent.EncodeDimensions();
        if (pEvent.Unit != null)
        {
            var unit = "";
            switch (pEvent.Unit) {
                case Unit.Count:
                    unit = "count";
                    break;
                case Unit.Milliseconds:
                    unit = "milliseconds";
                    break;
                case Unit.Seconds:
                    unit = "seconds";
                    break;
            }
            _justtrack_sdk_rp_publish_event(pEvent.Name, dimensions, pEvent.Value, unit, null, requestId);
        }
        else if (pEvent.Currency != null)
        {
            _justtrack_sdk_rp_publish_event(pEvent.Name, dimensions, pEvent.Value, null, pEvent.Currency, requestId);
        }
        else
        {
            _justtrack_sdk_rp_publish_event(pEvent.Name, dimensions, 0.0, null, null, requestId);
        }
    }

    internal void SetFirebaseAppInstanceId(string firebaseAppInstanceId)
    {
        _justtrack_sdk_rp_set_firebase_app_instance_id(firebaseAppInstanceId);
    }

    internal void IntegrateWithAppLovin(string? customUserId, Action pOnSuccess, Action<string> pOnFailure)
    {
        onIntegrateAppLovinDone += pOnSuccess;
        onIntegrateAppLovinError += pOnFailure;
        _justtrack_sdk_rp_integrate_with_app_lovin(customUserId);
    }

    internal void IntegrateWithFirebase(Action onSuccess, Action<string> onError)
    {
        onIntegrateFirebaseDone += onSuccess;
        onIntegrateFirebaseError += onError;
        _justtrack_sdk_rp_integrate_with_firebase();
    }

    internal void IntegrateWithIronSource(string? customUserId, Action pOnSuccess, Action<string> pOnFailure)
    {
        onIntegrateIronSourceDone += pOnSuccess;
        onIntegrateIronSourceError += pOnFailure;
        _justtrack_sdk_rp_integrate_with_iron_source(customUserId);
    }

    internal void IntegrateWithUnityAds(Action pOnSuccess, Action<string> pOnFailure)
    {
        onIntegrateUnityAdsDone += pOnSuccess;
        onIntegrateUnityAdsError += pOnFailure;
        _justtrack_sdk_rp_integrate_with_unity_ads();
    }

    internal void IntegrateWithGoogleOdm(Action pOnSuccess, Action<string> pOnFailure)
    {
        onIntegrateGoogleOdmDone += pOnSuccess;
        onIntegrateGoogleOdmError += pOnFailure;
        _justtrack_sdk_rp_integrate_with_google_odm();
    }

    internal void GetAdvertiserIdInfo(Action<string> onSuccess, Action<string> onError)
    {
        onGetAdvertiserIdInfoDone += onSuccess;
        onGetAdvertiserIdInfoError += onError;
        _justtrack_sdk_rp_get_advertiser_id_info();
    }

    internal int? GetTestGroupId()
    {
        int testGroupId = _justtrack_sdk_rp_get_test_group_id();
        if (testGroupId == -1) {
            return null;
        }

        return testGroupId;
    }

    internal void RequestTrackingAuthorization(Action<bool> onAuthorized)
    {
        onAuthorizationDone += onAuthorized;
        _justtrack_sdk_rp_request_tracking_authorization();
    }

    internal void ForwardTransactionId(string transactionId, string productId, int quantity)
    {
        _justtrack_sdk_rp_forward_transaction_id(transactionId, productId, quantity);
    }

    internal AttAuthorizationStatus GetTrackingAuthorizationStatus()
    {
        var status = _justtrack_sdk_rp_get_tracking_authorization_status();

        if (Enum.IsDefined(typeof(AttAuthorizationStatus), status))
        {
            return (AttAuthorizationStatus)status;
        }

        return AttAuthorizationStatus.NotDetermined;
    }

    internal void SetExperimentVariant(string experiment, string variant, string[]? tags, DateTime? happenedAt, Action onSuccess, Action<string> onError)
    {
        onSetExperimentVariantDone += onSuccess;
        onSetExperimentVariantError += onError;

        var tagsJson = tags != null ? JsonUtility.ToJson(new StringArrayWrapper { items = tags }) : "";
        var happenedAtString = happenedAt != null ? happenedAt.Value.ToString("yyyy-MM-ddTHH:mm:ssK") : "";

        _justtrack_sdk_rp_set_experiment_variant(experiment, variant, tagsJson, happenedAtString);
    }

    internal void FetchRemoteConfig(Action onSuccess, Action<string> onError)
    {
        onFetchRemoteConfigDone += onSuccess;
        onFetchRemoteConfigError += onError;
        _justtrack_sdk_rp_fetch_remote_config();
    }

    internal void ActivateRemoteConfig(string[] experiments, Action onSuccess, Action<string> onError)
    {
        onActivateRemoteConfigDone += onSuccess;
        onActivateRemoteConfigError += onError;
        var items = experiments ?? Array.Empty<string>();
        var experimentsJson = JsonUtility.ToJson(new StringArrayWrapper { items = items });
        _justtrack_sdk_rp_activate_remote_config(experimentsJson);
    }

    internal void FetchAndActivateRemoteConfig(Action onSuccess, Action<string> onError)
    {
        onFetchAndActivateRemoteConfigDone += onSuccess;
        onFetchAndActivateRemoteConfigError += onError;
        _justtrack_sdk_rp_fetch_and_activate_remote_config();
    }

    internal void SetRemoteConfigSettings(JusttrackRemoteConfigSettings settings)
    {
        _justtrack_sdk_rp_set_remote_config_settings(settings.minimumFetchIntervalInSeconds);
    }

    internal string? GetAllAssignments()
    {
        return ReadNullableStringFromPointer(_justtrack_sdk_rp_get_all_assignments());
    }

    internal bool? GetRemoteConfigBoolean(string configKey)
    {
        string? value = ReadNullableStringFromPointer(_justtrack_sdk_rp_get_remote_config_string(configKey));
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        if (bool.TryParse(value, out bool parsed))
        {
            return parsed;
        }

        return null;
    }

    internal int? GetRemoteConfigInt(string configKey)
    {
        string? value = ReadNullableStringFromPointer(_justtrack_sdk_rp_get_remote_config_string(configKey));
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        if (int.TryParse(value, out int parsed))
        {
            return parsed;
        }

        return null;
    }

    internal double? GetRemoteConfigDouble(string configKey)
    {
        string? value = ReadNullableStringFromPointer(_justtrack_sdk_rp_get_remote_config_string(configKey));
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed))
        {
            return parsed;
        }

        return null;
    }

    internal long? GetRemoteConfigLong(string configKey)
    {
        string? value = ReadNullableStringFromPointer(_justtrack_sdk_rp_get_remote_config_string(configKey));
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        if (long.TryParse(value, out long parsed))
        {
            return parsed;
        }

        return null;
    }

    internal string? GetRemoteConfigString(string configKey)
    {
        return ReadNullableStringFromPointer(_justtrack_sdk_rp_get_remote_config_string(configKey));
    }

    [System.Serializable]
    private class StringArrayWrapper
    {
        public string[] items = Array.Empty<string>();
    }

    
    
    private static JustTrackSDKNativeBridgeUnity? _instance = null;

    internal static JustTrackSDKNativeBridgeUnity Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("JustTrackSDKNativeBridgeUnity");
                _instance = obj.AddComponent<JustTrackSDKNativeBridgeUnity>();
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null)
        {
            // we are a duplicate and thus there is no need for this instance.
            // destroy this instance and keep the already existing instance.
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    
    
    private event Action<string>? onSdkInitialized = null;
    private event Action<string>? onAttributionDone = null;
    private event Action<string>? onAttributionError = null;
    private event Action<string>? onGetRetargetingParametersDone = null;
    private event Action<string>? onGetRetargetingParametersError = null;
    private event Action<string>? onGetAdvertiserIdInfoDone = null;
    private event Action<string>? onGetAdvertiserIdInfoError = null;
    private event Action<bool>? onAuthorizationDone = null;
    private event Action? onAnonymizeDone = null;
    private event Action<string>? onAnonymizeError = null;
    private event Action? onIntegrateAppLovinDone = null;
    private event Action<string>? onIntegrateAppLovinError = null;
    private event Action? onIntegrateFirebaseDone = null;
    private event Action<string>? onIntegrateFirebaseError = null;
    private event Action? onIntegrateIronSourceDone = null;
    private event Action<string>? onIntegrateIronSourceError = null;
    private event Action? onIntegrateUnityAdsDone = null;
    private event Action<string>? onIntegrateUnityAdsError = null;
    private event Action? onIntegrateGoogleOdmDone = null;
    private event Action<string>? onIntegrateGoogleOdmError = null;
    private event Action? onSetExperimentVariantDone = null;
    private event Action<string>? onSetExperimentVariantError = null;
    private event Action? onForwardAdImpressionDone = null;
    private event Action<string>? onForwardAdImpressionError = null;

    private event Action? onFetchRemoteConfigDone = null;
    private event Action<string>? onFetchRemoteConfigError = null;
    private event Action? onActivateRemoteConfigDone = null;
    private event Action<string>? onActivateRemoteConfigError = null;
    private event Action? onFetchAndActivateRemoteConfigDone = null;
    private event Action<string>? onFetchAndActivateRemoteConfigError = null;

    internal void OnSdkInitialized(string userId)
    {
        if (onSdkInitialized != null)
        {
            onSdkInitialized.Invoke(userId);
        }
        onSdkInitialized = null;
    }

    internal void OnAttributionDone(string response)
    {
        if (onAttributionDone != null)
        {
            onAttributionDone.Invoke(response);
        }
        onAttributionDone = null;
        onAttributionError = null;
    }

    internal void OnAttributionError(string error)
    {
        if (onAttributionError != null)
        {
            onAttributionError.Invoke(error);
        }
        onAttributionDone = null;
        onAttributionError = null;
    }

    internal void OnGetRetargetingParametersDone(string response)
    {
        if (onGetRetargetingParametersDone != null)
        {
            onGetRetargetingParametersDone.Invoke(response);
        }
        onGetRetargetingParametersDone = null;
        onGetRetargetingParametersError = null;
    }

    internal void OnGetRetargetingParametersError(string error)
    {
        if (onGetRetargetingParametersError != null)
        {
            onGetRetargetingParametersError.Invoke(error);
        }
        onGetRetargetingParametersDone = null;
        onGetRetargetingParametersError = null;
    }

    internal void OnAttributionListenerReceived(string response)
    {
        SDKiOSAgent.INSTANCE.OnAttributionListenerReceived(response);
    }

    internal void OnRetargetingParametersListenerReceived(string response)
    {
        SDKiOSAgent.INSTANCE.OnRetargetingParametersListenerReceived(response);
    }

    internal void OnPreliminaryRetargetingParametersListenerReceived(string response)
    {
        SDKiOSAgent.INSTANCE.OnPreliminaryRetargetingParametersListenerReceived(response);
    }

    internal void OnValidatePreliminaryRetargetingParametersDone(string response)
    {
        SDKiOSAgent.INSTANCE.OnValidatePreliminaryRetargetingParametersDone(response);
    }

    internal void OnValidatePreliminaryRetargetingParametersError(string response)
    {
        SDKiOSAgent.INSTANCE.OnValidatePreliminaryRetargetingParametersError(response);
    }

    internal void OnHandleError(string error)
    {

    }

    internal void OnGetAdvertiserIdInfo(string advertiserId)
    {
        if (onGetAdvertiserIdInfoDone != null) {
            onGetAdvertiserIdInfoDone.Invoke(advertiserId);
        }
        onGetAdvertiserIdInfoDone = null;
        onGetAdvertiserIdInfoError = null;
    }

    internal void OnGetAdvertiserIdInfoError(string error)
    {
        if (onGetAdvertiserIdInfoError != null) {
            onGetAdvertiserIdInfoError.Invoke(error);
        }
        onGetAdvertiserIdInfoDone = null;
        onGetAdvertiserIdInfoError = null;
    }

    internal void OnTrackingAuthorization(string reply)
    {
        var isAuthorized = reply == "authorized";

        Reflection.SetAdvertiserTrackingEnabled(isAuthorized);

        if (onAuthorizationDone != null)
        {
            onAuthorizationDone.Invoke(isAuthorized);
        }
        onAuthorizationDone = null;
    }

    internal void OnAnonymizeDone()
    {
        if (onAnonymizeDone != null)
        {
            onAnonymizeDone.Invoke();
        }
        onAnonymizeDone = null;
        onAnonymizeError = null;
    }

    internal void OnAnonymizeError(string error)
    {
        if (onAnonymizeError != null)
        {
            onAnonymizeError.Invoke(error);
        }
        onAnonymizeDone = null;
        onAnonymizeError = null;
    }

    internal void OnIntegrateAppLovinDone()
    {
        if (onIntegrateAppLovinDone != null)
        {
            onIntegrateAppLovinDone.Invoke();
        }
        onIntegrateAppLovinDone = null;
        onIntegrateAppLovinError = null;
    }

    internal void OnIntegrateAppLovinError(string error)
    {
        if (onIntegrateAppLovinError != null)
        {
            onIntegrateAppLovinError.Invoke(error);
        }
        onIntegrateAppLovinDone = null;
        onIntegrateAppLovinError = null;
    }

    internal void OnIntegrateFirebaseDone()
    {
        if (onIntegrateFirebaseDone != null)
        {
            onIntegrateFirebaseDone.Invoke();
        }
        onIntegrateFirebaseDone = null;
        onIntegrateFirebaseError = null;
    }

    internal void OnIntegrateFirebaseError(string error)
    {
        if (onIntegrateFirebaseError != null)
        {
            onIntegrateFirebaseError.Invoke(error);
        }
        onIntegrateFirebaseDone = null;
        onIntegrateFirebaseError = null;
    }

    internal void OnIntegrateIronSourceDone()
    {
        if (onIntegrateIronSourceDone != null)
        {
            onIntegrateIronSourceDone.Invoke();
        }
        onIntegrateIronSourceDone = null;
        onIntegrateIronSourceError = null;
    }

    internal void OnIntegrateIronSourceError(string error)
    {
        if (onIntegrateIronSourceError != null)
        {
            onIntegrateIronSourceError.Invoke(error);
        }
        onIntegrateIronSourceDone = null;
        onIntegrateIronSourceError = null;
    }

    internal void OnIntegrateUnityAdsDone()
    {
        if (onIntegrateUnityAdsDone != null)
        {
            onIntegrateUnityAdsDone.Invoke();
        }
        onIntegrateUnityAdsDone = null;
        onIntegrateUnityAdsError = null;
    }

    internal void OnIntegrateUnityAdsError(string error)
    {
        if (onIntegrateUnityAdsError != null)
        {
            onIntegrateUnityAdsError.Invoke(error);
        }
        onIntegrateUnityAdsDone = null;
        onIntegrateUnityAdsError = null;
    }

    internal void OnIntegrateGoogleOdmDone()
    {
        if (onIntegrateGoogleOdmDone != null)
        {
            onIntegrateGoogleOdmDone.Invoke();
        }
        onIntegrateGoogleOdmDone = null;
        onIntegrateGoogleOdmError = null;
    }

    internal void OnIntegrateGoogleOdmError(string error)
    {
        if (onIntegrateGoogleOdmError != null)
        {
            onIntegrateGoogleOdmError.Invoke(error);
        }
        onIntegrateGoogleOdmDone = null;
        onIntegrateGoogleOdmError = null;
    }

    internal void OnSetExperimentVariantDone()
    {
        if (onSetExperimentVariantDone != null)
        {
            onSetExperimentVariantDone.Invoke();
        }
        onSetExperimentVariantDone = null;
        onSetExperimentVariantError = null;
    }

    internal void OnSetExperimentVariantError(string error)
    {
        if (onSetExperimentVariantError != null)
        {
            onSetExperimentVariantError.Invoke(error);
        }
        onSetExperimentVariantDone = null;
        onSetExperimentVariantError = null;
    }

    internal void OnForwardAdImpressionDone()
    {
        if (onForwardAdImpressionDone != null)
        {
            onForwardAdImpressionDone.Invoke();
        }
        onForwardAdImpressionDone = null;
        onForwardAdImpressionError = null;
    }

    internal void OnForwardAdImpressionError(string error)
    {
        if (onForwardAdImpressionError != null)
        {
            onForwardAdImpressionError.Invoke(error);
        }
        onForwardAdImpressionDone = null;
        onForwardAdImpressionError = null;
    }

    internal void OnFetchRemoteConfigDone()
    {
        if (onFetchRemoteConfigDone != null)
        {
            onFetchRemoteConfigDone.Invoke();
        }
        onFetchRemoteConfigDone = null;
        onFetchRemoteConfigError = null;
    }

    internal void OnFetchRemoteConfigError(string error)
    {
        if (onFetchRemoteConfigError != null)
        {
            onFetchRemoteConfigError.Invoke(error);
        }
        onFetchRemoteConfigDone = null;
        onFetchRemoteConfigError = null;
    }

    internal void OnActivateRemoteConfigDone()
    {
        if (onActivateRemoteConfigDone != null)
        {
            onActivateRemoteConfigDone.Invoke();
        }
        onActivateRemoteConfigDone = null;
        onActivateRemoteConfigError = null;
    }

    internal void OnActivateRemoteConfigError(string error)
    {
        if (onActivateRemoteConfigError != null)
        {
            onActivateRemoteConfigError.Invoke(error);
        }
        onActivateRemoteConfigDone = null;
        onActivateRemoteConfigError = null;
    }

    internal void OnFetchAndActivateRemoteConfigDone()
    {
        if (onFetchAndActivateRemoteConfigDone != null)
        {
            onFetchAndActivateRemoteConfigDone.Invoke();
        }
        onFetchAndActivateRemoteConfigDone = null;
        onFetchAndActivateRemoteConfigError = null;
    }

    internal void OnFetchAndActivateRemoteConfigError(string error)
    {
        if (onFetchAndActivateRemoteConfigError != null)
        {
            onFetchAndActivateRemoteConfigError.Invoke(error);
        }
        onFetchAndActivateRemoteConfigDone = null;
        onFetchAndActivateRemoteConfigError = null;
    }

    internal void OnPublishEventDone(string requestId)
    {
        System.Tuple<Action?, Action<string>?>? callbacks = null;

        lock (publishEventCallbacksLock)
        {
            if (publishEventCallbacks.TryGetValue(requestId, out callbacks))
            {
                publishEventCallbacks.Remove(requestId);
            }
        }

        if (callbacks != null && callbacks.Item1 != null)
        {
            JustTrackSDKBehaviour.CallOnMainThread(() => {
                callbacks.Item1.Invoke();
            });
        }
    }

    internal void OnPublishEventError(string requestIdAndError)
    {
        string requestId = "";
        string errorMessage = "Unknown error";
        
        int separatorIndex = requestIdAndError.IndexOf('|');
        if (separatorIndex > 0)
        {
            requestId = requestIdAndError.Substring(0, separatorIndex);
            errorMessage = requestIdAndError.Substring(separatorIndex + 1);
        }
        else
        {
            requestId = requestIdAndError;
        }
        
        System.Tuple<Action?, Action<string>?>? callbacks = null;

        lock (publishEventCallbacksLock)
        {
            if (publishEventCallbacks.TryGetValue(requestId, out callbacks))
            {
                publishEventCallbacks.Remove(requestId);
            }
        }

        if (callbacks != null && callbacks.Item2 != null)
        {
            JustTrackSDKBehaviour.CallOnMainThread(() => {
                callbacks.Item2.Invoke(errorMessage);
            });
        }
    }

    }
#endif
