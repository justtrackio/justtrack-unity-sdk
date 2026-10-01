#if UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;

namespace JustTrack
{
    /// <summary>
    /// WebGL-specific implementation of the SDK agent that communicates with the JavaScript Web SDK.
    /// </summary>
    internal class SDKWebGLAgent : ISDKAgent
    {
        internal static SDKWebGLAgent INSTANCE = new SDKWebGLAgent();
        private bool initialized = false;

        private SDKWebGLAgent() {}

#pragma warning disable SA1309 // Field names should not begin with underscore (required for native interop)
        [DllImport("__Internal")]
        private static extern void _justtrack_webgl_init(string apiKey, string bundleId, string appVersion, string appCode, string userId, bool manualStart, bool isLoggingEnabled, string gameObjectName, string callbackMethodName);

        [DllImport("__Internal")]
        private static extern void _justtrack_webgl_start();

        [DllImport("__Internal")]
        private static extern void _justtrack_webgl_stop();

        [DllImport("__Internal")]
        private static extern bool _justtrack_webgl_is_running();

        [DllImport("__Internal")]
        private static extern void _justtrack_webgl_anonymize(string gameObjectName, string callbackMethodName);

        [DllImport("__Internal")]
        private static extern void _justtrack_webgl_track_event(string eventName, string dimensionsJson, string valueJson, string gameObjectName, string callbackMethodName, string callbackId);

        [DllImport("__Internal")]
        private static extern void _justtrack_webgl_get_attribution(string gameObjectName, string callbackMethodName);
#pragma warning restore SA1309

#pragma warning disable SA1401, SA1307 // Fields should be private; field names should begin with upper-case letter (required for JSON serialization)
        [Serializable]
        public class WebGLAttributionResponse
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
        public class WebGLCallbackResponse
        {
            public bool success = false;
            public string? data = null;
            public string? error = null;
        }

        [Serializable]
        private class WebGLEventCallbackEnvelope
        {
            public string callbackId = "";
            public WebGLCallbackResponse response = new WebGLCallbackResponse();
        }
#pragma warning restore SA1401, SA1307

        private Action? anonymizeSuccessCallback = null;
        private Action<string>? anonymizeFailureCallback = null;
        private Action<AttributionResponse>? attributionListenerCallback = null;

        private Dictionary<string, Action> eventSuccessCallbacks = new Dictionary<string, Action>();
        private Dictionary<string, Action<string>> eventFailureCallbacks = new Dictionary<string, Action<string>>();

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
            var settings = JustTrackSettings.LoadFromResources();
            string bundleId = !string.IsNullOrEmpty(pBundleId) ? pBundleId : (!string.IsNullOrEmpty(settings?.WebglBundleId) ? settings.WebglBundleId : Application.identifier);
            string appVersion = !string.IsNullOrEmpty(pAppVersion) ? pAppVersion : (!string.IsNullOrEmpty(settings?.WebglAppVersion) ? settings.WebglAppVersion : Application.version);
            string appCode = !string.IsNullOrEmpty(pAppCode) ? pAppCode : (!string.IsNullOrEmpty(settings?.WebglAppCode) ? settings.WebglAppCode : appVersion);
            string userId = !string.IsNullOrEmpty(settings?.WebglUserId) ? settings.WebglUserId : (pCustomUserId ?? "");

