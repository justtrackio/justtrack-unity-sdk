using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace JustTrack
{
    /// <summary>
    /// Defines the integration mode for Facebook Audience Network.
    /// </summary>
    public enum FacebookAudienceNetworkIntegration
    {
        /// <summary>
        /// No integration with Facebook Audience Network.
        /// </summary>
        NoIntegration = 0,

        /// <summary>
        /// Unity-based integration with Facebook Audience Network.
        /// </summary>
        UnityIntegration = 1,

        /// <summary>
        /// Native platform integration with Facebook Audience Network.
        /// </summary>
        NativeIntegration = 2,
    }

    /// <summary>
    /// Configuration settings for iOS tracking permissions and integrations.
    /// </summary>
    [Serializable]
#pragma warning disable SA1401 // Fields should be private (required for Unity serialization)
#pragma warning disable SA1300 // Element should begin with an uppercase letter (public API)
    public struct iOSTrackingSettings
#pragma warning restore SA1300
    {
        /// <summary>
        /// Whether to request tracking permission from the user on iOS.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("requestTrackingPermission")]
        public bool RequestTrackingPermission;

        /// <summary>
        /// Whether to use a custom advertising attribution report endpoint.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("useCustomAdvertisingAttributionReportEndpoint")]
        public bool UseCustomAdvertisingAttributionReportEndpoint;

        /// <summary>
        /// The description text shown to users when requesting tracking permission.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("trackingPermissionDescription")]
        public string TrackingPermissionDescription;

        /// <summary>
        /// Whether the tracking permission description is managed automatically.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("trackingPermissionDescriptionManaged")]
        public bool TrackingPermissionDescriptionManaged;

        /// <summary>
        /// The Facebook Audience Network integration mode for iOS.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("facebookAudienceNetworkIntegration")]
        public FacebookAudienceNetworkIntegration FacebookAudienceNetworkIntegration;
    }

    /// <summary>
    /// ScriptableObject containing justtrack SDK configuration settings.
    /// </summary>
    public class JustTrackSettings : ScriptableObject
    {
        /// <summary>
        /// The directory path where JustTrack settings are stored.
        /// </summary>
        public const string JustTrackSettingsDirectory = "Assets/JustTrack/Resources";

        /// <summary>
        /// The resource name for JustTrack settings.
        /// </summary>
        public const string JustTrackSettingsResource = "JustTrackSettings";

        /// <summary>
        /// The full path to the JustTrack settings asset file.
        /// </summary>
        public const string JustTrackSettingsPath = JustTrackSettingsDirectory + "/" + JustTrackSettingsResource + ".asset";

        /// <summary>
        /// Whether the SDK should be started manually instead of automatically.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("manualStart")]
        public bool ManualStart;

        /// <summary>
        /// Whether to always update injected code during builds.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("alwaysUpdateInjectedCode")]
        public bool AlwaysUpdateInjectedCode;

        /// <summary>
        /// Whether to ignore Firebase integration.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("ignoreFirebaseIntegration")]
        public bool IgnoreFirebaseIntegration;

        /// <summary>
        /// The API token for Android platform integration.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("androidApiToken")]
        public string AndroidApiToken = string.Empty;

        /// <summary>
        /// The API token for iOS platform integration.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("iosApiToken")]
        public string IosApiToken = string.Empty;

        /// <summary>
        /// The API token for WebGL platform integration.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("webglApiToken")]
        public string WebglApiToken = string.Empty;

        /// <summary>
        /// The bundle ID for WebGL platform integration.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("webglBundleId")]
        public string WebglBundleId = string.Empty;

        /// <summary>
        /// The app version for WebGL platform integration.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("webglAppVersion")]
        public string WebglAppVersion = string.Empty;

        /// <summary>
        /// The app code for WebGL platform integration.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("webglAppCode")]
        public string WebglAppCode = string.Empty;

        /// <summary>
        /// The user ID for WebGL platform integration.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("webglUserId")]
        public string WebglUserId = string.Empty;

        /// <summary>
        /// Whether AppLovin integration is enabled for Android.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("androidAppLovinIntegration")]
        public bool AndroidAppLovinIntegration;

        /// <summary>
        /// Whether AppLovin integration is enabled for iOS.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("iosAppLovinIntegration")]
        public bool IosAppLovinIntegration;

        /// <summary>
        /// Whether Unity Ads integration is enabled for Android.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("androidUnityAdsIntegration")]
        public bool AndroidUnityAdsIntegration;

        /// <summary>
        /// Whether Unity Ads integration is enabled for iOS.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("iosUnityAdsIntegration")]
        public bool IosUnityAdsIntegration;

        /// <summary>
        /// Whether IronSource integration is enabled for Android.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("androidIronSourceIntegration")]
        public bool AndroidIronSourceIntegration;

        /// <summary>
        /// Whether IronSource integration is enabled for iOS.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("iosIronSourceIntegration")]
        public bool IosIronSourceIntegration;

        /// <summary>
        /// iOS-specific tracking configuration settings.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("iosTrackingSettings")]
        public iOSTrackingSettings IosTrackingSettings;

        /// <summary>
        /// Whether automatic in-app purchase tracking is disabled for iOS.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("iosDisableAutomaticInAppPurchaseTracking")]
        public bool IosDisableAutomaticInAppPurchaseTracking;

        /// <summary>
        /// Whether automatic in-app purchase tracking is disabled for Android.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("androidDisableAutomaticInAppPurchaseTracking")]
        public bool AndroidDisableAutomaticInAppPurchaseTracking;

        /// <summary>
        /// Whether Firebase integration is enabled for Android.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("androidFirebaseIntegration")]
        public bool AndroidFirebaseIntegration;

        /// <summary>
        /// Whether Firebase integration is enabled for iOS.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("iosFirebaseIntegration")]
        public bool IosFirebaseIntegration;

        /// <summary>
        /// Whether Google ODM integration is enabled for iOS.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("iosGoogleOdmIntegration")]
        public bool IosGoogleOdmIntegration;

        /// <summary>
        /// Custom bundle ID for Android platform integration.
        /// If empty, the default bundle ID from PlayerSettings will be used.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("androidBundleId")]
        public string AndroidBundleId = string.Empty;

        /// <summary>
        /// Custom app version name for Android platform integration (e.g., "1.0.0").
        /// If empty, the default version from PlayerSettings will be used.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("androidAppVersion")]
        public string AndroidAppVersion = string.Empty;

        /// <summary>
        /// Custom app version code for Android platform integration (e.g., "1").
        /// If empty, the default version code from PlayerSettings will be used.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("androidAppCode")]
        public string AndroidAppCode = string.Empty;

        /// <summary>
        /// Custom bundle ID for iOS platform integration.
        /// If empty, the default bundle ID from PlayerSettings will be used.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("iosBundleId")]
        public string IosBundleId = string.Empty;

        /// <summary>
        /// Custom app version name for iOS platform integration (e.g., "1.0.0").
        /// If empty, the default version from PlayerSettings will be used.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("iosAppVersion")]
        public string IosAppVersion = string.Empty;

        /// <summary>
        /// Custom app version code for iOS platform integration (e.g., "1").
        /// If empty, the default build number from PlayerSettings will be used.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("iosAppCode")]
        public string IosAppCode = string.Empty;

        /// <summary>
        /// Custom server URL for the SDK.
        /// Leave empty to use the default production server.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("serverUrl")]
        public string ServerUrl = string.Empty;

        /// <summary>
        /// Whether debug mode is enabled for the SDK.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("enableDebugMode")]
        public bool EnableDebugMode;

        /// <summary>
        /// Whether console logging is enabled for the SDK.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("enableConsoleLogging")]
        public bool EnableConsoleLogging;

        /// <summary>
        /// Loads JustTrack settings from Unity Resources.
        /// </summary>
        /// <returns>The loaded JustTrack settings instance.</returns>
        internal static JustTrackSettings LoadFromResources()
        {
            return Resources.Load<JustTrackSettings>(JustTrackSettings.JustTrackSettingsResource);
        }
    }
#pragma warning restore SA1401
}
