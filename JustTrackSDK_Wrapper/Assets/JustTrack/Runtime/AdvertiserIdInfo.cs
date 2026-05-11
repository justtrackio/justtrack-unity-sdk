namespace JustTrack
{
    /// <summary>
    /// The result of reading an advertiser id can be one of three possibilities:
    /// <list type="bullet">
    /// <item><description>The advertiser id was successfully read</description></item>
    /// <item><description>The user limited ad tracking and the OS enforces this
    /// (late 2021: Android 12, early 2022: all Android devices; iOS &lt;14: The user opted out, iOS 14+: the user didn't opt in)</description></item>
    /// <item><description>Reading the advertiser id failed</description></item>
    /// </list>
    ///
    /// In the first case there will be an advertiser id available. In the second case, the id will be null
    /// and IsLimitedAdTracking will be true. In the last case the advertiser id will also be
    /// null, but ad tracking will not be reported as limited.
    /// </summary>
    public class AdvertiserIdInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdvertiserIdInfo"/> class.
        /// </summary>
        /// <param name="pAdvertiserId">The advertiser ID, or null if unavailable.</param>
        /// <param name="pIsLimitedAdTracking">Whether ad tracking is limited by the user.</param>
        internal AdvertiserIdInfo(string? pAdvertiserId, bool pIsLimitedAdTracking)
        {
            this.AdvertiserId = pAdvertiserId;
            this.IsLimitedAdTracking = pIsLimitedAdTracking;
        }

        /// <summary>
        /// Gets contains the advertiser id of the user or null if it could not be read (user limited tracking
        /// and the OS enforces it or an error occurred).
        /// </summary>
        public string? AdvertiserId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether did the user limit ad tracking? This can also be reported as true while the advertiser id is
        /// available. In that case the OS does not enforce the limit yet.
        /// </summary>
        public bool IsLimitedAdTracking { get; private set; }
    }
}