            try
            {
                _justtrack_webgl_init(
                    pApiKey,
                    bundleId,
                    appVersion,
                    appCode,
                    userId,
                    pManualStart,
                    pEnableConsoleLogging,
                    "JustTrackSDKBehaviour",
                    "OnWebGLInitCallback"
                );

                initialized = true;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"[justtrack WebGL] Initialization error: {e.Message}");
            }
        }

        public void OnWebGLInitCallback(string responseJson)
        {
            // Init callback no longer carries attribution data
        }

        private Action<AttributionResponse>? getAttributionSuccess = null;
        private Action<string>? getAttributionFailure = null;

        public void GetAttribution(Action<AttributionResponse> pOnSuccess, Action<string> pOnFailure)
        {
            getAttributionSuccess = pOnSuccess;
            getAttributionFailure = pOnFailure;
            _justtrack_webgl_get_attribution("JustTrackSDKBehaviour", "OnWebGLGetAttributionCallback");
        }

        public void OnWebGLGetAttributionCallback(string responseJson)
        {
            try
            {
                WebGLCallbackResponse response = JsonUtility.FromJson<WebGLCallbackResponse>(responseJson);

                if (response.success && response.data != null)
                {
                    var attribution = ParseAttributionResponse(response.data);
                    JustTrackSDKBehaviour.CallOnMainThread(() =>
                    {
                        getAttributionSuccess?.Invoke(attribution);
                    });
                }
                else
                {
                    var errorMsg = response.error ?? "Unknown attribution error";
                    JustTrackSDKBehaviour.CallOnMainThread(() =>
                    {
                        getAttributionFailure?.Invoke(errorMsg);
                    });
                }
            }
            catch (Exception e)
            {
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    getAttributionFailure?.Invoke(e.Message);
                });
            }
        }

        private AttributionResponse ParseAttributionResponse(string attributionJson)
        {
            WebGLAttributionResponse parsed = JsonUtility.FromJson<WebGLAttributionResponse>(attributionJson);

            if (!int.TryParse(parsed.channelId, out int channelId))
            {
                channelId = 0;
            }
            bool channelIncent = parsed.channelIncent == "true";
            if (!int.TryParse(parsed.partnerId, out int partnerId))
            {
                partnerId = 0;
            }
            CultureInfo provider = CultureInfo.InvariantCulture;
            if (!DateTime.TryParseExact(parsed.createdAt, "yyyy-MM-dd'T'HH:mm:ssK", provider, DateTimeStyles.None, out DateTime createdAt))
            {
                createdAt = DateTime.UtcNow;
            }

            return AttributionResponse.CreateResponse(
                parsed.userType,
                parsed.campaignId,
                parsed.campaignName,
                parsed.campaignType,
                channelId,
                parsed.channelName,
                channelIncent,
                partnerId,
                parsed.partnerName,
                parsed.sourceId,
                parsed.sourceBundleId,
                parsed.sourcePlacement,
                parsed.adsetId,
                createdAt
            );
        }

        public void Start()
        {
            try
            {
                _justtrack_webgl_start();
            }
            catch (Exception)
            {
            }
        }

        public void Stop()
        {
            try
            {
                _justtrack_webgl_stop();
            }
            catch (Exception)
            {
            }
        }

        public void Anonymize(Action pOnSuccess, Action<string> pOnFailure)
        {
            anonymizeSuccessCallback = pOnSuccess;
            anonymizeFailureCallback = pOnFailure;

            try
            {
                _justtrack_webgl_anonymize("JustTrackSDKBehaviour", "OnWebGLAnonymizeCallback");
            }
            catch (Exception e)
            {
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    pOnFailure(e.Message);
                });
            }
        }

        public void OnWebGLAnonymizeCallback(string responseJson)
        {
            try
            {
                WebGLCallbackResponse response = JsonUtility.FromJson<WebGLCallbackResponse>(responseJson);

                if (response.success)
                {
                    JustTrackSDKBehaviour.CallOnMainThread(() =>
                    {
                        anonymizeSuccessCallback?.Invoke();
                    });
                }
                else
                {
                    JustTrackSDKBehaviour.CallOnMainThread(() =>
                    {
                        anonymizeFailureCallback?.Invoke(response.error ?? "Unknown anonymize error");
                    });
                }
            }
            catch (Exception e)
            {
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    anonymizeFailureCallback?.Invoke(e.Message);
                });
            }
        }

        public bool IsRunning()
        {
            try
            {
                return _justtrack_webgl_is_running();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void RegisterAttributionListener(Action<AttributionResponse> pListener)
        {
            lock (this)
            {
                if (attributionListenerCallback == null)
                {
                    attributionListenerCallback = pListener;
                }
                else
                {
                    attributionListenerCallback += pListener;
                }
            }
        }

        public void PublishEvent(AppEvent pEvent, Action? pOnSuccess, Action<string>? pOnFailure)
        {
            try
            {
                string dimensionsJson = "{}";
                if (pEvent.Dimensions.Count > 0)
                {
                    var dimensionPairs = new System.Collections.Generic.List<string>();
                    foreach (var kvp in pEvent.Dimensions)
                    {
                        string jsonValue = kvp.Value != null ? JsonUtility.ToJson(new StringWrapper { value = kvp.Value.ToString() }) : "null";
                        if (jsonValue.StartsWith("{\"value\":\"") && jsonValue.EndsWith("\"}"))
                        {
                            jsonValue = jsonValue.Substring(10, jsonValue.Length - 12);
                        }
                        dimensionPairs.Add($"\"{kvp.Key}\":\"{jsonValue}\"");
                    }
                    dimensionsJson = "{\"dimensions\":{" + string.Join(",", dimensionPairs.ToArray()) + "}}";
                }

                string valueJson = "null";
                if (pEvent.Unit != null)
                {
                    valueJson = JsonUtility.ToJson(new ValueWrapper
                    {
                        value = pEvent.Value,
                        unit = pEvent.Unit.ToString().ToLower()
                    });
                }
                else if (pEvent.Currency != null)
                {
                    valueJson = JsonUtility.ToJson(new MoneyWrapper
                    {
                        value = pEvent.Value,
                        currency = pEvent.Currency
                    });
                }

                bool hasCallbacks = pOnSuccess != null || pOnFailure != null;
                string callbackId = hasCallbacks ? Guid.NewGuid().ToString() : "";
                if (pOnSuccess != null)
                {
                    eventSuccessCallbacks[callbackId] = pOnSuccess;
                }
                if (pOnFailure != null)
                {
                    eventFailureCallbacks[callbackId] = pOnFailure;
                }

                string callbackMethodName = hasCallbacks ? "OnWebGLEventCallback" : "";

                _justtrack_webgl_track_event(
                    pEvent.Name,
                    dimensionsJson,
                    valueJson,
                    "JustTrackSDKBehaviour",
                    callbackMethodName,
                    callbackId
                );
            }
            catch (Exception e)
            {
                if (pOnFailure != null)
                {
                    JustTrackSDKBehaviour.CallOnMainThread(() =>
                    {
                        pOnFailure(e.Message);
                    });
                }
            }
        }

#pragma warning disable SA1401, SA1307 // Fields should be private; field names should begin with upper-case letter (required for JSON serialization)
        [Serializable]
        private class StringWrapper
        {
            public string value = "";
        }

        [Serializable]
        private class ValueWrapper
        {
            public double value;
            public string unit = "";
        }

        [Serializable]
        private class MoneyWrapper
        {
            public double value;
            public string currency = "";
        }
#pragma warning restore SA1401, SA1307

        public void OnWebGLEventCallback(string callbackData)
        {
            string callbackId = "";
            try
            {
                WebGLEventCallbackEnvelope envelope = JsonUtility.FromJson<WebGLEventCallbackEnvelope>(callbackData);
                callbackId = envelope.callbackId;
                WebGLCallbackResponse response = envelope.response;

                if (string.IsNullOrEmpty(callbackId) || response == null)
                {
                    return;
                }

                if (response.success)
                {
                    if (eventSuccessCallbacks.TryGetValue(callbackId, out var successCallback))
                    {
                        JustTrackSDKBehaviour.CallOnMainThread(() =>
                        {
                            successCallback?.Invoke();
                        });
                    }
                }
                else
                {
                    if (eventFailureCallbacks.TryGetValue(callbackId, out var failureCallback))
                    {
                        JustTrackSDKBehaviour.CallOnMainThread(() =>
                        {
                            failureCallback?.Invoke(response.error ?? "Unknown event tracking error");
                        });
                    }
                }

                eventSuccessCallbacks.Remove(callbackId);
                eventFailureCallbacks.Remove(callbackId);
            }
            catch (Exception e)
            {
                if (!string.IsNullOrEmpty(callbackId) && eventFailureCallbacks.TryGetValue(callbackId, out var failureCallback))
                {
                    JustTrackSDKBehaviour.CallOnMainThread(() =>
                    {
                        failureCallback?.Invoke(e.Message);
                    });
                }

                if (!string.IsNullOrEmpty(callbackId))
                {
                    eventSuccessCallbacks.Remove(callbackId);
                    eventFailureCallbacks.Remove(callbackId);
                }
            }
        }

        public bool IsInitialized()
        {
            return initialized;
        }
    }
}
#endif
