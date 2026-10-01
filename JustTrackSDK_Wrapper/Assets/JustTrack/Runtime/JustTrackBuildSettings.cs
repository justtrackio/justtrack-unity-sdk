using UnityEngine;

namespace JustTrack
{
    /// <summary>
    /// Provides build settings configuration for the justtrack SDK.
    /// </summary>
    public class JustTrackBuildSettings : ScriptableObject
    {
        /// <summary>
        /// The directory path for justtrack SDK build settings.
        /// </summary>
        public const string JustTrackBuildSettingsDirectory = JustTrackSettings.JustTrackSettingsDirectory;

        /// <summary>
        /// The resource name for justtrack SDK build settings.
        /// </summary>
        public const string JustTrackBuildSettingsResource = "JustTrackBuildSettings";

        /// <summary>
        /// The full path for justtrack SDK build settings asset.
        /// </summary>
        public const string JustTrackBuildSettingsPath = JustTrackBuildSettingsDirectory + "/" + JustTrackBuildSettingsResource + ".asset";
    }
}
