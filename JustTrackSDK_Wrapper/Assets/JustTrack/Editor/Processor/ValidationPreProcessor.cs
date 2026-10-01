using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace JustTrack
{
    /// <summary>
    /// A Unity build preprocessor that validates the justtrack SDK configuration before building.
    /// </summary>
    public class ValidationPreProcessor : IPreprocessBuildWithReport
    {
        /// <summary>
        /// Gets the callback order for this build preprocessor.
        /// </summary>
#pragma warning disable SA1300 // Element should begin with an uppercase letter (interface requirement)
        public int callbackOrder
#pragma warning restore SA1300
        {
            get { return 0; }
        }

        /// <summary>
        /// Called before the build process starts to validate the justtrack SDK configuration.
        /// </summary>
        /// <param name="report">The build report containing build information.</param>
        public void OnPreprocessBuild(BuildReport report)
        {
            JustTrackUtils.ValidationResult validateResult;
            validateResult.Warnings = new List<string>();
            validateResult.Errors = new List<string>();
            var settings = JustTrackUtils.GetSettings();
            if (settings == null)
            {
                throw new BuildFailedException("justtrack SDK configuration was not found");
            }

            // When the runtime constructor is used, the SDK is not configured via the settings asset,
            // so there is nothing to validate at build time.
            if (settings.UseRuntimeConstructor)
            {
                return;
            }

            JustTrackUtils.ValidationMode mode = JustTrackUtils.ValidationMode.ValidateAll;
            var buildTarget = report.summary.platform;
            if (buildTarget == BuildTarget.Android)
            {
                mode = JustTrackUtils.ValidationMode.ValidateAndroid;
            }
            else if (buildTarget == BuildTarget.iOS)
            {
                mode = JustTrackUtils.ValidationMode.ValidateIOS;
            }

            var steps = JustTrackUtils.ValidateAsync(settings, mode, (result) =>
            {
                validateResult = result;
            });
            while (steps.MoveNext())
            {
                // wait
            }

            foreach (string error in validateResult.Errors)
            {
                Debug.LogError(error);
            }

            foreach (string warning in validateResult.Warnings)
            {
                Debug.LogWarning(warning);
            }

            if (validateResult.Errors.Count > 0)
            {
                throw new BuildFailedException("justtrack SDK configuration is not valid");
            }
        }
    }
}
