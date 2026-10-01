#pragma warning disable SA1600

using System;

namespace JustTrack
{
        internal interface ISDKAgent
        {
                bool IsInitialized();

                void PublishEvent(AppEvent pEvent, Action? pOnSuccess = null, Action<string>? pOnFailure = null);

                void Initialize(string pApiKey, string? pTrackingId, string? pTrackingProvider, string? pCustomUserId, bool pAutomaticInAppPurchaseTracking, bool pManualStart, bool pEnableConsoleLogging, bool pEnableConnectionTracking, string? pServerUrl, string? pBundleId, string? pAppVersion, string? pAppCode);

                void Start();

                void Stop();

                void Anonymize(Action pOnSuccess, Action<string> pOnFailure);

                bool IsRunning();

                void RegisterAttributionListener(Action<AttributionResponse> pListener);

                void GetAttribution(Action<AttributionResponse> pOnSuccess, Action<string> pOnFailure);

#if UNITY_IOS
                void ForwardTransactionId(string transactionId, string productId, int quantity);
        
                AttAuthorizationStatus GetTrackingAuthorizationStatus();
#endif

#if UNITY_ANDROID
                void ForwardTransaction(string token, string productId, Money money, ProductType productType);
#endif

#if !UNITY_WEBGL
                void RegisterRetargetingParameterListener(Action<RetargetingParameters> pListener);

                void RegisterPreliminaryRetargetingListener(Action<PreliminaryRetargetingParameters> pListener);

                void IntegrateWithAppLovin(string? customUserId, Action pOnSuccess, Action<string> pOnFailure);

                void IntegrateWithFirebase(Action pOnSuccess, Action<string> pOnFailure);

                void IntegrateWithIronSource(string? customUserId, Action pOnSuccess, Action<string> pOnFailure);

                void IntegrateWithUnityAds(Action pOnSuccess, Action<string> pOnFailure);

                void IntegrateWithGoogleOdm(Action pOnSuccess, Action<string> pOnFailure);

                void ForwardAdImpression(AdImpression pAdImpression, Action pOnSuccess, Action<string> pOnFailure);

                void SetCustomUserId(string pCustomUserId);

                void SetAutomaticInAppPurchaseTracking(bool pEnabled);

                void GetRetargetingParameters(Action<RetargetingParameters?> pOnSuccess, Action<string> pOnFailure);

                PreliminaryRetargetingParameters? GetPreliminaryRetargetingParameters();

                void SetFirebaseAppInstanceId(string pFirebaseAppInstanceId);

                void SetGlobalDimension0(string? value);

                void SetGlobalDimension1(string? value);

                void SetGlobalDimension2(string? value);

                void GetInstallInstanceId(Action<string> pOnSuccess, Action<string> pOnFailure);

                void GetAdvertiserIdInfo(Action<AdvertiserIdInfo> pOnSuccess, Action<string> pOnFailure);

                void SetExperimentVariant(string experiment, string variant, string[]? tags, DateTime? happenedAt, Action pOnSuccess, Action<string> pOnFailure);

                void FetchRemoteConfig(Action pOnSuccess, Action<string> pOnFailure);

                void ActivateRemoteConfig(string[] experiments, Action pOnSuccess, Action<string> pOnFailure);

                void FetchAndActivateRemoteConfig(Action pOnSuccess, Action<string> pOnFailure);

                void SetRemoteConfigSettings(JusttrackRemoteConfigSettings settings);

                Assignment[] GetAllAssignments();

                bool? GetRemoteConfigBoolean(string configKey);

                double? GetRemoteConfigDouble(string configKey);

                int? GetRemoteConfigInt(string configKey);

                long? GetRemoteConfigLong(string configKey);

                string? GetRemoteConfigString(string configKey);
#endif
        }
}
#pragma warning restore SA1600