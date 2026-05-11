using System;
using System.Threading;
using UnityEngine;

namespace JustTrack
{
    /// <summary>
    /// Unity MonoBehaviour component that manages the justtrack SDK lifecycle and operations.
    /// </summary>
    public class JustTrackSDKBehaviour : MonoBehaviour
    {
        // Invoked once the attribution has been retrieved from the backend if not null.
        private static Action<AttributionResponse>? onInitialized = null;

        // Invoked if an error occurs during attribution if not null.
        private static Action<string>? onError = null;

        // Stores the attribution response after the SDK has been initialized.
        private static AttributionResponse? attributionResponse = null;

        // Stores the error if getting the attribution fails.
        private static string? initError = null;

        // Lock guarding the attribution results and callbacks.
        private static readonly object SyncLock = new object();

        static JustTrackSDKBehaviour()
        {
        }

        private void Awake()
        {
            if (unityMainThreadId != -1)
            {
                // we have been initialized a second time and thus can skip initialization
                return;
            }

            // set the main thread id first - we can't do that in the constructur or similar
            // (unity might load stuff on a background thread), so doing it here is the first
            // place we can do that.
            // This also serves as a check above whether we have already created an SDK instance once
            unityMainThreadId = Thread.CurrentThread.ManagedThreadId;

            var settings = JustTrackSettings.LoadFromResources();

            if (settings == null)
            {
                Debug.LogError("There are no settings defined for the justtrack SDK, can not initialize");
                return;
            }

            gameObject.name = "JustTrackSDKBehaviour";
            if (gameObject.transform.parent != null)
            {
                Debug.Log("Detaching from parent so we can mark ourselves as DontDestroyOnLoad");
                gameObject.transform.parent = null;
            }

            DontDestroyOnLoad(gameObject);
#if UNITY_IOS
            if (settings.IosTrackingSettings.RequestTrackingPermission)
            {
                JustTrackSDK.RequestTrackingAuthorization((authorized) => { });
            }
#endif
            Init(settings);
        }

        private void Init(JustTrackSettings settings)
        {
#if UNITY_WEBGL
            var apiToken = settings.WebglApiToken;
            var automaticInAppPurchaseTracking = false; // WebGL does not support IAP tracking
            var enableDebugMode = settings.EnableDebugMode;
            var manualStart = settings.ManualStart;
            var enableLogging = settings.EnableConsoleLogging;
#elif UNITY_IOS
            var apiToken = settings.IosApiToken;
            var automaticInAppPurchaseTracking = !settings.IosDisableAutomaticInAppPurchaseTracking;
            var enableDebugMode = settings.EnableDebugMode;
            var manualStart = settings.ManualStart;
            var enableLogging = settings.EnableConsoleLogging;
#else
            var apiToken = settings.AndroidApiToken;
            var automaticInAppPurchaseTracking = !settings.AndroidDisableAutomaticInAppPurchaseTracking;
            var enableDebugMode = settings.EnableDebugMode;
            var manualStart = settings.ManualStart;
            var enableLogging = settings.EnableConsoleLogging;
#endif
            string trackingId = string.Empty;
            string trackingProvider = string.Empty;

            JustTrackSDK.Init(
                apiToken,
                trackingId,
                trackingProvider,
                null,
                automaticInAppPurchaseTracking,
                enableDebugMode,
                manualStart,
                enableLogging,
                (response) =>
            {
                lock (SyncLock)
                {
                    attributionResponse = response;
                    if (onInitialized != null)
                    {
                        var toCall = onInitialized;
                        CallOnMainThread(() =>
                        {
                            toCall(response);
                        });
                    }

                    onInitialized = null;
                    onError = null;
                }
            }, (error) =>
            {
                lock (SyncLock)
                {
                    initError = error;
                    if (onError != null)
                    {
                        var toCall = onError;
                        CallOnMainThread(() =>
                        {
                            toCall(error);
                        });
                    }

                    onInitialized = null;
                    onError = null;
                }
            });
        }

        // Retrieve the attribution produced by the SDK. If the SDK already can provide an attribution, your
        // callbacks are immediately invoked with the attribution result. Otherwise they are stored and called
        // as soon as a result is available.
        // You can call this method as many times as you want - each invocation will add your delegates to
        // the list of delegates to call as soon as the attribution is available.

        /// <summary>
        /// Gets the attribution response from the justtrack SDK.
        /// </summary>
        /// <param name="pOnInitialized">Callback invoked when attribution is successfully retrieved.</param>
        /// <param name="pOnError">Callback invoked if an error occurs during attribution retrieval.</param>
        public static void GetAttribution(Action<AttributionResponse> pOnInitialized, Action<string> pOnError)
        {
            lock (SyncLock)
            {
                if (attributionResponse != null)
                {
                    CallOnMainThread(() =>
                    {
                        pOnInitialized(attributionResponse);
                    });
                }
                else if (initError != null)
                {
                    CallOnMainThread(() =>
                    {
                        pOnError(initError);
                    });
                }
                else
                {
                    if (onInitialized == null)
                    {
                        onInitialized = pOnInitialized;
                    }
                    else
                    {
                        onInitialized += pOnInitialized;
                    }

                    if (onError == null)
                    {
                        onError = pOnError;
                    }
                    else
                    {
                        onError += pOnError;
                    }
                }
            }
        }

        private static int unityMainThreadId = -1;

        /// <summary>
        /// Checks whether the current thread is the Unity main thread.
        /// </summary>
        /// <returns>True if running on the main thread, false otherwise.</returns>
        internal static bool IsOnMainThread()
        {
            return Thread.CurrentThread.ManagedThreadId == unityMainThreadId;
        }

        private static Action? actionsOnMainThead = null;
        private static readonly object OnMainThreadLock = new object();

        /// <summary>
        /// Executes the specified action on the Unity main thread.
        /// </summary>
        /// <param name="pAction">The action to execute on the main thread.</param>
        internal static void CallOnMainThread(Action pAction)
        {
            lock (OnMainThreadLock)
            {
                if (actionsOnMainThead == null)
                {
                    actionsOnMainThead = pAction;
                }
                else
                {
                    actionsOnMainThead += pAction;
                }
            }
        }

        private void Update()
        {
            Action? action = null;
            lock (OnMainThreadLock)
            {
                action = actionsOnMainThead;
                actionsOnMainThead = null;
            }

            if (action != null)
            {
                action();
            }
        }
    }
}
