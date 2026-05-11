namespace JustTrack
{
    /// <summary>
    /// Settings for JustTrack Remote Config.
    /// </summary>
#pragma warning disable SA1401, SA1307 // Fields should be private; field names should begin with upper-case letter (public API, camelCase for compatibility)
    public class JusttrackRemoteConfigSettings
    {
        /// <summary>
        /// Minimum fetch interval in seconds.
        /// </summary>
        public long minimumFetchIntervalInSeconds = 3600;

        /// <summary>
        /// Initializes a new instance of the <see cref="JusttrackRemoteConfigSettings"/> class.
        /// </summary>
        /// <param name="interval">Minimum fetch interval in seconds.</param>
        public JusttrackRemoteConfigSettings(long interval)
        {
            this.minimumFetchIntervalInSeconds = interval;
        }
    }
#pragma warning restore SA1401, SA1307
}