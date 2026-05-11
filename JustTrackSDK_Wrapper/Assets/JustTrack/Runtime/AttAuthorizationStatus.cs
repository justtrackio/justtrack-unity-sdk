namespace JustTrack
{
    /// <summary>
    /// Represents the App Tracking Transparency authorization status on iOS.
    /// </summary>
    public enum AttAuthorizationStatus
    {
        /// <summary>
        /// The user has not yet received an authorization request to authorize access to app-related data.
        /// </summary>
        NotDetermined = 0,

        /// <summary>
        /// Authorization to access app-related data is restricted.
        /// </summary>
        Restricted = 1,

        /// <summary>
        /// The user denied authorization to access app-related data.
        /// </summary>
        Denied = 2,

        /// <summary>
        /// The user authorized access to app-related data.
        /// </summary>
        Authorized = 3,

        /// <summary>
        /// The authorization status is not available (e.g., on non-iOS platforms).
        /// </summary>
        NotAvailable = 4,
    }
}