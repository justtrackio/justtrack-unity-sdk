using System;
using System.Collections.Generic;
using System.Linq;

namespace JustTrack
{
    /// <summary>
    /// Main SDK class providing access to JustTrack analytics functionality.
    /// </summary>
    public class JustTrackSDK
    {
        private static ISDKAgent initializedAgent = null!;
        private static readonly object InitInProgressLock = new object();
        private static readonly object InitLock = new object();
        private static Action? onInit = null;

        private static ISDKAgent Agent
        {
            get
            {
                if (initializedAgent == null)
                {
#if UNITY_EDITOR
                    initializedAgent = new SDKEditorAgent();
#elif UNITY_WEBGL
                    initializedAgent = SDKWebGLAgent.INSTANCE;
#elif UNITY_ANDROID
                    initializedAgent = new SDKAndroidAgent();
#elif UNITY_IOS
                    initializedAgent = SDKiOSAgent.INSTANCE;
#endif
                }

                return initializedAgent;
            }
        }

        private static void WaitForInitialization(Action pOnInit)
        {
            lock (InitLock)
            {
                if (onInit == null)
                {
                    onInit = pOnInit;
                }
                else
                {
                    onInit += pOnInit;
                }
            }

            CheckAfterInit();
        }

        private static void CheckAfterInit()
        {
            Action? toCall = null;
            lock (InitLock)
            {
                if (!Agent.IsInitialized())
                {
                    return;
                }

                toCall = onInit;
                onInit = null;
            }

            if (toCall != null)
            {
                toCall();
            }
        }

        private static event Action<AttributionResponse>? SOnAttributionResponse;

#if !UNITY_WEBGL
        private static IRemoteConfig? remoteConfig;
#endif

        /// <summary>
        /// Event that is triggered when the attribution response is received.
        /// </summary>
        public static event Action<AttributionResponse> OnAttributionResponse
        {
            add
            {
                if (SOnAttributionResponse == null || !SOnAttributionResponse.GetInvocationList().Contains(value))
                {
                    SOnAttributionResponse += value;
                }
            }

            remove
            {
                if (SOnAttributionResponse != null && SOnAttributionResponse.GetInvocationList().Contains(value))
                {
                    SOnAttributionResponse -= value;
                }
            }
        }

        private static event Action<RetargetingParameters>? SOnRetargetingParameters;

        /// <summary>
        /// Event that is triggered when retargeting parameters are received.
        /// </summary>
        public static event Action<RetargetingParameters> OnRetargetingParameters
        {
            add
            {
                if (SOnRetargetingParameters == null || !SOnRetargetingParameters.GetInvocationList().Contains(value))
                {
                    SOnRetargetingParameters += value;
                }
            }

            remove
            {
                if (SOnRetargetingParameters != null && SOnRetargetingParameters.GetInvocationList().Contains(value))
                {
                    SOnRetargetingParameters -= value;
                }
            }
        }

        private static event Action<PreliminaryRetargetingParameters>? SOnPreliminaryRetargetingParameters;

        /// <summary>
        /// Event that is triggered when preliminary retargeting parameters are received.
        /// </summary>
        public static event Action<PreliminaryRetargetingParameters> OnPreliminaryRetargetingParameters
        {
            add
            {
                if (SOnPreliminaryRetargetingParameters == null || !SOnPreliminaryRetargetingParameters.GetInvocationList().Contains(value))
                {
                    SOnPreliminaryRetargetingParameters += value;
                }
            }

            remove
            {
                if (SOnPreliminaryRetargetingParameters != null && SOnPreliminaryRetargetingParameters.GetInvocationList().Contains(value))
                {
                    SOnPreliminaryRetargetingParameters -= value;
                }
            }
        }

