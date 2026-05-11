using System;
using UnityEngine;

namespace JustTrack
{
    /// <summary>
    /// Represents an experiment assignment.
    /// </summary>
#pragma warning disable SA1401 // Fields should be private (public API)
    public class Assignment
    {
        /// <summary>
        /// The config key associated with this assignment.
        /// </summary>
        public string ConfigKey;

        /// <summary>
        /// The raw config value for this assignment.
        /// </summary>
        public string ConfigValue;

        /// <summary>
        /// The unique identifier of the experiment (UUID format).
        /// </summary>
        public string ExperimentId;

        /// <summary>
        /// Whether this assignment is pending activation.
        /// </summary>
        public bool IsPending;

        /// <summary>
        /// Initializes a new instance of the <see cref="Assignment"/> class.
        /// Constructor for Assignment.
        /// </summary>
        /// <param name="configKey">The config key.</param>
        /// <param name="configValue">The config value.</param>
        /// <param name="experimentId">The experiment ID.</param>
        /// <param name="isPending">Whether the assignment is pending activation.</param>
        public Assignment(string configKey, string configValue, string experimentId, bool isPending)
        {
            this.ConfigKey = configKey;
            this.ConfigValue = configValue;
            this.ExperimentId = experimentId;
            this.IsPending = isPending;
        }

#if UNITY_ANDROID
        internal static Assignment? FromAndroidObject(AndroidJavaObject pObject)
        {
            if (pObject == null)
            {
                return null;
            }
            try
            {
                return new Assignment(
                    pObject.Call<string>("getConfigKey"),
                    pObject.Call<string>("getConfigValue"),
                    pObject.Call<string>("getExperimentId"),
                    pObject.Call<bool>("isPending")
                );
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create Assignment from Android object.", ex);
            }
        }
#endif

#if UNITY_IOS
        [Serializable]
        private class AssignmentDto
        {
            public string ConfigKey = "";
            public string ConfigValue = "";
            public string ExperimentId = "";
            public bool IsPending = false;
        }


        internal static Assignment? FromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            try
            {
                AssignmentDto parsed = JsonUtility.FromJson<AssignmentDto>(json);
                if (parsed == null)
                {
                    throw new InvalidOperationException("Failed to create Assignment from JSON.");
                }

                return new Assignment(parsed.ConfigKey ?? "", parsed.ConfigValue ?? "", parsed.ExperimentId ?? "", parsed.IsPending);
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new InvalidOperationException("Failed to create Assignment from JSON.", ex);
            }
        }
#endif
    }
#pragma warning restore SA1401
}
