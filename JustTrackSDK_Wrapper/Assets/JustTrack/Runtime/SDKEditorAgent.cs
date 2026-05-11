#if UNITY_EDITOR
using System;
using UnityEngine;

namespace JustTrack
{
    internal class SDKEditorAgent : ISDKAgent
    {
        private bool EditorInitialized = false;

        internal SDKEditorAgent() {}

        public void Initialize(
            string pApiKey,
            string? pTrackingId,
            string? pTrackingProvider,
            string? pCustomUserId,
            bool pAutomaticInAppPurchaseTracking,
            bool pEnableDebugMode,
            bool pManualStart,
            bool pEnableConsoleLogging,
            Action<AttributionResponse> pOnSuccess,
            Action<string> pOnFailure)
        {
            AttributionResponse fakeResponse = AttributionResponse.CreateFakeResponse();

            EditorInitialized = true;

            JustTrackSDKBehaviour.CallOnMainThread(() => {
                pOnSuccess(fakeResponse);
            });
        }

        public void Start()
        {
            LogDebug("Performed SDK start");
        }

        public void Stop()
        {
            LogDebug("Performed SDK stop");
        }

        public void Anonymize(Action pOnSuccess, Action<string> pOnFailure)
        {
            LogDebug("Performed SDK anonymize");
            JustTrackSDKBehaviour.CallOnMainThread(() => {
                pOnSuccess();
            });
        }

        public bool IsRunning()
        {
            return false;
        }

        public void RegisterAttributionListener(Action<AttributionResponse> pListener)
        {
            LogDebug("Registered attribution listener");
        }

#if !UNITY_WEBGL
        public void GetRetargetingParameters(Action<RetargetingParameters?> pOnSuccess, Action<string> pOnFailure)
        {
            JustTrackSDKBehaviour.CallOnMainThread(() => {
                pOnSuccess(null);
            });
        }

        public PreliminaryRetargetingParameters? GetPreliminaryRetargetingParameters()
        {
            return null;
        }

        public void RegisterRetargetingParameterListener(Action<RetargetingParameters> pListener)
        {
            LogDebug("Registered retargeting parameter listener");
        }

        public void RegisterPreliminaryRetargetingListener(Action<PreliminaryRetargetingParameters> pListener)
        {
            LogDebug("Registered preliminary retargeting parameter listener");
        }

        public void IntegrateWithAppLovin(string? customUserId, Action pOnSuccess, Action<string> pOnFailure)
        {
            LogDebug("Performed AppLovin integration");
            JustTrackSDKBehaviour.CallOnMainThread(pOnSuccess);
        }

        public void IntegrateWithFirebase(Action pOnSuccess, Action<string> pOnFailure)
        {
            LogDebug("Performed Firebase integration");
            JustTrackSDKBehaviour.CallOnMainThread(pOnSuccess);
        }

        public void IntegrateWithIronSource(string? customUserId, Action pOnSuccess, Action<string> pOnFailure)
        {
            LogDebug("Performed IronSource integration");
            JustTrackSDKBehaviour.CallOnMainThread(pOnSuccess);
        }

        public void IntegrateWithUnityAds(Action pOnSuccess, Action<string> pOnFailure)
        {
            LogDebug("Performed UnityAds integration");
            JustTrackSDKBehaviour.CallOnMainThread(pOnSuccess);
        }

        public void IntegrateWithGoogleOdm(Action pOnSuccess, Action<string> pOnFailure)
        {
            LogDebug("Performed Google ODM integration");
            JustTrackSDKBehaviour.CallOnMainThread(pOnSuccess);
        }

        public void ForwardAdImpression(AdImpression pAdImpression, Action pOnSuccess, Action<string> pOnFailure)
        {
            var revenue = pAdImpression.Revenue;
            if (revenue == null)
            {
                revenue = new Money(0.0, "USD");
            }
            LogDebug($"Forwarded ad impression for ad unit {pAdImpression.Unit}, sdk name {pAdImpression.SdkName}, network {pAdImpression.Network}, placement {pAdImpression.Placement}, test group {pAdImpression.TestGroup}, segment {pAdImpression.SegmentName}, instance {pAdImpression.InstanceName}, and bundle id {pAdImpression.BundleId} with {revenue.Value} {revenue.Currency} revenue");
        }
        
        public void SetAutomaticInAppPurchaseTracking(bool pEnabled)
        {
            LogDebug($"Setting automatic in-app purchase tracking to {pEnabled}");
        }
        
        public void SetFirebaseAppInstanceId(string pFirebaseAppInstanceId)
        {
            LogDebug($"Forwarding Firebase app instance id {pFirebaseAppInstanceId}");
        }
        