        /// <summary>
        /// Initializes the justtrack SDK with the specified configuration.
        /// </summary>
        /// <param name="pApiKey">The API key for the justtrack SDK.</param>
        /// <param name="pTrackingId">Optional tracking ID.</param>
        /// <param name="pTrackingProvider">Optional tracking provider.</param>
        /// <param name="pCustomUserId">Optional custom user ID.</param>
        /// <param name="pAutomaticInAppPurchaseTracking">Whether to enable automatic in-app purchase tracking.</param>
        /// <param name="pEnableDebugMode">Whether to enable debug mode.</param>
        /// <param name="pManualStart">Whether to use manual start mode.</param>
        /// <param name="pEnableConsoleLogging">Whether to enable console logging.</param>
        /// <param name="pOnSuccess">Callback invoked when initialization succeeds.</param>
        /// <param name="pOnFailure">Callback invoked when initialization fails.</param>
        public static void Init(
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
            if (string.IsNullOrEmpty(pApiKey))
            {
                return;
            }

            lock (InitInProgressLock)
            {
                if (Agent.IsInitialized())
                {
                    return;
                }

                Agent.Initialize(
                    pApiKey,
                    pTrackingId,
                    pTrackingProvider,
                    pCustomUserId,
                    pAutomaticInAppPurchaseTracking,
                    pEnableDebugMode,
                    pManualStart,
                    pEnableConsoleLogging,
                    (attribution) =>
                    {
                        if (pOnSuccess != null)
                        {
                            pOnSuccess(attribution);
                        }
                    },
                    (error) =>
                    {
                        if (pOnFailure != null)
                        {
                            pOnFailure(error);
                        }
                    });
            }

            CheckAfterInit();
            Agent.RegisterAttributionListener((attribution) =>
            {
                // already on the main thread
                if (SOnAttributionResponse != null)
                {
                    SOnAttributionResponse.Invoke(attribution);
                }
            });
#if !UNITY_WEBGL
            Agent.RegisterRetargetingParameterListener((parameters) =>
            {
                // already on the main thread
                if (SOnRetargetingParameters != null)
                {
                    SOnRetargetingParameters.Invoke(parameters);
                }
            });
            Agent.RegisterPreliminaryRetargetingListener((parameters) =>
            {
                // already on the main thread
                if (SOnPreliminaryRetargetingParameters != null)
                {
                    SOnPreliminaryRetargetingParameters.Invoke(parameters);
                }
            });

            remoteConfig = new RemoteConfig(Agent, WaitForInitialization);
#endif
        }

        /// <summary>
        /// Starts the SDK with default configuration.
        /// </summary>
        public static void Start()
        {
            WaitForInitialization(() => Agent.Start());
        }

        /// <summary>
        /// Once this API is invoked, our SDK no longer communicates with our servers and stops functioning. Useful when implementing user opt-in/opt-out.
        /// </summary>
        public static void Stop()
        {
            WaitForInitialization(() => Agent.Stop());
        }

        /// <summary>
        /// Anonymize a user's installs, events, and sessions.
        /// </summary>
        /// <param name="pOnSuccess">Callback for success.</param>
        /// <param name="pOnFailure">Callback for failure.</param>
        public static void Anonymize(Action pOnSuccess, Action<string> pOnFailure)
        {
            WaitForInitialization(() => Agent.Anonymize(pOnSuccess, pOnFailure));
        }

        /// <summary>
        /// Gets a value indicating whether the SDK is currently running.
        /// </summary>
        /// <returns><c>true</c> if the SDK is running; otherwise, <c>false</c>.</returns>
        public static bool IsRunning()
        {
            return Agent.IsRunning();
        }
#if !UNITY_WEBGL
        /// <summary>
        /// Gets the retargeting parameters.
        /// </summary>
        /// <param name="pOnSuccess">Callback for success.</param>
        /// <param name="pOnFailure">Callback for failure.</param>
        public static void GetRetargetingParameters(Action<RetargetingParameters?> pOnSuccess, Action<string> pOnFailure)
        {
            WaitForInitialization(() => Agent.GetRetargetingParameters(pOnSuccess, pOnFailure));
        }

        /// <summary>
        /// Gets the preliminary retargeting parameters.
        /// </summary>
        /// <returns>The preliminary retargeting parameters, or null if not available.</returns>
        public static PreliminaryRetargetingParameters? GetPreliminaryRetargetingParameters()
        {
            if (!Agent.IsInitialized())
            {
                return null;
            }

            return Agent.GetPreliminaryRetargetingParameters();
        }

        /// <summary>
        /// Listens for AppLovin impressions and automatically forwards them to the justtrack backend.
        /// You only have to call this if you are using AppLovin and didn't enable the integration
        /// on the prefab or if you want to specify the supplied user id.
        /// </summary>
        /// <param name="customUserId">Optional custom user ID for AppLovin.</param>
        /// <param name="pOnSuccess">Callback for success.</param>
        /// <param name="pOnFailure">Callback for failure.</param>
        public static void IntegrateWithAppLovin(string? customUserId, Action pOnSuccess, Action<string> pOnFailure)
        {
            WaitForInitialization(() => Agent.IntegrateWithAppLovin(customUserId, pOnSuccess, pOnFailure));
        }

