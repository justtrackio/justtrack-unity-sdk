namespace JustTrack
{
    /// <summary>
    /// Defines integration types for AppLovin.
    /// </summary>
    public enum AppLovinIntegrationType
    {
        /// <summary>
        /// Listen for specific ad events and forward ad revenue to the justtrack backend.
        /// Required if you want to forward any user id to AppLovin.
        /// </summary>
        MaxIntegration,

        /// <summary>
        /// Listen for the AppLovin ad activity being visible and forward ad impressions to the justtrack backend.
        /// </summary>
        SimpleIntegration,
    }
}