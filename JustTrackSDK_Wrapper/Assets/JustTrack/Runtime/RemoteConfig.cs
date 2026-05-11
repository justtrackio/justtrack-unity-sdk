using System;

namespace JustTrack
{
#if !UNITY_WEBGL
    /// <summary>
    /// Implementation of IRemoteConfig interface.
    /// </summary>
    internal class RemoteConfig : IRemoteConfig
    {
        private readonly ISDKAgent agent;
        private readonly Action<Action> waitForInitialization;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteConfig"/> class.
        /// </summary>
        /// <param name="agent">The SDK agent instance.</param>
        /// <param name="waitForInitialization">Action to wait for SDK initialization before executing callbacks.</param>
        internal RemoteConfig(ISDKAgent agent, Action<Action> waitForInitialization)
        {
            this.agent = agent;
            this.waitForInitialization = waitForInitialization;
        }

        /// <summary>
        /// Fetches remote config values from the server if the minimum fetch interval has elapsed.
        /// </summary>
        /// <param name="pOnSuccess">Callback invoked on successful fetch.</param>
        /// <param name="pOnFailure">Callback invoked on fetch failure.</param>
        public void Fetch(Action pOnSuccess, Action<string> pOnFailure)
        {
            waitForInitialization(() => this.agent.FetchRemoteConfig(pOnSuccess, pOnFailure));
        }

        /// <summary>
        /// Activates experiment assignments by confirming enrollment with the server.
        /// </summary>
        /// <param name="experiments">The list of experiments to activate.</param>
        /// <param name="pOnSuccess">Callback invoked on successful activation.</param>
        /// <param name="pOnFailure">Callback invoked on activation failure.</param>
        public void Activate(string[] experiments, Action pOnSuccess, Action<string> pOnFailure)
        {
            waitForInitialization(() => this.agent.ActivateRemoteConfig(experiments, pOnSuccess, pOnFailure));
        }

        /// <summary>
        /// A convenience method that fetches and then activates all pending experiments in one call.
        /// </summary>
        /// <param name="pOnSuccess">Callback invoked on successful fetch and activation.</param>
        /// <param name="pOnFailure">Callback invoked on fetch or activation failure.</param>
        public void FetchAndActivate(Action pOnSuccess, Action<string> pOnFailure)
        {
            waitForInitialization(() => this.agent.FetchAndActivateRemoteConfig(pOnSuccess, pOnFailure));
        }

        /// <summary>
        /// Sets the Remote Config settings.
        /// </summary>
        /// <param name="settings">The settings to apply.</param>
        public void SetConfig(JusttrackRemoteConfigSettings settings)
        {
            waitForInitialization(() => this.agent.SetRemoteConfigSettings(settings));
        }

        /// <summary>
        /// Gets all current experiment assignments.
        /// </summary>
        /// <returns>An array of all current experiment assignments.</returns>
        public Assignment[] GetAll()
        {
            if (!agent.IsInitialized())
            {
                return new Assignment[0];
            }

            return this.agent.GetAllAssignments();
        }

        /// <summary>
        /// Gets a boolean value from Remote Config.
        /// </summary>
        /// <param name="configKey">The key of the config value.</param>
        /// <returns>Config value as Boolean. If the config value is not a valid Boolean or is not found, null is returned.</returns>
        public bool? GetBoolean(string configKey)
        {
            if (!agent.IsInitialized())
            {
                return null;
            }

            return this.agent.GetRemoteConfigBoolean(configKey);
        }

        /// <summary>
        /// Gets a double value from Remote Config.
        /// </summary>
        /// <param name="configKey">The key of the config value.</param>
        /// <returns>Config value as Double. If the config value is not a valid Double or is not found, null is returned.</returns>
        public double? GetDouble(string configKey)
        {
            if (!agent.IsInitialized())
            {
                return null;
            }

            return this.agent.GetRemoteConfigDouble(configKey);
        }

        /// <summary>
        /// Gets an integer value from Remote Config.
        /// </summary>
        /// <param name="configKey">The key of the config value.</param>
        /// <returns>Config value as Integer. If the config value is not a valid Integer or is not found, null is returned.</returns>
        public int? GetInt(string configKey)
        {
            if (!agent.IsInitialized())
            {
                return null;
            }

            return this.agent.GetRemoteConfigInt(configKey);
        }

        /// <summary>
        /// Gets a long value from Remote Config.
        /// </summary>
        /// <param name="configKey">The key of the config value.</param>
        /// <returns>Config value as Long. If the config value is not a valid Long or is not found, null is returned.</returns>
        public long? GetLong(string configKey)
        {
            if (!agent.IsInitialized())
            {
                return null;
            }

            return this.agent.GetRemoteConfigLong(configKey);
        }

        /// <summary>
        /// Gets a string value from Remote Config.
        /// </summary>
        /// <param name="configKey">The key of the config value.</param>
        /// <returns>Config value as String. If the config value is not a valid String or is not found, null is returned.</returns>
        public string? GetString(string configKey)
        {
            if (!agent.IsInitialized())
            {
                return null;
            }

            return this.agent.GetRemoteConfigString(configKey);
        }
    }
#endif
}