        /// <summary>
        /// Automatically publish the Firebase App Instance ID to the justtrack backend.
        /// </summary>
        /// <param name="pOnSuccess">Callback for success.</param>
        /// <param name="pOnFailure">Callback for failure.</param>
        public static void IntegrateWithFirebase(Action pOnSuccess, Action<string> pOnFailure)
        {
            WaitForInitialization(() => Agent.IntegrateWithFirebase(pOnSuccess, pOnFailure));
        }

        /// <summary>
        /// Configure the IronSource SDK to use the user id from justtrack and forward ad impressions as user events.
        /// You only have to call this if you are not using the prefab (and are using IronSource). If you call this yourself, you have
        /// to wait for any callback to get called before you initialize IronSource itself.
        /// </summary>
        /// <param name="customUserId">Custom user identifier.</param>
        /// <param name="pOnSuccess">Callback for success.</param>
        /// <param name="pOnFailure">Callback for failure.</param>
        public static void IntegrateWithIronSource(string? customUserId, Action pOnSuccess, Action<string> pOnFailure)
        {
            WaitForInitialization(() => Agent.IntegrateWithIronSource(customUserId, pOnSuccess, pOnFailure));
        }

        /// <summary>
        /// Listens for UnityAds impressions and automatically forwards them to the justtrack backend.
        /// You only have to call this if you are using UnityAds and didn't enable the integration
        /// on the prefab.
        /// </summary>
        /// <param name="pOnSuccess">Callback invoked on successful integration.</param>
        /// <param name="pOnFailure">Callback invoked on integration failure with error message.</param>
        public static void IntegrateWithUnityAds(Action pOnSuccess, Action<string> pOnFailure)
        {
            WaitForInitialization(() => Agent.IntegrateWithUnityAds(pOnSuccess, pOnFailure));
        }

        /// <summary>
        /// Integrates with Google ODM (On Device Mediation) on iOS.
        /// Automatically forwards relevant data to the justtrack backend.
        /// </summary>
        /// <param name="pOnSuccess">Callback invoked on successful integration.</param>
        /// <param name="pOnFailure">Callback invoked on integration failure with error message.</param>
        public static void IntegrateWithGoogleOdm(Action pOnSuccess, Action<string> pOnFailure)
        {
            WaitForInitialization(() => Agent.IntegrateWithGoogleOdm(pOnSuccess, pOnFailure));
        }

        /// <summary>
        /// Forward an ad impression to the justtrack backend. Depending on the ad SDK we will use this
        /// data to display the correct amount of ad revenue your app generated.
        /// </summary>
        /// <param name="pAdImpression">The ad impression to forward.</param>
        /// <param name="pOnSuccess">Callback for success.</param>
        /// <param name="pOnFailure">Callback for failure.</param>
        public static void ForwardAdImpression(AdImpression pAdImpression, Action pOnSuccess, Action<string> pOnFailure)
        {
            if (pAdImpression.Revenue != null && (pAdImpression.Revenue.Value < 0 || pAdImpression.Revenue.Currency == null || pAdImpression.Revenue.Currency.Length != 3))
            {
                pOnFailure("Invalid AdImpression");
                return;
            }

            WaitForInitialization(() => Agent.ForwardAdImpression(pAdImpression, pOnSuccess, pOnFailure));
        }

        /// <summary>
        /// Forward an ad impression to the justtrack backend. Depending on the ad SDK we will use this
        /// data to display the correct amount of ad revenue your app generated.
        /// </summary>
        /// <param name="pAdImpression">The ad impression to forward.</param>
        public static void ForwardAdImpression(AdImpression pAdImpression)
        {
            ForwardAdImpression(pAdImpression, () => { }, (_) => { });
        }

        /// <summary>
        /// Forward a user id to the server.
        /// </summary>
        /// <param name="pCustomUserId">The custom user ID to set.</param>
        public static void SetUserId(string pCustomUserId)
        {
            WaitForInitialization(() => Agent.SetCustomUserId(pCustomUserId));
        }

        /// <summary>
        /// Enable and disable automatic in-app purchase tracking.
        /// </summary>
        /// <param name="pEnabled">Whether to enable automatic in-app purchase tracking.</param>
        public static void SetAutomaticInAppPurchaseTracking(bool pEnabled)
        {
            WaitForInitialization(() => Agent.SetAutomaticInAppPurchaseTracking(pEnabled));
        }

