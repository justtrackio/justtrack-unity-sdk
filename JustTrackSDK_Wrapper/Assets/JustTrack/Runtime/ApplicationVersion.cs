using System;

namespace JustTrack
{
    /// <summary>
    /// Wraps the application version name and version code that can be forwarded to the justtrack SDK
    /// when initializing it via the runtime constructor.
    /// <para>
    /// Both the version name and version code are always required. When no custom application version
    /// should be forwarded, pass a null <see cref="ApplicationVersion"/> reference instead of
    /// constructing an instance with empty values.
    /// </para>
    /// </summary>
    public sealed class ApplicationVersion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationVersion"/> class.
        /// </summary>
        /// <param name="version">The application version name (e.g., "1.0.0"). Must not be null or empty.</param>
        /// <param name="versionCode">The application version code (e.g., "1"). Must not be null or empty.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="version"/> or <paramref name="versionCode"/> is null or empty.
        /// </exception>
        public ApplicationVersion(string version, string versionCode)
        {
            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentException("ApplicationVersion requires a non-empty version name.", nameof(version));
            }

            if (string.IsNullOrEmpty(versionCode))
            {
                throw new ArgumentException("ApplicationVersion requires a non-empty version code.", nameof(versionCode));
            }

            Version = version;
            VersionCode = versionCode;
        }

        /// <summary>
        /// Gets the application version name (e.g., "1.0.0").
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the application version code (e.g., "1").
        /// </summary>
        public string VersionCode { get; }
    }
}
