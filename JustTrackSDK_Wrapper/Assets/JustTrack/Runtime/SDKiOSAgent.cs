#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace JustTrack
{
    internal class SDKiOSAgent : ISDKAgent
    {
        internal static SDKiOSAgent INSTANCE = new SDKiOSAgent();
        private string installInstanceId = "";
        private bool initialized = false;

        private SDKiOSAgent() {}

#pragma warning disable SA1401, SA1307 // Fields should be private; field names should begin with upper-case letter (required for JSON serialization)
        [Serializable]
        public class iOSAttributionResponse
        {
            public string userType = "";
            public string campaignId = "";
            public string campaignName = "";
            public string campaignType = "";
            public string channelId = "";
            public string channelName = "";
            public string channelIncent = "";
            public string partnerId = "";
            public string partnerName = "";
            public string? sourceId = null;
            public string? sourceBundleId = null;
            public string? sourcePlacement = null;
            public string? adsetId = null;
            public string createdAt = "";
        }

        [Serializable]
        public class iOSRetargetingParameters
        {
            public string wasAlreadyInstalled = "";
            public string? url = null;
            public string parameters = "";
            public string? promoCode = null;
        }

        [Serializable]
        public class iOSRetargetingParameter
        {
            public string parameter = "";
            public string value = "";
        }

        [Serializable]
        public class iOSRetargetingParametersList
        {
            public iOSRetargetingParameter[] parameters = new iOSRetargetingParameter[]{};
        }

        [Serializable]
        public class iOSPreliminaryRetargetingParameters
        {
            public string preliminaryId = "";
            public string wasAlreadyInstalled = "";
            public string? url = null;
            public string parameters = "";
            public string? promoCode = null;
        }

        [Serializable]
        public class iOSPreliminaryRetargetingParametersValidateError
        {
            public string preliminaryId = "";
            public string error = "";
        }

        [Serializable]
        public class iOSPreliminaryRetargetingParametersValidateResult
        {
            public string preliminaryId = "";
            public string response = "";
            public string? parameters = null;
        }

        [Serializable]
        public class iOSUserData
        {
            public string installInstanceId = "";
        }
#pragma warning restore SA1401, SA1307

        public void Initialize(
            string pApiKey,
            string? pTrackingId,
            string? pTrackingProvider,
            string? pCustomUserId,
            bool pAutomaticInAppPurchaseTracking,
            bool pManualStart,
            bool pEnableConsoleLogging,
            bool pEnableConnectionTracking,
            string? pServerUrl,
            string? pBundleId,
            string? pAppVersion,
            string? pAppCode)
        {
            string customBundleId = pBundleId ?? "";
            string customAppVersion = pAppVersion ?? "";
            string customAppCode = pAppCode ?? "";

            string? serverUrl = pServerUrl;

            JustTrackSDKNativeBridgeUnity.Instance.Init(
                pApiKey,
                pTrackingId,
                pTrackingProvider,
                pCustomUserId,
                pAutomaticInAppPurchaseTracking,
                pManualStart,
                pEnableConsoleLogging,
                pEnableConnectionTracking,
                customBundleId,
                customAppVersion,
                customAppCode,
                serverUrl,
                (onInitializedJson) =>
                {
                    iOSUserData userData = JsonUtility.FromJson<iOSUserData>(onInitializedJson);
                    installInstanceId = userData.installInstanceId;
                });

            initialized = true;
        }

        private AttributionResponse ParseAttributionResponse(string attribution)
        {
                iOSAttributionResponse parsed = JsonUtility.FromJson<iOSAttributionResponse>(attribution);

                var userType = parsed.userType;
                var campaignId = parsed.campaignId;
                var campaignName = parsed.campaignName;
                var campaignType = parsed.campaignType;
                var channelIdString = parsed.channelId;
                var channelName = parsed.channelName;
                var channelIncentString = parsed.channelIncent;
                var partnerIdString = parsed.partnerId;
                var partnerName = parsed.partnerName;
                var sourceId = parsed.sourceId;
                var sourceBundleId = parsed.sourceBundleId;
                var sourcePlacement = parsed.sourcePlacement;
                var adsetId = parsed.adsetId;
                var createdAtString = parsed.createdAt;

                int channelId = int.Parse(channelIdString);
                bool channelIncent = channelIncentString == "true";
                int partnerId = int.Parse(partnerIdString);
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime createdAt = DateTime.ParseExact(createdAtString, "yyyy-MM-dd'T'HH:mm:ssK", provider);
                return AttributionResponse.CreateResponse(userType, campaignId, campaignName, campaignType, channelId, channelName, channelIncent, partnerId, partnerName, sourceId, sourceBundleId, sourcePlacement, adsetId, createdAt);
        }

        public void GetAttribution(Action<AttributionResponse> pOnSuccess, Action<string> pOnFailure)
        {
            Action<string> onSuccess = (attribution) =>
            {
                var response = ParseAttributionResponse(attribution);
                JustTrackSDKBehaviour.CallOnMainThread(() => pOnSuccess(response));
            };
            JustTrackSDKNativeBridgeUnity.Instance.GetAttribution(onSuccess, pOnFailure);
        }

        public void Start()
        {
            JustTrackSDKNativeBridgeUnity.Instance.StartSDK();
        }

        public void Stop()
        {
            JustTrackSDKNativeBridgeUnity.Instance.StopSDK();
        }

        public void Anonymize(Action pOnSuccess, Action<string> pOnFailure)
        {
            JustTrackSDKNativeBridgeUnity.Instance.Anonymize(pOnSuccess, pOnFailure);
        }

        public bool IsRunning()
        {
            return JustTrackSDKNativeBridgeUnity.Instance.IsRunning();
        }

        public void GetRetargetingParameters(Action<RetargetingParameters?> pOnSuccess, Action<string> pOnFailure)
        {
            Action<string> onSuccess = (parameters) =>
            {
                if (String.IsNullOrEmpty(parameters)) {
                    JustTrackSDKBehaviour.CallOnMainThread(() =>
                    {
                        pOnSuccess(null);
                    });
                    return;
                }

                var response = parseRetargetingParameters(parameters);
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    pOnSuccess(response);
                });
            };
            JustTrackSDKNativeBridgeUnity.Instance.GetRetargetingParameters(onSuccess, pOnFailure);
        }

        private RetargetingParameters parseRetargetingParameters(string parameters)
        {
            iOSRetargetingParameters parsed = JsonUtility.FromJson<iOSRetargetingParameters>(parameters);

            var wasAlreadyInstalled = parsed.wasAlreadyInstalled;
            var url = parsed.url;
            var promoCode = parsed.promoCode;

            bool wasAlreadyInstalledBool = wasAlreadyInstalled == "true";
            iOSRetargetingParametersList parsedParameters = JsonUtility.FromJson<iOSRetargetingParametersList>(parsed.parameters);
            var parametersDict = new Dictionary<string, string>();
            foreach (var item in parsedParameters.parameters)
            {
                parametersDict.Add(item.parameter, item.value);
            }

            return RetargetingParameters.CreateRetargetingParameters(wasAlreadyInstalledBool, url, parametersDict, promoCode);
        }

        private Dictionary<String, PreliminaryRetargetingParameters> pendingPreliminaryParameters = new Dictionary<String, PreliminaryRetargetingParameters>();

        public PreliminaryRetargetingParameters? GetPreliminaryRetargetingParameters()
        {
            string json = JustTrackSDKNativeBridgeUnity.Instance.GetPreliminaryRetargetingParameters();
            if (String.IsNullOrEmpty(json))
            {
                return null;
            }

            string preliminaryId;
            PreliminaryRetargetingParameters? parameters = parsePreliminaryRetargetingParameters(json, out preliminaryId);

            if (parameters != null)
            {
                lock(this)
                {
                    pendingPreliminaryParameters.Add(preliminaryId, parameters);
                }
            }

            return parameters;
        }

        private PreliminaryRetargetingParameters? parsePreliminaryRetargetingParameters(string parameters, out string preliminaryId)
        {
            iOSPreliminaryRetargetingParameters parsed = JsonUtility.FromJson<iOSPreliminaryRetargetingParameters>(parameters);

            preliminaryId = parsed.preliminaryId;
            var wasAlreadyInstalled = parsed.wasAlreadyInstalled;
            var url = parsed.url;
            var promoCode = parsed.promoCode;

            bool wasAlreadyInstalledBool = wasAlreadyInstalled == "true";
            iOSRetargetingParametersList parsedParameters = JsonUtility.FromJson<iOSRetargetingParametersList>(parsed.parameters);
            var parametersDict = new Dictionary<string, string>();
            foreach (var item in parsedParameters.parameters)
            {
                parametersDict.Add(item.parameter, item.value);
            }

            return PreliminaryRetargetingParameters.CreatePreliminaryRetargetingParameters(wasAlreadyInstalledBool, url, parametersDict, promoCode);
        }

        private Action<AttributionResponse>? OnAttributionCallback = null;
        private Action<RetargetingParameters>? OnRetargetingCallback = null;
        private Action<PreliminaryRetargetingParameters>? OnPreliminaryRetargetingCallback = null;

        public void RegisterAttributionListener(Action<AttributionResponse> pListener)
        {
            lock(this)
            {
                if (OnAttributionCallback == null)
                {
                    OnAttributionCallback = pListener;
                }
                else
                {
                    OnAttributionCallback += pListener;
                }
            }
        }

        public void RegisterRetargetingParameterListener(Action<RetargetingParameters> pListener)
        {
            lock(this)
            {
                if (OnRetargetingCallback == null)
                {
                    OnRetargetingCallback = pListener;
                }
                else
                {
                    OnRetargetingCallback += pListener;
                }
            }
        }

        public void RegisterPreliminaryRetargetingListener(Action<PreliminaryRetargetingParameters> pListener)
        {
            lock(this)
            {
                if (OnPreliminaryRetargetingCallback == null)
                {
                    OnPreliminaryRetargetingCallback = pListener;
                }
                else
                {
                    OnPreliminaryRetargetingCallback += pListener;
                }
            }
        }

        internal void OnAttributionListenerReceived(string response)
        {
            var attribution = ParseAttributionResponse(response);

            Action<AttributionResponse>? localCallback = null;
            lock(this)
            {
                localCallback = OnAttributionCallback;
            }

            if (localCallback != null)
            {
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    localCallback(attribution);
                });
            }
        }

        internal void OnRetargetingParametersListenerReceived(string response)
        {
            var parameters = parseRetargetingParameters(response);

            Action<RetargetingParameters>? localCallback = null;
            lock(this)
            {
                localCallback = OnRetargetingCallback;
            }

            if (localCallback != null)
            {
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    localCallback(parameters);
                });
            }
        }

        internal void OnPreliminaryRetargetingParametersListenerReceived(string response)
        {
            string preliminaryId;
            PreliminaryRetargetingParameters? parameters = parsePreliminaryRetargetingParameters(response, out preliminaryId);
            if (parameters != null)
            {
                Action<PreliminaryRetargetingParameters>? localCallback = null;
                lock(this)
                {
                    pendingPreliminaryParameters.Add(preliminaryId, parameters);
                    localCallback = OnPreliminaryRetargetingCallback;
                }

                if (localCallback != null)
                {
                    JustTrackSDKBehaviour.CallOnMainThread(() =>
                    {
                        localCallback(parameters);
                    });
                }
            }
        }

        internal void OnValidatePreliminaryRetargetingParametersDone(string response)
        {
            iOSPreliminaryRetargetingParametersValidateResult parsed = JsonUtility.FromJson<iOSPreliminaryRetargetingParametersValidateResult>(response);

            var preliminaryId = parsed.preliminaryId;
            var attribution = ParseAttributionResponse(parsed.response);
            var parameters = parsed.parameters == null ? null : parseRetargetingParameters(parsed.parameters);
            var validateResult = ValidateResult.CreateValidateResult(parameters, attribution);

            lock(this)
            {
                if (pendingPreliminaryParameters.ContainsKey(preliminaryId))
                {
                    pendingPreliminaryParameters[preliminaryId].Resolve(validateResult);
                    pendingPreliminaryParameters.Remove(preliminaryId);
                }
            }
        }

        internal void OnValidatePreliminaryRetargetingParametersError(string response)
        {
            iOSPreliminaryRetargetingParametersValidateError parsed = JsonUtility.FromJson<iOSPreliminaryRetargetingParametersValidateError>(response);

            var preliminaryId = parsed.preliminaryId;
            var error = parsed.error;

            lock(this)
            {
                if (pendingPreliminaryParameters.ContainsKey(preliminaryId))
                {
                    pendingPreliminaryParameters[preliminaryId].Reject(error);
                    pendingPreliminaryParameters.Remove(preliminaryId);
                }
            }
        }

        public void IntegrateWithAppLovin(string? customUserId, Action pOnSuccess, Action<string> pOnFailure)
        {
            JustTrackSDKNativeBridgeUnity.Instance.IntegrateWithAppLovin(customUserId, pOnSuccess, pOnFailure);
        }

        public void IntegrateWithFirebase(Action pOnSuccess, Action<string> pOnFailure)
        {
            JustTrackSDKNativeBridgeUnity.Instance.IntegrateWithFirebase(pOnSuccess, pOnFailure);
        }

        public void IntegrateWithIronSource(string? customUserId, Action pOnSuccess, Action<string> pOnFailure)
        {
            JustTrackSDKNativeBridgeUnity.Instance.IntegrateWithIronSource(customUserId, pOnSuccess, pOnFailure);
        }

        public void IntegrateWithUnityAds(Action pOnSuccess, Action<string> pOnFailure)
        {
            JustTrackSDKNativeBridgeUnity.Instance.IntegrateWithUnityAds(pOnSuccess, pOnFailure);
        }

        public void IntegrateWithGoogleOdm(Action pOnSuccess, Action<string> pOnFailure)
        {
            JustTrackSDKNativeBridgeUnity.Instance.IntegrateWithGoogleOdm(pOnSuccess, pOnFailure);
        }

        public void ForwardAdImpression(AdImpression pAdImpression, Action pOnSuccess, Action<string> pOnFailure)
        {
            var value = pAdImpression.Revenue != null ? pAdImpression.Revenue.Value : 0.0;
            var currency = pAdImpression.Revenue != null ? pAdImpression.Revenue.Currency : "USD";

            JustTrackSDKNativeBridgeUnity.Instance.ForwardAdImpression(
                pAdImpression.Unit,
                pAdImpression.SdkName,
                pAdImpression.Network,
                pAdImpression.Placement,
                pAdImpression.TestGroup,
                pAdImpression.SegmentName,
                pAdImpression.InstanceName,
                pAdImpression.BundleId,
                value,
                currency,
                pOnSuccess,
                pOnFailure
            );
        }

        public void SetCustomUserId(string pCustomUserId)
        {
            JustTrackSDKNativeBridgeUnity.Instance.SetCustomUserId(pCustomUserId);
        }

        public void SetAutomaticInAppPurchaseTracking(bool pEnabled)
        {
            JustTrackSDKNativeBridgeUnity.Instance.SetAutomaticInAppPurchaseTracking(pEnabled);
        }

        public void SetFirebaseAppInstanceId(string pFirebaseAppInstanceId)
        {
            JustTrackSDKNativeBridgeUnity.Instance.SetFirebaseAppInstanceId(pFirebaseAppInstanceId);
        }

        public void SetGlobalDimension0(string? value)
        {
            JustTrackSDKNativeBridgeUnity.Instance.SetGlobalDimension0(value);
        }

        public void SetGlobalDimension1(string? value)
        {
            JustTrackSDKNativeBridgeUnity.Instance.SetGlobalDimension1(value);
        }

        public void SetGlobalDimension2(string? value)
        {
            JustTrackSDKNativeBridgeUnity.Instance.SetGlobalDimension2(value);
        }

        public void PublishEvent(AppEvent pEvent, Action? pOnSuccess, Action<string>? pOnFailure)
        {
            JustTrackSDKNativeBridgeUnity.Instance.PublishEvent(pEvent, pOnSuccess, pOnFailure);
        }

        public void GetInstallInstanceId(Action<string> pOnSuccess, Action<string> pOnFailure)
        {
            JustTrackSDKBehaviour.CallOnMainThread(() =>
            {
                pOnSuccess(installInstanceId);
            });
        }

        public void GetAdvertiserIdInfo(Action<AdvertiserIdInfo> pOnSuccess, Action<string> pOnFailure)
        {
            JustTrackSDKNativeBridgeUnity.Instance.GetAdvertiserIdInfo((info) =>
                {
                    JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    pOnSuccess(new AdvertiserIdInfo(String.IsNullOrEmpty(info) ? null : info, String.IsNullOrEmpty(info)));
                });
            }, (error) =>
                {
                    JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    pOnFailure(error);
                });
            });
        }

        public void ForwardTransactionId(string transactionId, string productId, int quantity)
        {
            JustTrackSDKNativeBridgeUnity.Instance.ForwardTransactionId(transactionId, productId, quantity);
        }

        public AttAuthorizationStatus GetTrackingAuthorizationStatus()
        {
            return JustTrackSDKNativeBridgeUnity.Instance.GetTrackingAuthorizationStatus();
        }

        public bool IsInitialized()
        {
            return initialized;
        }

        public void SetExperimentVariant(string experiment, string variant, string[]? tags, DateTime? happenedAt, Action pOnSuccess, Action<string> pOnFailure)
        {
            JustTrackSDKNativeBridgeUnity.Instance.SetExperimentVariant(experiment, variant, tags, happenedAt, pOnSuccess, pOnFailure);
        }

        public void FetchRemoteConfig(Action pOnSuccess, Action<string> pOnFailure)
        {
            JustTrackSDKNativeBridgeUnity.Instance.FetchRemoteConfig(pOnSuccess, pOnFailure);
        }

        public void ActivateRemoteConfig(string[] experiments, Action pOnSuccess, Action<string> pOnFailure)
        {
            JustTrackSDKNativeBridgeUnity.Instance.ActivateRemoteConfig(experiments, pOnSuccess, pOnFailure);
        }

        public void FetchAndActivateRemoteConfig(Action pOnSuccess, Action<string> pOnFailure)
        {
            JustTrackSDKNativeBridgeUnity.Instance.FetchAndActivateRemoteConfig(pOnSuccess, pOnFailure);
        }

        public void SetRemoteConfigSettings(JusttrackRemoteConfigSettings settings)
        {
            JustTrackSDKNativeBridgeUnity.Instance.SetRemoteConfigSettings(settings);
        }

        public bool? GetRemoteConfigBoolean(string configKey)
        {
            return JustTrackSDKNativeBridgeUnity.Instance.GetRemoteConfigBoolean(configKey);
        }

        public int? GetRemoteConfigInt(string configKey)
        {
            return JustTrackSDKNativeBridgeUnity.Instance.GetRemoteConfigInt(configKey);
        }

        public double? GetRemoteConfigDouble(string configKey)
        {
            return JustTrackSDKNativeBridgeUnity.Instance.GetRemoteConfigDouble(configKey);
        }

        public long? GetRemoteConfigLong(string configKey)
        {
            return JustTrackSDKNativeBridgeUnity.Instance.GetRemoteConfigLong(configKey);
        }

        public string? GetRemoteConfigString(string configKey)
        {
            return JustTrackSDKNativeBridgeUnity.Instance.GetRemoteConfigString(configKey);
        }

        public Assignment[] GetAllAssignments()
        {
            string? assignmentsJson = JustTrackSDKNativeBridgeUnity.Instance.GetAllAssignments();
            return Assignment.FromJsonArray(assignmentsJson);
        }
    }
}
#endif