        public void SetExperimentVariant(string experiment, string variant, string[]? tags, DateTime? happenedAt, Action pOnSuccess, Action<string> pOnFailure)
        {
            var tagsString = tags != null ? string.Join(", ", tags) : "none";
            var happenedAtString = happenedAt != null ? happenedAt.Value.ToString("yyyy-MM-ddTHH:mm:ssK") : "none";
            LogDebug($"Setting experiment variant - experiment: {experiment}, variant: {variant}, tags: {tagsString}, happenedAt: {happenedAtString}");
            
            JustTrackSDKBehaviour.CallOnMainThread(() =>
            {
                pOnSuccess();
            });
        }

        public void FetchRemoteConfig(Action pOnSuccess, Action<string> pOnFailure)
        {
            LogDebug($"PerformFetchRemoteConfig called");
                        JustTrackSDKBehaviour.CallOnMainThread(() =>
            {
                pOnSuccess();
            });
        }

        public void ActivateRemoteConfig(string[] experiments, Action pOnSuccess, Action<string> pOnFailure)
        {
            LogDebug($"PerformActivateRemoteConfig called");
                        JustTrackSDKBehaviour.CallOnMainThread(() =>
            {
                pOnSuccess();
            });
        }

        public void FetchAndActivateRemoteConfig(Action pOnSuccess, Action<string> pOnFailure)
        {
            LogDebug($"PerformFetchAndActivateRemoteConfig called");
                        JustTrackSDKBehaviour.CallOnMainThread(() =>
            {
                pOnSuccess();
            });
        }

        public void SetRemoteConfigSettings(JusttrackRemoteConfigSettings settings)
        {
            LogDebug($"PerformSetRemoteConfigSettings called");
        }

        public Assignment[] GetAllAssignments()
        {
            LogDebug($"PerformGetAllAssignments called");
            return new Assignment[0];
        }

        public bool? GetRemoteConfigBoolean(string configKey)
        {
            LogDebug($"Getting remote config boolean {configKey}");
            return null;
        }

        public double? GetRemoteConfigDouble(string configKey)
        {
            LogDebug($"Getting remote config double {configKey}");
            return null;
        }

        public int? GetRemoteConfigInt(string configKey)
        {
            LogDebug($"Getting remote config int {configKey}");
            return null;
        }

        public long? GetRemoteConfigLong(string configKey)
        {
            LogDebug($"Getting remote config long {configKey}");
            return null;
        }

        public string? GetRemoteConfigString(string configKey)
        {
            LogDebug($"Getting remote config string {configKey}");
            return null;
        }
#endif

#if UNITY_IOS
        public void ForwardTransactionId(string transactionId, string productId, int quantity) {
            LogDebug($"Forwarding transaction id {transactionId} product id {productId} quantity {quantity}");
        }

        public AttAuthorizationStatus GetTrackingAuthorizationStatus()
        {
            return AttAuthorizationStatus.NotAvailable;
        }
#endif

#if UNITY_ANDROID
        public void ForwardTransaction(string token, string productId, Money money, ProductType ProductType) {
            LogDebug($"Forwarding transaction token {token} product id {productId}");
        }
#endif

        public void PublishEvent(AppEvent pEvent, Action? pOnSuccess = null, Action<string>? pOnFailure = null)
        {
            LogDebug($"Publishing event {pEvent.Name} {pEvent.Value} {pEvent.Unit}");
            pOnSuccess?.Invoke();
        }

#if !UNITY_WEBGL
        public void GetInstallInstanceId(Action<string> pOnSuccess, Action<string> pOnFailure)
        {
            JustTrackSDKBehaviour.CallOnMainThread(() =>
            {
                pOnSuccess("00000000-0000-0000-0000-000000000000");
            });
        }
        
        public void SetCustomUserId(string pCustomUserId)
        {
            LogDebug($"Setting custom user id {pCustomUserId}");
        }

        public void GetAdvertiserIdInfo(Action<AdvertiserIdInfo> pOnSuccess, Action<string> pOnFailure)
        {
            JustTrackSDKBehaviour.CallOnMainThread(() =>
            {
                pOnSuccess(new AdvertiserIdInfo("00000000-0000-0000-0000-000000000000", false));
            });
        }

        public void GetTestGroupId(Action<int?> pOnSuccess, Action<string> pOnFailure)
        {
            JustTrackSDKBehaviour.CallOnMainThread(() =>
            {
                pOnSuccess(1);
            });
        }
#endif

        public bool IsInitialized()
        {
            return EditorInitialized;
        }

        private void LogDebug(string pMessage)
        {
            Debug.Log(pMessage);
        }

        private void LogInfo(string pMessage)
        {
            Debug.Log(pMessage);
        }

        private void LogWarning(string pMessage)
        {
            Debug.LogWarning(pMessage);
        }

        private void LogError(string pMessage)
        {
            Debug.LogError(pMessage);
        }
    }
}
#endif
