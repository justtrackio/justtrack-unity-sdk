#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace JustTrack
{
    internal sealed class SDKAndroidAgent : ISDKAgent, IDisposable
    {
        internal const string Package = "io.justtrack";
        internal const string BuilderClassPath = "JustTrackSdkBuilder";
        internal const string IntegrationManagerClassPath = "IntegrationManager";
        internal const string PlatformTypeClassPath = "PlatformType";
        internal const string EventClassPath = "AppEvent";
        internal const string AdImpressionClassPath = "ads.AdImpression";
        internal const string MoneyClassPath = "events.Money";

        internal const string FirebaseAdapterFullPath = "io.justtrack.integrations.firebase.FirebaseIntegrationAdapter";
        internal const string ApplovinMaxAdapterFullPath = "io.justtrack.integrations.applovin.AppLovinMaxIntegrationAdapter";
        internal const string IronsourceAdapterFullPath = "io.justtrack.integrations.ironsource.IronSourceIntegrationAdapter";
        internal const string UnityadsAdapterFullPath = "io.justtrack.integrations.unityads.UnityAdsIntegrationAdapter";

        private AndroidJavaObject INSTANCE = null!;
        private ExceptionHandler exceptionHandler;

        internal SDKAndroidAgent() 
        {
            exceptionHandler = new ExceptionHandler();
            exceptionHandler.OnReceiveException += (message, stackTrace) => {
                WriteToLogFile(message, stackTrace);
            };
        }

        private AndroidJavaObject GetRemoteConfig()
        {
            return INSTANCE.Call<AndroidJavaObject>("getRemoteConfig");
        }

        private void RegisterSimpleCallback(AndroidJavaObject responseFuture, Action onSuccess, Action<string> onFailure)
        {
            Action<AndroidJavaObject> onResolve = (response) =>
            {
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    onSuccess();
                });
            };
            Action<string> onReject = (err) =>
            {
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    onFailure(err);
                });
            };
            responseFuture.Call("registerCallback", new Callback(onResolve, onReject));
        }


        private void RegisterStringCallback(AndroidJavaObject responseFuture, Action<string> onSuccess, Action<string> onFailure)
        {
            Action<string> onResolve = (value) =>
            {
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    onSuccess(value);
                });
            };
            Action<string> onReject = (err) =>
            {
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    onFailure(err);
                });
            };
            responseFuture.Call("registerCallback", new StringCallback(onResolve, onReject));
        }

        private void RegisterIntCallback(AndroidJavaObject responseFuture, Action<int?> onSuccess, Action<string> onFailure)
        {
            Action<int?> onResolve = (value) =>
            {
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    onSuccess(value);
                });
            };
            Action<string> onReject = (err) =>
            {
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    onFailure(err);
                });
            };
            responseFuture.Call("registerCallback", new IntCallback(onResolve, onReject));
        }

        private void RegisterCallbackWithTransform<TResult>(
            AndroidJavaObject responseFuture,
            Func<AndroidJavaObject, TResult> transform,
            Action<TResult> onSuccess,
            Action<string> onFailure)
        {
            Action<AndroidJavaObject> onResolve = (obj) =>
            {
                var result = transform(obj);
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    onSuccess(result);
                });
            };
            Action<string> onReject = (err) =>
            {
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    onFailure(err);
                });
            };
            responseFuture.Call("registerCallback", new Callback(onResolve, onReject));
        }

        private void RegisterNullableCallbackWithTransform<TResult>(
            AndroidJavaObject responseFuture,
            Func<AndroidJavaObject, TResult> transform,
            Action<TResult?> onSuccess,
            Action<string> onFailure) where TResult : class
        {
            Action<AndroidJavaObject> onResolve = (obj) =>
            {
                var result = obj == null ? default(TResult) : transform(obj);
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    onSuccess(result);
                });
            };
            Action<string> onReject = (err) =>
            {
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    onFailure(err);
                });
            };
            responseFuture.Call("registerCallback", new Callback(onResolve, onReject));
        }

        public void Dispose()
        {
            if (INSTANCE != null)
            {
                INSTANCE.Dispose();
                INSTANCE = null!;
            }
        }

        private Action? onStart = null;

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
            using var builder = new AndroidJavaObject($"{Package}.{BuilderClassPath}", CurrentActivity(), pApiKey);
            if (!String.IsNullOrEmpty(pTrackingId))
            {
                builder.Call<AndroidJavaObject>("setTrackingId", pTrackingId, pTrackingProvider)?.Dispose();
            }
            if (!String.IsNullOrEmpty(pCustomUserId))
            {
                builder.Call<AndroidJavaObject>("setUserId", pCustomUserId)?.Dispose();
            }

            if (!string.IsNullOrEmpty(pBundleId))
            {
                builder.Call<AndroidJavaObject>("setPackageName", pBundleId)?.Dispose();
            }
            if (!string.IsNullOrEmpty(pAppVersion) && !string.IsNullOrEmpty(pAppCode))
            {
                builder.Call<AndroidJavaObject>("setApplicationVersion", pAppVersion, pAppCode)?.Dispose();
            }

            string? serverUrl = pServerUrl;

            builder.Call<AndroidJavaObject>("setAutomaticInAppPurchaseTracking", pAutomaticInAppPurchaseTracking)?.Dispose();
            using var platformTypeClass = new AndroidJavaClass($"{Package}.{PlatformTypeClassPath}");
            using var platformType = platformTypeClass.GetStatic<AndroidJavaObject>("UNITY");
            builder.Call<AndroidJavaObject>("setPlatformType", platformType)?.Dispose();
            builder.Call<AndroidJavaObject>("setManualStart", pManualStart)?.Dispose();
            builder.Call<AndroidJavaObject>("setLoggingEnabled", pEnableConsoleLogging)?.Dispose();
            builder.Call<AndroidJavaObject>("setEnableConnectionTracking", pEnableConnectionTracking)?.Dispose();
            if (!string.IsNullOrEmpty(serverUrl))
            {
                builder.Call<AndroidJavaObject>("setServerUrl", serverUrl)?.Dispose();
            }
            builder.Call<AndroidJavaObject>("runCallbacksSerially")?.Dispose();
            INSTANCE = builder.Call<AndroidJavaObject>("build");
        }

        public void Start()
        {
            INSTANCE.Call("start");
            if (onStart != null)
            {
                onStart.Invoke();
                onStart = null;
            }
        }

        public void Stop()
        {
            INSTANCE.Call("stop");
        }

        public void Anonymize(Action pOnSuccess, Action<string> pOnFailure)
        {
            using var responseFuture = INSTANCE.Call<AndroidJavaObject>("anonymize");
            RegisterSimpleCallback(responseFuture, pOnSuccess, pOnFailure);
        }

        public bool IsRunning()
        {
            if (INSTANCE != null)
            {
                return INSTANCE.Call<bool>("isRunning");
            }
            else
            {
                return false;
            }
        }

        public void GetRetargetingParameters(Action<RetargetingParameters?> pOnSuccess, Action<string> pOnFailure)
        {
            using var responseFuture = INSTANCE.Call<AndroidJavaObject>("getRetargetingParameters");
            RegisterNullableCallbackWithTransform(responseFuture, RetargetingParameters.FromAndroidObject, pOnSuccess, pOnFailure);
        }

        public PreliminaryRetargetingParameters? GetPreliminaryRetargetingParameters()
        {
            using var parameters = INSTANCE.Call<AndroidJavaObject>("getPreliminaryRetargetingParameters");

            if (parameters == null)
            {
                return null;
            }

            return PreliminaryRetargetingParameters.FromAndroidObject(parameters, INSTANCE);
        }

        public void RegisterAttributionListener(Action<AttributionResponse> pListener)
        {
            INSTANCE.Call<AndroidJavaObject>("registerAttributionListener", new AttributionListener((attribution) =>
            {
                var response = AttributionResponse.FromAndroidObject(attribution);
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    pListener(response);
                });
            }))?.Dispose();
        }

        public void GetAttribution(Action<AttributionResponse> pOnSuccess, Action<string> pOnFailure)
        {
            using var responseFuture = INSTANCE.Call<AndroidJavaObject>("getAttribution");
            Action<AndroidJavaObject> onResolve = (obj) =>
            {
                var response = AttributionResponse.FromAndroidObject(obj);
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    pOnSuccess(response);
                });
            };
            Action<string> onReject = (err) =>
            {
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    pOnFailure(err);
                });
            };
            responseFuture.Call("registerCallback", new Callback(onResolve, onReject));
        }

        public void RegisterRetargetingParameterListener(Action<RetargetingParameters> pListener)
        {
            INSTANCE.Call<AndroidJavaObject>("registerRetargetingParametersListener", new RetargetingParametersListener((parameters) =>
            {
                var response = RetargetingParameters.FromAndroidObject(parameters);
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    pListener(response);
                });
            }))?.Dispose();
        }

        public void RegisterPreliminaryRetargetingListener(Action<PreliminaryRetargetingParameters> pListener)
        {
            INSTANCE.Call<AndroidJavaObject>("registerPreliminaryRetargetingParametersListener", new PreliminaryRetargetingParametersListener((parameters) =>
            {
                var response = PreliminaryRetargetingParameters.FromAndroidObject(parameters, INSTANCE);
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    pListener(response);
                });
            }))?.Dispose();
        }

        public void IntegrateWithAppLovin(string? customUserId, Action pOnSuccess, Action<string> pOnFailure)
        {
            try
            {
                using var appLovinAdapter = new AndroidJavaObject(ApplovinMaxAdapterFullPath, customUserId);
                INSTANCE.Call("integrateWith", appLovinAdapter);
                pOnSuccess();
            }
            catch (Exception e)
            {
                pOnFailure(e.Message);
            }
        }

        public void IntegrateWithIronSource(string? customUserId, Action pOnSuccess, Action<string> pOnFailure)
        {
            try
            {
                using var ironSourceAdapter = new AndroidJavaObject(IronsourceAdapterFullPath, customUserId);
                INSTANCE.Call("integrateWith", ironSourceAdapter);
                pOnSuccess();
            }
            catch (Exception e)
            {
                pOnFailure(e.Message);
            }
        }

        public void IntegrateWithUnityAds(Action pOnSuccess, Action<string> pOnFailure)
        {
            try
            {
                using var unityAdsAdapter = new AndroidJavaObject(UnityadsAdapterFullPath);
                INSTANCE.Call("integrateWith", unityAdsAdapter);
                pOnSuccess();
            }
            catch (Exception e)
            {
                pOnFailure(e.Message);
            }
        }

        public void IntegrateWithFirebase(Action pOnSuccess, Action<string> pOnFailure)
        {
            try
            {
                using var firebaseAdapter = new AndroidJavaObject(FirebaseAdapterFullPath);
                INSTANCE.Call("integrateWith", firebaseAdapter);
                pOnSuccess();
            }
            catch (Exception e)
            {
                pOnFailure(e.Message);
            }
        }

        public void IntegrateWithGoogleOdm(Action pOnSuccess, Action<string> pOnFailure)
        {
            pOnFailure("Google ODM integration is only available on iOS");
        }

        public void ForwardAdImpression(AdImpression pAdImpression, Action pOnSuccess, Action<string> pOnFailure)
        {
            AndroidJavaObject? revenueObject;
            if (pAdImpression.Revenue != null)
            {
                revenueObject = new AndroidJavaObject($"{Package}.{MoneyClassPath}", pAdImpression.Revenue.Value, pAdImpression.Revenue.Currency);
            } else
            {
                revenueObject = null;
            }

            using var impression = new AndroidJavaObject(
                $"{Package}.{AdImpressionClassPath}",
                pAdImpression.Unit,
                pAdImpression.SdkName,
                pAdImpression.Network,
                pAdImpression.Placement,
                pAdImpression.TestGroup,
                pAdImpression.SegmentName,
                pAdImpression.InstanceName,
                pAdImpression.BundleId,
                revenueObject,
                (AndroidJavaObject?)null
            );


            using var responseFuture = INSTANCE.Call<AndroidJavaObject>("forwardAdImpression", impression);
            RegisterSimpleCallback(responseFuture, pOnSuccess, pOnFailure);
        }

        public void SetCustomUserId(string pCustomUserId)
        {
            INSTANCE.Call<AndroidJavaObject>("setUserId", pCustomUserId)?.Dispose();
        }

        public void SetAutomaticInAppPurchaseTracking(bool pEnabled)
        {
            INSTANCE.Call("setAutomaticInAppPurchaseTracking", pEnabled);
        }

        public void SetFirebaseAppInstanceId(string pFirebaseAppInstanceId)
        {
            INSTANCE.Call<AndroidJavaObject>("setFirebaseAppInstanceId", pFirebaseAppInstanceId)?.Dispose();
        }

        public void SetGlobalDimension0(string? value)
        {
            INSTANCE.Call("setGlobalDimension0", value);
        }

        public void SetGlobalDimension1(string? value)
        {
            INSTANCE.Call("setGlobalDimension1", value);
        }

        public void SetGlobalDimension2(string? value)
        {
            INSTANCE.Call("setGlobalDimension2", value);
        }

        public void PublishEvent(AppEvent pEvent, Action? pOnSuccess, Action<string>? pOnFailure)
        {
            using var javaEvent = new AndroidJavaObject($"{Package}.{EventClassPath}", pEvent.Name);
            if (Unit.Count == pEvent.Unit)
            {
                javaEvent.Call<AndroidJavaObject>("setCount", pEvent.Value)?.Dispose();
            }
            if (Unit.Milliseconds == pEvent.Unit)
            {
                javaEvent.Call<AndroidJavaObject>("setMilliseconds", pEvent.Value)?.Dispose();
            }
            if (Unit.Seconds == pEvent.Unit)
            {
                javaEvent.Call<AndroidJavaObject>("setSeconds", pEvent.Value)?.Dispose();
            }
            if (pEvent.Currency != null && pEvent.Currency != "")
            {
                using var money = new AndroidJavaObject($"{Package}.{MoneyClassPath}", pEvent.Value, pEvent.Currency);
                javaEvent.Call<AndroidJavaObject>("setValue", money)?.Dispose();
            }
            foreach (var dimension in pEvent.Dimensions)
            {
                javaEvent.Call<AndroidJavaObject>("addDimension", dimension.Key, dimension.Value)?.Dispose();
            }

            if (pOnSuccess != null || pOnFailure != null)
            {
                try
                {
                    using var responseFuture = INSTANCE.Call<AndroidJavaObject>("publishEvent", javaEvent);
                    RegisterSimpleCallback(responseFuture, () => pOnSuccess?.Invoke(), (err) => pOnFailure?.Invoke(err));
                }
                catch (Exception e)
                {
                    JustTrackSDKBehaviour.CallOnMainThread(() =>
                    {
                        pOnFailure?.Invoke(e.Message);
                    });
                }
            }
            else
            {
                try
                {
                    INSTANCE.Call<AndroidJavaObject>("publishEvent", javaEvent);
                }
                catch (Exception)
                {
                }
            }
        }

        public void GetInstallInstanceId(Action<string> pOnSuccess, Action<string> pOnFailure)
        {
            using var responseFuture = INSTANCE.Call<AndroidJavaObject>("getInstallInstanceId");
            RegisterStringCallback(responseFuture, pOnSuccess, pOnFailure);
        }

        public void GetAdvertiserIdInfo(Action<AdvertiserIdInfo> pOnSuccess, Action<string> pOnFailure)
        {
            using var responseFuture = INSTANCE.Call<AndroidJavaObject>("getAdvertiserIdInfo");
            RegisterCallbackWithTransform(responseFuture, (obj) => {
                return new AdvertiserIdInfo(
                    obj.Call<string?>("getAdvertiserId"),
                    obj.Call<bool>("isLimitedAdTracking")
                );
            }, pOnSuccess, pOnFailure);
        }

        public void ForwardTransaction(string token, string productId, Money money, ProductType productType)
        {
            using var revenue = new AndroidJavaObject($"{Package}.{MoneyClassPath}", money.Value, money.Currency);
            if (productType == ProductType.INAPP) {
                INSTANCE.Call<bool>("forwardInApp", productId, token, revenue);
            } else {
                INSTANCE.Call<bool>("forwardSubscription", productId, token, revenue);
            }
        }

        private AndroidJavaObject CurrentActivity()
        {
            using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                return activity.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }

        public bool IsInitialized()
        {
            return INSTANCE != null;
        }

        private void WriteToLogFile(string message, string stackTrace)
        {
            if (!IsInternalException(stackTrace))
            {
                return;
            }

            try
            {
                string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                string filePath = Path.Combine(GetInternalFilesDir(), $"io_justtrack_native_stacktrace_{timestamp}.json");
                string exceptionName = ExtractExceptionName(message);

                var crashData = new CrashData
                {
                    timestamp = timestamp,
                    stacktrace = stackTrace,
                    name = exceptionName,
                    reason = message,
                    crashType = 3
                };

                var crashWrapper = new CrashWrapper
                {
                    data = crashData
                };

                string jsonString = JsonUtility.ToJson(crashWrapper, prettyPrint: true);

                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to write JSON crash file: " + e.Message);
            }
        }

        private string ExtractExceptionName(string message)
        {
            int colonIndex = message.IndexOf(':');
            if (colonIndex > 0)
            {
                return message.Substring(0, colonIndex).Trim();
            }
            return "UnknownException";
        }

        private string GetInternalFilesDir()
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject filesDir = currentActivity.Call<AndroidJavaObject>("getFilesDir"))
            {
                return filesDir.Call<string>("getAbsolutePath");
            }
        }

        private bool IsInternalException(string stacktrace)
        {
            if (string.IsNullOrEmpty(stacktrace)) return false;

            return stacktrace.IndexOf("JustTrack", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public void SetExperimentVariant(string experiment, string variant, string[]? tags, DateTime? happenedAt, Action pOnSuccess, Action<string> pOnFailure)
        {
            AndroidJavaObject responseFuture;
            if (tags != null && happenedAt != null)
            {
                using var tagsArray = new AndroidJavaObject("java.util.ArrayList");
                foreach (var tag in tags)
                {
                    tagsArray.Call<bool>("add", tag);
                }
                using var dateClass = new AndroidJavaClass("java.util.Date");
                using var happenedAtDate = new AndroidJavaObject("java.util.Date", happenedAt.Value.Ticks / 10000 - 62135596800000);
                responseFuture = INSTANCE.Call<AndroidJavaObject>("setExperimentVariant", experiment, variant, tagsArray, happenedAtDate);
            }
            else if (tags != null)
            {
                using var tagsArray = new AndroidJavaObject("java.util.ArrayList");
                foreach (var tag in tags)
                {
                    tagsArray.Call<bool>("add", tag);
                }
                responseFuture = INSTANCE.Call<AndroidJavaObject>("setExperimentVariant", experiment, variant, tagsArray, null);
            }
            else if (happenedAt != null)
            {
                using var dateClass = new AndroidJavaClass("java.util.Date");
                using var happenedAtDate = new AndroidJavaObject("java.util.Date", happenedAt.Value.Ticks / 10000 - 62135596800000);
                responseFuture = INSTANCE.Call<AndroidJavaObject>("setExperimentVariant", experiment, variant, null, happenedAtDate);
            }
            else
            {
                responseFuture = INSTANCE.Call<AndroidJavaObject>("setExperimentVariant", experiment, variant, null, null);
            }

            RegisterSimpleCallback(responseFuture, pOnSuccess, pOnFailure);
            responseFuture.Dispose();
        }

        public void FetchRemoteConfig(Action pOnSuccess, Action<string> pOnFailure)
        {
            AndroidJavaObject remoteConfigObject = GetRemoteConfig();
            AndroidJavaObject responseFuture = remoteConfigObject.Call<AndroidJavaObject>("fetch");

            RegisterSimpleCallback(responseFuture, pOnSuccess, pOnFailure);
        }

        public void ActivateRemoteConfig(string[] experiments, Action pOnSuccess, Action<string> pOnFailure)
        {
            using var experimentList = new AndroidJavaObject("java.util.ArrayList");
            foreach (var experiment in experiments)
            {
                experimentList.Call<bool>("add", experiment);
            }
            AndroidJavaObject remoteConfigObject = GetRemoteConfig();
            AndroidJavaObject responseFuture = remoteConfigObject.Call<AndroidJavaObject>("activate", experimentList);

            RegisterSimpleCallback(responseFuture, pOnSuccess, pOnFailure);
        }

        public void FetchAndActivateRemoteConfig(Action pOnSuccess, Action<string> pOnFailure)
        {
            AndroidJavaObject remoteConfigObject = GetRemoteConfig();
            AndroidJavaObject responseFuture = remoteConfigObject.Call<AndroidJavaObject>("fetchAndActivate");

            RegisterSimpleCallback(responseFuture, pOnSuccess, pOnFailure);
        }

        public void SetRemoteConfigSettings(JusttrackRemoteConfigSettings settings)
        {
            AndroidJavaObject remoteConfigObject = GetRemoteConfig();
            using var settingsObject = new AndroidJavaObject("io.justtrack.config.JusttrackRemoteConfigSettings", settings.minimumFetchIntervalInSeconds);
            remoteConfigObject.Call("setConfig", settingsObject);
        }

        public Assignment[] GetAllAssignments()
        {
            AndroidJavaObject remoteConfigObject = GetRemoteConfig();
            using var allAssignments = remoteConfigObject.Call<AndroidJavaObject>("getAll");

            int size = allAssignments.Call<int>("size");
            List<Assignment> assignments = new List<Assignment>(size);
            for (int i = 0; i < size; i++)
            {
                using var assignmentObject = allAssignments.Call<AndroidJavaObject>("get", i);
                Assignment? assignment = Assignment.FromAndroidObject(assignmentObject);
                if (assignment != null)
                {
                    assignments.Add(assignment);
                }
            }

            return assignments.ToArray();
        }

        public bool? GetRemoteConfigBoolean(string configKey)
        {
            AndroidJavaObject remoteConfigObject = INSTANCE.Call<AndroidJavaObject>("getRemoteConfig");
            using var boxedValue = remoteConfigObject.Call<AndroidJavaObject>("getBoolean", configKey);
            if (boxedValue == null)
            {
                return null;
            }
            return boxedValue.Call<bool>("booleanValue");
        }

        public double? GetRemoteConfigDouble(string configKey)
        {
            AndroidJavaObject remoteConfigObject = INSTANCE.Call<AndroidJavaObject>("getRemoteConfig");
            using var boxedValue = remoteConfigObject.Call<AndroidJavaObject>("getDouble", configKey);
            if (boxedValue == null)
            {
                return null;
            }
            return boxedValue.Call<double>("doubleValue");
        }

        public int? GetRemoteConfigInt(string configKey)
        {
            AndroidJavaObject remoteConfigObject = INSTANCE.Call<AndroidJavaObject>("getRemoteConfig");
            using var boxedValue = remoteConfigObject.Call<AndroidJavaObject>("getInt", configKey);
            if (boxedValue == null)
            {
                return null;
            }
            return boxedValue.Call<int>("intValue");
        }

        public long? GetRemoteConfigLong(string configKey)
        {
            AndroidJavaObject remoteConfigObject = INSTANCE.Call<AndroidJavaObject>("getRemoteConfig");
            using var boxedValue = remoteConfigObject.Call<AndroidJavaObject>("getLong", configKey);
            if (boxedValue == null)
            {
                return null;
            }
            return boxedValue.Call<long>("longValue");
        }

        public string? GetRemoteConfigString(string configKey)
        {
            AndroidJavaObject remoteConfigObject = INSTANCE.Call<AndroidJavaObject>("getRemoteConfig");
            return remoteConfigObject.Call<string>("getString", configKey);
        }
    }
}
#endif
