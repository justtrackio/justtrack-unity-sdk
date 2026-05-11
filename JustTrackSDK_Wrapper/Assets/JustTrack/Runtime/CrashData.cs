using System;

namespace JustTrack
{
#pragma warning disable SA1401, SA1307 // Fields should be private; field names should begin with upper-case letter (required for JSON serialization)
    /// <summary>
    /// Represents crash data information for the justtrack SDK.
    /// </summary>
    [Serializable]
    internal class CrashData
    {
        /// <summary>
        /// Gets or sets the timestamp when the crash occurred.
        /// </summary>
        public string timestamp = string.Empty;

        /// <summary>
        /// Gets or sets the stack trace of the crash.
        /// </summary>
        public string stacktrace = string.Empty;

        /// <summary>
        /// Gets or sets the name of the crash.
        /// </summary>
        public string name = string.Empty;

        /// <summary>
        /// Gets or sets the reason for the crash.
        /// </summary>
        public string reason = string.Empty;

        /// <summary>
        /// Gets or sets the type of crash.
        /// </summary>
        public int crashType;
    }

    /// <summary>
    /// Wrapper class for crash data used by the justtrack SDK.
    /// </summary>
    [Serializable]
    internal class CrashWrapper
    {
        /// <summary>
        /// Gets or sets the crash data.
        /// </summary>
        public CrashData data = new CrashData();
    }
#pragma warning restore SA1401, SA1307
}