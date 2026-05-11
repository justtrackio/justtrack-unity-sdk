using System;

namespace JustTrack
{
#if !UNITY_WEBGL
    /// <summary>
    /// Interface for Remote Config functionalities.
    /// </summary>
    public interface IRemoteConfig
    {
        /// <summary>
        /// Fetches remote config values from the server if the minimum fetch interval has elapsed.
        /// </summary>
        /// <param name="pOnSuccess">Callback invoked on success.</param>
        /// <param name="pOnFailure">Callback invoked on failure.</param>
        void Fetch(Action pOnSuccess, Action<string> pOnFailure);

        /// <summary>
        /// Activates experiment assignments by confirming enrollment with the server.
        /// </summary>
        /// <param name="experiments">Array of experiment ids to activate.</param>
        /// <param name="pOnSuccess">Callback invoked on success.</param>
        /// <param name="pOnFailure">Callback invoked on failure.</param>
        void Activate(string[] experiments, Action pOnSuccess, Action<string> pOnFailure);

        /// <summary>
        /// A convenience method that fetches and then activates all pending experiments in one call.
        /// </summary>
        /// <param name="pOnSuccess">Callback invoked on success.</param>
        /// <param name="pOnFailure">Callback invoked on failure.</param>
        void FetchAndActivate(Action pOnSuccess, Action<string> pOnFailure);

        /// <summary>
        /// Sets the Remote Config settings.
        /// </summary>
        /// <param name="settings">The settings to apply.</param>
        void SetConfig(JusttrackRemoteConfigSettings settings);

        /// <summary>
        /// Gets all current experiment assignments.
        /// </summary>
        /// <returns>Array of current experiment assignments.</returns>
        public Assignment[] GetAll();

        /// <summary>
        /// Gets a boolean value from Remote Config.
        /// </summary>
        /// <param name="configKey">The key of the config value.</param>
        /// <returns>Config value as Boolean. If the config value is not a valid Boolean or is not found, null is returned.</returns>
        public bool? GetBoolean(string configKey);

        /// <summary>
        /// Gets a double value from Remote Config.
        /// </summary>
        /// <param name="configKey">The key of the config value.</param>
        /// <returns>Config value as Double. If the config value is not a valid Double or is not found, null is returned.</returns>
        public double? GetDouble(string configKey);

        /// <summary>
        /// Gets an integer value from Remote Config.
        /// </summary>
        /// <param name="configKey">The key of the config value.</param>
        /// <returns>Config value as Integer. If the config value is not a valid Integer or is not found, null is returned.</returns>
        public int? GetInt(string configKey);

        /// <summary>
        /// Gets a long value from Remote Config.
        /// </summary>
        /// <param name="configKey">The key of the config value.</param>
        /// <returns>Config value as Long. If the config value is not a valid Long or is not found, null is returned.</returns>
        public long? GetLong(string configKey);

        /// <summary>
        /// Gets a string value from Remote Config.
        /// </summary>
        /// <param name="configKey">The key of the config value.</param>
        /// <returns>Config value as String. If the config value is not a valid String or is not found, null is returned.</returns>
        public string? GetString(string configKey);
    }
#endif
}