        /// <summary>
        /// Forward the firebase app instance id to the justtrack backend.
        /// </summary>
        /// <param name="pFirebaseAppInstanceId">The Firebase app instance ID.</param>
        /// <remarks>
        /// See https://firebase.google.com/docs/reference/android/com/google/firebase/analytics/FirebaseAnalytics#public-taskstring-getappinstanceid
        /// for how to obtain one.
        /// </remarks>
        public static void SetFirebaseAppInstanceId(string pFirebaseAppInstanceId)
        {
            WaitForInitialization(() => Agent.SetFirebaseAppInstanceId(pFirebaseAppInstanceId));
        }
#endif

        /// <summary>
        /// Publish a new user event to the server.
        /// </summary>
        /// <param name="pEvent">The event name to publish.</param>
        [Obsolete("Use Track() instead.")]
        public static void PublishEvent(string pEvent)
        {
            if (string.IsNullOrEmpty(pEvent))
            {
                return;
            }

            WaitForInitialization(() => Agent.PublishEvent(new AppEvent(pEvent)));
        }

        /// <summary>
        /// Publish a new user event to the server.
        /// </summary>
        /// <param name="pEvent">The event to publish.</param>
        [Obsolete("Use Track() instead.")]
        public static void PublishEvent(AppEvent pEvent)
        {
            WaitForInitialization(() => Agent.PublishEvent(pEvent));
        }

        /// <summary>
        /// Publish a new user event to the server.
        /// </summary>
        /// <param name="pEvent">The event to publish.</param>
        /// <param name="pOnSuccess">Callback for success.</param>
        /// <param name="pOnFailure">Callback for failure.</param>
        [Obsolete("Use Track() instead.")]
        public static void PublishEvent(string pEvent, Action pOnSuccess, Action<string> pOnFailure)
        {
            if (string.IsNullOrEmpty(pEvent))
            {
                pOnFailure?.Invoke("pEvent parameter cannot be null or empty.");
                return;
            }

            WaitForInitialization(() => Agent.PublishEvent(new AppEvent(pEvent), pOnSuccess, pOnFailure));
        }

        /// <summary>
        /// Publish a new user event to the server.
        /// </summary>
        /// <param name="pEvent">The event to publish.</param>
        /// <param name="pOnSuccess">Callback for success.</param>
        /// <param name="pOnFailure">Callback for failure.</param>
        [Obsolete("Use Track() instead.")]
        public static void PublishEvent(AppEvent pEvent, Action pOnSuccess, Action<string> pOnFailure)
        {
            WaitForInitialization(() => Agent.PublishEvent(pEvent, pOnSuccess, pOnFailure));
        }

        /// <summary>
        /// Track an event the user caused to the backend. Events are sent in batches to the backend and
        /// persisted to disk until they have successfully been sent.
        /// </summary>
        /// <param name="pEvent">The event you want to track.</param>
        /// <param name="pOnSuccess">Callback invoked when the event is successfully tracked.</param>
        /// <param name="pOnFailure">Callback invoked if the event could not be tracked.</param>
        public static void Track(AppEvent pEvent, Action pOnSuccess, Action<string> pOnFailure)
        {
            WaitForInitialization(() => Agent.PublishEvent(pEvent, pOnSuccess, pOnFailure));
        }

        /// <summary>
        /// Track an event the user caused to the backend. Events are sent in batches to the backend and
        /// persisted to disk until they have successfully been sent.
        /// </summary>
        /// <param name="pEvent">The event you want to track.</param>
        public static void Track(AppEvent pEvent)
        {
            WaitForInitialization(() => Agent.PublishEvent(pEvent));
        }

        /// <summary>
        /// Track an event the user caused to the backend. Events are sent in batches to the backend and
        /// persisted to disk until they have successfully been sent.
        /// </summary>
        /// <param name="pEventName">The name of the event you want to track.</param>
        /// <param name="pOnSuccess">Callback invoked when the event is successfully tracked.</param>
        /// <param name="pOnFailure">Callback invoked if the event could not be tracked.</param>
        public static void Track(string pEventName, Action pOnSuccess, Action<string> pOnFailure)
        {
            if (string.IsNullOrEmpty(pEventName))
            {
                pOnFailure?.Invoke("pEventName parameter cannot be null or empty.");
                return;
            }

            WaitForInitialization(() => Agent.PublishEvent(new AppEvent(pEventName), pOnSuccess, pOnFailure));
        }

