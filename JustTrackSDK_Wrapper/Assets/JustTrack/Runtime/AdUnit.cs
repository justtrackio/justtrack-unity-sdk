namespace JustTrack
{
    /// <summary>
    /// Defines the different types of ad units supported by the SDK.
    /// </summary>
    public enum AdUnit
    {
        /// <summary>
        /// Banner ad unit.
        /// </summary>
        Banner,

        /// <summary>
        /// Interstitial ad unit.
        /// </summary>
        Interstitial,

        /// <summary>
        /// Rewarded ad unit.
        /// </summary>
        Rewarded,

        /// <summary>
        /// Rewarded interstitial ad unit.
        /// </summary>
        RewardedInterstitial,

        /// <summary>
        /// Native ad unit.
        /// </summary>
        Native,

        /// <summary>
        /// App open ad unit.
        /// </summary>
        AppOpen,
    }

    /// <summary>
    /// Internal utility class for converting AdUnit enum values to string representations.
    /// </summary>
    internal static class AdUnitInternalConversation
    {
        /// <summary>
        /// Converts an AdUnit enum value to its internal string representation.
        /// </summary>
        /// <param name="adFormat">The ad unit to convert.</param>
        /// <returns>The string representation of the ad unit.</returns>
        internal static string ToInternalString(AdUnit adFormat)
        {
            switch (adFormat)
            {
                case AdUnit.Banner:
                    return "banner";
                case AdUnit.Interstitial:
                    return "interstitial";
                case AdUnit.Rewarded:
                    return "rewarded";
                case AdUnit.RewardedInterstitial:
                    return "rewarded_interstitial";
                case AdUnit.Native:
                    return "native";
                case AdUnit.AppOpen:
                    return "app_open";
                default:
                    return string.Empty;
            }
        }
    }

    /// <summary>
    /// Internal utility class for converting IronSource ad format strings to AdUnit enum values.
    /// </summary>
    internal static class AdUnitFromIronsourceConversion
    {
        /// <summary>
        /// Converts an IronSource ad format string to an AdUnit enum value.
        /// </summary>
        /// <param name="adFormat">The IronSource ad format string.</param>
        /// <returns>The corresponding AdUnit enum value, or null if not found.</returns>
        internal static AdUnit? ToAdUnit(string adFormat)
        {
            switch (adFormat)
            {
                case "banner":
                    return AdUnit.Banner;
                case "interstitial":
                    return AdUnit.Interstitial;
                case "rewarded_video":
                    return AdUnit.Rewarded;
            }

            return null;
        }
    }
}
