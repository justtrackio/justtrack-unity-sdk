using UnityEngine;
using UnityEngine.Serialization;

namespace JustTrack
{
    /// <summary>
    /// Provides build settings configuration for the justtrack SDK.
    /// </summary>
#pragma warning disable SA1401 // Fields should be private (required for Unity serialization)
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

        /// <summary>
        /// Gets or sets a value indicating whether validation errors are allowed on build.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("allowValidationErrorsOnBuild")]
        public bool AllowValidationErrorsOnBuild;
    }
#pragma warning restore SA1401
}