        /// <summary>
        /// Track an event the user caused to the backend. Events are sent in batches to the backend and
        /// persisted to disk until they have successfully been sent.
        /// </summary>
        /// <param name="pEventName">The name of the event you want to track.</param>
        public static void Track(string pEventName)
        {
            if (string.IsNullOrEmpty(pEventName))
            {
                return;
            }

            WaitForInitialization(() => Agent.PublishEvent(new AppEvent(pEventName)));
        }

        /// <summary>
        /// Track an event the user caused to the backend. Events are sent in batches to the backend and
        /// persisted to disk until they have successfully been sent.
        /// </summary>
        /// <param name="pEventName">The name of the event you want to track.</param>
        /// <param name="pDimensions">The dimensions of the event you want to track.</param>
        /// <param name="pOnSuccess">Callback invoked when the event is successfully tracked.</param>
        /// <param name="pOnFailure">Callback invoked if the event could not be tracked.</param>
        public static void Track(string pEventName, Dictionary<string, string> pDimensions, Action pOnSuccess, Action<string> pOnFailure)
        {
            if (string.IsNullOrEmpty(pEventName))
            {
                pOnFailure?.Invoke("pEventName parameter cannot be null or empty.");
                return;
            }

            var appEvent = new AppEvent(pEventName);
            if (pDimensions != null)
            {
                foreach (var dimension in pDimensions)
                {
                    appEvent.AddDimension(dimension.Key, dimension.Value);
                }
            }

            WaitForInitialization(() => Agent.PublishEvent(appEvent, pOnSuccess, pOnFailure));
        }

        /// <summary>
        /// Track an event the user caused to the backend. Events are sent in batches to the backend and
        /// persisted to disk until they have successfully been sent.
        /// </summary>
        /// <param name="pEventName">The name of the event you want to track.</param>
        /// <param name="pDimensions">The dimensions of the event you want to track.</param>
        public static void Track(string pEventName, Dictionary<string, string> pDimensions)
        {
            if (string.IsNullOrEmpty(pEventName))
            {
                return;
            }

            var appEvent = new AppEvent(pEventName);
            if (pDimensions != null)
            {
                foreach (var dimension in pDimensions)
                {
                    appEvent.AddDimension(dimension.Key, dimension.Value);
                }
            }

            WaitForInitialization(() => Agent.PublishEvent(appEvent));
        }

#if !UNITY_WEBGL
        /// <summary>
        /// Get the unique id of the current install of the user.
        /// </summary>
        /// <param name="pOnSuccess">Callback invoked on success with the install instance ID.</param>
        /// <param name="pOnFailure">Callback invoked on failure with error message.</param>
        public static void GetInstallInstanceId(Action<string> pOnSuccess, Action<string> pOnFailure)
        {
            WaitForInitialization(() => Agent.GetInstallInstanceId(pOnSuccess, pOnFailure));
        }

        /// <summary>
        /// Gets the advertiser ID information.
        /// </summary>
        /// <param name="pOnSuccess">Callback for success.</param>
        /// <param name="pOnFailure">Callback for failure.</param>
        public static void GetAdvertiserIdInfo(Action<AdvertiserIdInfo> pOnSuccess, Action<string> pOnFailure)
        {
            WaitForInitialization(() => Agent.GetAdvertiserIdInfo(pOnSuccess, pOnFailure));
        }

        /// <summary>
        /// Gets the test group ID.
        /// </summary>
        /// <param name="pOnSuccess">Callback for success.</param>
        /// <param name="pOnFailure">Callback for failure.</param>
        public static void GetTestGroupId(Action<int?> pOnSuccess, Action<string> pOnFailure)
        {
            WaitForInitialization(() => Agent.GetTestGroupId(pOnSuccess, pOnFailure));
        }
#endif

#if UNITY_IOS
        /// <summary>
        /// Request tracking authorization. If not supported on this device, checks if ad tracking is limited.
        /// If authorization was already permitted or denied, the callback is invoked immediately.
        /// Otherwise it prompts the user for authorization and returns whether the user allowed tracking.
        /// </summary>
        /// <param name="pOnAuthorized">Callback that receives whether tracking is authorized.</param>
        public static void RequestTrackingAuthorization(Action<bool> pOnAuthorized) {
#if UNITY_EDITOR
                // assume we always get the autorization when running inside the editor
                pOnAuthorized(true);
#else
                JustTrackSDKNativeBridgeUnity.Instance.RequestTrackingAuthorization(pOnAuthorized);
#endif
        }
#endif

