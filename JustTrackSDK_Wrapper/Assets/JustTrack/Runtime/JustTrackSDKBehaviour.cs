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
            if (settings.UseRuntimeConstructor)
            {
                return;
            }

            Init(settings);
        }

        private void Init(JustTrackSettings settings)
        {
#if UNITY_WEBGL
            var apiToken = settings.WebglApiToken;
            var automaticInAppPurchaseTracking = false;
            var manualStart = settings.ManualStart;
            var enableLogging = settings.EnableConsoleLogging;
            string? bundleId = null;
            string? appVersion = null;
            string? appCode = null;
#elif UNITY_IOS
            var apiToken = settings.IosApiToken;
            var automaticInAppPurchaseTracking = !settings.IosDisableAutomaticInAppPurchaseTracking;
            var manualStart = settings.ManualStart;
            var enableLogging = settings.EnableConsoleLogging;
            string? bundleId = settings.IosBundleId;
            string? appVersion = settings.IosAppVersion;
            string? appCode = settings.IosAppCode;
#else
            var apiToken = settings.AndroidApiToken;
            var automaticInAppPurchaseTracking = !settings.AndroidDisableAutomaticInAppPurchaseTracking;
            var manualStart = settings.ManualStart;
            var enableLogging = settings.EnableConsoleLogging;
            string? bundleId = settings.AndroidBundleId;
            string? appVersion = settings.AndroidAppVersion;
            string? appCode = settings.AndroidAppCode;
#endif
            string trackingId = string.Empty;
            string trackingProvider = string.Empty;

            var enableConnectionTracking = settings.EnableConnectionTracking;

            // Only forward a custom application version when both the version name and version code are
            // configured. Otherwise fall back to the values provided by the platform (null).
            ApplicationVersion? applicationVersion = null;
            if (!string.IsNullOrEmpty(appVersion) && !string.IsNullOrEmpty(appCode))
            {
                applicationVersion = new ApplicationVersion(appVersion, appCode);
            }

            JustTrackSDK.Init(
                apiToken,
                trackingId,
                trackingProvider,
                null,
                automaticInAppPurchaseTracking,
                manualStart,
                enableLogging,
                enableConnectionTracking,
                settings.ServerUrl,
                bundleId,
                applicationVersion);
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

#if UNITY_WEBGL
        private void OnWebGLInitCallback(string responseJson)
        {
            SDKWebGLAgent.INSTANCE.OnWebGLInitCallback(responseJson);
        }

        private void OnWebGLGetAttributionCallback(string responseJson)
        {
            SDKWebGLAgent.INSTANCE.OnWebGLGetAttributionCallback(responseJson);
        }

        private void OnWebGLAnonymizeCallback(string responseJson)
        {
            SDKWebGLAgent.INSTANCE.OnWebGLAnonymizeCallback(responseJson);
        }

        private void OnWebGLEventCallback(string callbackData)
        {
            SDKWebGLAgent.INSTANCE.OnWebGLEventCallback(callbackData);
        }
#endif
    }
}