        /// <summary>
        /// Determine if we are allowed to access the IDFA/GAID. On iOS, this calls RequestTrackingAuthorization
        /// if the justtrack SDK was configured to request the tracking permission on behalf of the developer.
        /// If this is not the case (or we are on Android), this method determines if we are allowed to access
        /// the advertiser id and returns this information in the callback.
        /// </summary>
        /// <param name="pOnAuthorized">Callback that receives whether tracking is authorized.</param>
        public static void OnTrackingAuthorization(Action<bool> pOnAuthorized)
        {
#if UNITY_IOS
                var settings = JustTrackSettings.LoadFromResources();
                if (settings.IosTrackingSettings.RequestTrackingPermission) {
                    // if we should request it, we can just make use of the logic there to handle the case where we
                    // already have the permission
                    RequestTrackingAuthorization(pOnAuthorized);
                    return;
                }
#endif
#if !UNITY_WEBGL
            GetAdvertiserIdInfo(
                (info) =>
            {
                pOnAuthorized(!info.IsLimitedAdTracking);
            }, (error) =>
            {
                // if we can't read the advertiser id, it is not available
                pOnAuthorized(false);
            });
#else
            // WebGL does not yet support advertiser id tracking
            pOnAuthorized(false);
#endif
        }

        /// <summary>
        /// Retrieve the current SDK version.
        /// </summary>
        /// <returns>The SDK version string.</returns>
        public static string GetVersion()
        {
#if UNITY_WEBGL
            return BuildConfig.SdkVersionWebGl;
#else
            return BuildConfig.SdkVersion;
#endif
        }

#if !UNITY_WEBGL

        /// <summary>
        /// Gets the Remote Config interface.
        /// </summary>
        /// <returns>The Remote Config interface, or null if the SDK has not been initialized.</returns>
        public static IRemoteConfig? GetRemoteConfig()
        {
            return remoteConfig;
        }

        /// <summary>
        /// Use this method to share the information about the test group assigned to the user.
        /// </summary>
        /// <param name="experiment">The name of the A/B test. Must be shorter than 255 characters and consist only of printable ASCII characters (U+0020 to U+007E).</param>
        /// <param name="variant">The test group the user is assigned to. Must be shorter than 255 characters and consist only of printable ASCII characters (U+0020 to U+007E).</param>
        /// <param name="tags">You can add tags to your experiments which can describe the purpose. You can add a maximum of 5 tags.</param>
        /// <param name="happenedAt">The time at which the user was assigned to the test group. Defaults to the time at which the server received the assignment request if not provided.</param>
        /// <param name="pOnSuccess">Callback for success.</param>
        /// <param name="pOnFailure">Callback for failure.</param>
        public static void SetExperimentVariant(string experiment, string variant, string[]? tags, DateTime? happenedAt, Action pOnSuccess, Action<string> pOnFailure)
        {
            WaitForInitialization(() => Agent.SetExperimentVariant(experiment, variant, tags, happenedAt, pOnSuccess, pOnFailure));
        }
#endif

#if UNITY_IOS
        /// <summary>
        /// Forwards StoreKit 2 transaction id, product id and quantity on in-app purchases and subscriptions from your app to the justtrack backend.
        /// Available starting with iOS 15.
        /// </summary>
        /// <param name="transactionId">The transaction ID.</param>
        /// <param name="productId">The product ID.</param>
        /// <param name="quantity">The quantity purchased.</param>
        public static void ForwardTransactionId(string transactionId, string productId, int quantity) {
            WaitForInitialization(() => Agent.ForwardTransactionId(transactionId, productId, quantity));
        }

        /// <summary>
        /// Returns the current tracking authorization status.
        /// </summary>
        /// <returns>The current tracking authorization status.</returns>
        public static AttAuthorizationStatus GetTrackingAuthorizationStatus()
        {
            return Agent.GetTrackingAuthorizationStatus();
        }
#endif

#if UNITY_ANDROID
        /// <summary>
        /// Forwards IAP transaction of in-app purchases and subscriptions from your app to the justtrack backend.
        /// </summary>
        /// <param name="token">The transaction token.</param>
        /// <param name="productId">The product ID.</param>
        /// <param name="money">The amount of revenue generated.</param>
        /// <param name="productType">Type of product INAPP or SUBS</param>
        public static void ForwardTransaction(string token, string productId, Money money, ProductType productType) {
            WaitForInitialization(() => Agent.ForwardTransaction(token, productId, money, productType));
        }
#endif

        private JustTrackSDK()
        {
        }
    }
}