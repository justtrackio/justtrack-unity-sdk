using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace JustTrack
{
    /// <summary>
    /// Utility class for justtrack SDK functionality.
    /// </summary>
    internal class JustTrackUtils
    {
        /// <summary>
        /// The download page URL for the External Dependency Manager.
        /// </summary>
        internal const string EdmDownloadPage = "https://developers.google.com/unity/archive#external_dependency_manager_for_unity";

        /// <summary>
        /// The direct download URL for the External Dependency Manager package.
        /// </summary>
        internal const string EdmPackageUrl = "https://github.com/googlesamples/unity-jar-resolver/raw/v1.2.183/external-dependency-manager-1.2.183.unitypackage";

        /// <summary>
        /// Gets the JustTrack settings from the asset database.
        /// </summary>
        /// <returns>The JustTrack settings or null if not found.</returns>
        internal static JustTrackSettings GetSettings()
        {
            return AssetDatabase.LoadAssetAtPath<JustTrackSettings>(JustTrackSettings.JustTrackSettingsPath);
        }

        /// <summary>
        /// Checks if validation errors are allowed during build.
        /// </summary>
        /// <returns>True if validation errors are allowed, false otherwise.</returns>
        internal static bool AreValidationErrorsAllowedOnBuild()
        {
            var settings = AssetDatabase.LoadAssetAtPath<JustTrackBuildSettings>(JustTrackBuildSettings.JustTrackBuildSettingsPath);

            return settings != null && settings.AllowValidationErrorsOnBuild;
        }

        /// <summary>
        /// Gets existing settings or creates new ones if they don't exist.
        /// </summary>
        /// <returns>The JustTrack settings or null if they couldn't be loaded.</returns>
        internal static JustTrackSettings? GetOrCreateSettings()
        {
            var settings = GetSettings();
            if (settings == null)
            {
                if (File.Exists(JustTrackSettings.JustTrackSettingsPath))
                {
                    return null;
                }

                settings = ScriptableObject.CreateInstance<JustTrackSettings>();
                settings.AndroidApiToken = string.Empty;
                settings.IosApiToken = string.Empty;
                PersistSettings(settings);

                return settings;
            }

            return settings;
        }

        private static void PersistSettings(JustTrackSettings settings)
        {
            Directory.CreateDirectory(JustTrackSettings.JustTrackSettingsDirectory);

            AssetDatabase.CreateAsset(settings, JustTrackSettings.JustTrackSettingsPath);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Gets a serialized object for the JustTrack settings.
        /// </summary>
        /// <returns>Serialized settings object or null if settings couldn't be loaded.</returns>
        internal static SerializedObject? GetSerializedSettings()
        {
            var settings = GetOrCreateSettings();
            if (settings == null)
            {
                return null;
            }

            return new SerializedObject(settings);
        }

        /// <summary>
        /// Sets the always update injected code flag in settings.
        /// </summary>
        /// <param name="value">True to always update injected code, false otherwise.</param>
        internal static void SetAlwaysUpdateInjectedCode(bool value)
        {
            var serializedObject = GetSerializedSettings();
            if (serializedObject == null)
            {
                Debug.LogError("no justtrack settings could be loaded");
                return;
            }

            SerializedProperty alwaysUpdateInjectedCode = serializedObject.FindProperty("AlwaysUpdateInjectedCode");
            alwaysUpdateInjectedCode.boolValue = value;
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Result of a validation operation.
        /// </summary>
        internal struct ValidationResult
        {
            /// <summary>
            /// List of validation warnings.
            /// </summary>
            internal List<string> Warnings;

            /// <summary>
            /// List of validation errors.
            /// </summary>
            internal List<string> Errors;
        }

        /// <summary>
        /// Validation mode for different platforms.
        /// </summary>
        internal enum ValidationMode
        {
            /// <summary>
            /// Validate Android platform only.
            /// </summary>
            ValidateAndroid,

            /// <summary>
            /// Validate iOS platform only.
            /// </summary>
            ValidateIOS,

            /// <summary>
            /// Validate all platforms.
            /// </summary>
            ValidateAll,
        }

        /// <summary>
        /// Validates JustTrack settings for all platforms.
        /// </summary>
        /// <param name="settings">The settings to validate.</param>
        /// <param name="callback">Callback to invoke with validation results.</param>
        internal static void Validate(JustTrackSettings settings, Action<ValidationResult> callback)
        {
            CoroutineRuntime.StartCoroutine(ValidateAsync(settings, ValidationMode.ValidateAll, callback));
        }

        /// <summary>
        /// Asynchronously validates JustTrack settings.
        /// </summary>
        /// <param name="settings">The settings to validate.</param>
        /// <param name="mode">The validation mode to use.</param>
        /// <param name="callback">Callback to invoke with validation results.</param>
        /// <returns>Coroutine enumerator.</returns>
        internal static IEnumerator ValidateAsync(JustTrackSettings settings, ValidationMode mode, Action<ValidationResult> callback)
        {
            ValidationResult result;
            result.Warnings = new List<string>();
            result.Errors = new List<string>();

            var androidErrors = mode == ValidationMode.ValidateIOS ? result.Warnings : result.Errors;
            var iosErrors = mode == ValidationMode.ValidateAndroid ? result.Warnings : result.Errors;

            // Use custom bundle ID if set, otherwise fall back to PlayerSettings
            string androidPackageId = !string.IsNullOrEmpty(settings.AndroidBundleId) ? settings.AndroidBundleId : PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            string iosPackageId = !string.IsNullOrEmpty(settings.IosBundleId) ? settings.IosBundleId : PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);

            foreach (var step in CoroutineRuntime.ToEnumerable(ValidateToken(androidPackageId, settings.AndroidApiToken, "Android", result.Warnings, androidErrors)))
            {
                yield return step;
            }

            foreach (var step in CoroutineRuntime.ToEnumerable(ValidateToken(iosPackageId, settings.IosApiToken, "iOS", result.Warnings, iosErrors)))
            {
                yield return step;
            }

            if (string.IsNullOrEmpty(settings.AndroidApiToken) && string.IsNullOrEmpty(settings.IosApiToken))
            {
                result.Errors.Add("Neither Android nor iOS API tokens are configured. You need to configure an API token for each platform to use the justtrack SDK");
            }

            ValidateTrackingSettings(!string.IsNullOrEmpty(settings.IosApiToken), settings.IosTrackingSettings, result.Warnings, iosErrors);

            if (!DetectExternalDependencyManager())
            {
                result.Errors.Add("The External Dependency Manager for Unity is missing. This is a required dependency of the justtrack SDK. Please go to " + EdmDownloadPage + " and download version 1.2.167 or later.");
            }

            callback(result);
            yield break;
        }

        private static IEnumerator ValidateToken(string packageId, string token, string platform, List<string> warnings, List<string> errors)
        {
            if (string.IsNullOrEmpty(token))
            {
                yield break;
            }

            var parts = token.Split('-');
            if (parts.Length != 2)
            {
                if (token.Length != 64)
                {
                    errors.Add(platform + " API token has length " + token.Length + ", expected a string of length 64");
                }
            }
            else if (parts[0] != "prod" && parts[0] != "sandbox")
            {
                errors.Add(platform + " API token starts with unknown environment '" + parts[0] + "', expected 'prod' or 'sandbox'");
            }
            else if (parts[1].Length != 64)
            {
                errors.Add(platform + " API token has " + parts[1].Length + " instead of 64 characters of token data");
            }

            foreach (var step in CoroutineRuntime.ToEnumerable(DoVerifyApiToken(packageId, token, (success, valid, errorMessage) =>
            {
                if (!success)
                {
                    warnings.Add(platform + " API token could not be verified. Are you offline?");
                }
                else if (!valid)
                {
                    string errorDetail = string.IsNullOrEmpty(errorMessage) ?
                        "Please follow the instructions at https://docs.justtrack.io/sdk/unity-sdk-readme#getting-an-api-token to obtain a valid API token" :
                        errorMessage;
                    errors.Add(platform + " API token is not valid. " + errorDetail);
                }
            })))
            {
                yield return step;
            }
        }

        private static void ValidateTrackingSettings(bool activeOnIOS, iOSTrackingSettings settings, List<string> warnings, List<string> errors)
        {
            if (!activeOnIOS && settings.RequestTrackingPermission)
            {
                errors.Add("You configured automatically requesting App Tracking Transparency opt-in, but there is no API token configured for iOS");
            }
            else if (settings.RequestTrackingPermission && string.IsNullOrEmpty(settings.TrackingPermissionDescription) && !settings.TrackingPermissionDescriptionManaged)
            {
                warnings.Add("You configured automatically requesting App Tracking Transparency opt-in, but there isn't a description configured. You have to set NSUserTrackingUsageDescription yourself.");
            }
            else if (!settings.RequestTrackingPermission && !string.IsNullOrEmpty(settings.TrackingPermissionDescription))
            {
                warnings.Add("You have not configured automatically requesting App Tracking Transparency opt-in, but there is a description configured. This will overwrite NSUserTrackingUsageDescription.");
            }

            switch (settings.FacebookAudienceNetworkIntegration)
            {
                case FacebookAudienceNetworkIntegration.NoIntegration:
                    if (settings.RequestTrackingPermission && Reflection.HasFacebookAudienceNetworkUnity())
                    {
                        errors.Add("You have not enabled the automatic integration with the Unity version of the Facebook Audience Network, but it was deteted in your game.");
                    }

                    break;
                case FacebookAudienceNetworkIntegration.UnityIntegration:
                    if (!settings.RequestTrackingPermission)
                    {
                        errors.Add("The automatic integration with the Unity version of the Facebook Audience Network will not be performed. Enable requesting the tracking permission for it to be performed.");
                    }
                    else if (!Reflection.HasFacebookAudienceNetworkUnity())
                    {
                        errors.Add("The automatic integration with the Unity version of the Facebook Audience Network might not work. No assembly was found containing the code of the FAN.");
                    }

                    break;
                case FacebookAudienceNetworkIntegration.NativeIntegration:
                    if (!settings.RequestTrackingPermission)
                    {
                        errors.Add("The automatic integration with the native version of the Facebook Audience Network will not be performed. Enable requesting the tracking permission for it to be performed.");
                    }
                    else if (Reflection.HasFacebookAudienceNetworkUnity())
                    {
                        errors.Add("You have chosen to integrate with the native version of the Facebook Audience Network, but the Unity version of it was detected.");
                    }

                    break;
            }
        }

        /// <summary>
        /// Called when Unity is loaded to initialize justtrack SDK.
        /// </summary>
        /// <param name="settings">The JustTrack settings to use.</param>
        internal static void OnUnityLoaded(JustTrackSettings settings)
        {
            RunCodeGenerator(settings, GetFacebookAudienceNetworkIntegration(settings), false);
            JustTrackCodeGenerator.GenerateDependencyFile();
        }

        /// <summary>
        /// Runs the code generator with specified settings.
        /// </summary>
        /// <param name="settings">The JustTrack settings to use.</param>
        /// <param name="facebookAudienceNetworkIntegration">Facebook Audience Network integration settings.</param>
        /// <param name="force">True to force code generation, false otherwise.</param>
        internal static void RunCodeGenerator(JustTrackSettings settings, FacebookAudienceNetworkIntegration facebookAudienceNetworkIntegration, bool force)
        {
            JustTrackCodeGenerator.Run(facebookAudienceNetworkIntegration, settings, force);
        }

        /// <summary>
        /// Configures preprocessor defines for the specified build target group.
        /// </summary>
        /// <param name="targetGroup">The build target group to configure defines for.</param>
        internal static void ConfigurePreprocessorDefines(BuildTargetGroup targetGroup)
        {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            string newDefines = GeneratePreprocessorDefines(defines, Enum.GetName(typeof(BuildTargetGroup), targetGroup));
            if (newDefines != defines)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, newDefines);
                try
                {
                    // Sometimes the defines are not updated directly... we have to press ctrl+s first. Avoid that need by doing it automatically.
                    EditorApplication.ExecuteMenuItem("File/Save Project");
                }
                catch (Exception)
                {
                    Debug.LogError("Could not save project automatically. Please save project by hand to ensure preprocessor defines are up to date.");
                }
            }
        }

        private static string GeneratePreprocessorDefines(string currentDefines, string platform)
        {
            return BuildPreprocessorDefines(currentDefines, platform, new Dictionary<string, bool>
            {
                // keep them for now to ensure we drop those defines again, remove in 2024 or so
                ["JUSTTRACK_APPSFLYER_INTEGRATION"] = false,
                ["JUSTTRACK_IRONSOURCE_INTEGRATION"] = false,
            });
        }

        private static string BuildPreprocessorDefines(string currentDefines, string platform, Dictionary<string, bool> wantedDefines)
        {
            List<string> defines = new List<string>();
            foreach (var define in currentDefines.Split(';'))
            {
                if (wantedDefines.ContainsKey(define))
                {
                    // define managed by us
                    if (wantedDefines[define])
                    {
                        // we want to keep it
                        defines.Add(define);
                    }
                    else
                    {
                        Debug.Log("Dropping " + define + " preprocessor symbol for platform " + platform);
                    }

                    // remove it in any case, afterwards we don't need to iterate over the defines again
                    wantedDefines.Remove(define);
                }
                else
                {
                    // unknown define, keep it as it is
                    defines.Add(define);
                }
            }

            foreach (var wantedDefine in wantedDefines)
            {
                if (wantedDefine.Value)
                {
                    // we wanted to have this, but didn't find it yet, so add it
                    defines.Add(wantedDefine.Key);
                    Debug.Log("Adding " + wantedDefine.Key + " preprocessor symbol for platform " + platform);
                }
            }

            return string.Join(";", defines.ToArray());
        }

        /// <summary>
        /// Gets the Facebook Audience Network integration setting from settings.
        /// </summary>
        /// <param name="settings">The JustTrack settings to read from.</param>
        /// <returns>The Facebook Audience Network integration setting.</returns>
        internal static FacebookAudienceNetworkIntegration GetFacebookAudienceNetworkIntegration(JustTrackSettings settings)
        {
            if (string.IsNullOrEmpty(settings.IosApiToken))
            {
                return FacebookAudienceNetworkIntegration.NoIntegration;
            }

            return settings.IosTrackingSettings.FacebookAudienceNetworkIntegration;
        }

        /// <summary>
        /// Checks if IL2CPP scripting backend is used for the specified platform.
        /// </summary>
        /// <param name="isAndroid">True to check Android platform, false for iOS.</param>
        /// <returns>True if IL2CPP is used, false otherwise.</returns>
        internal static bool IsIL2CPP(bool isAndroid)
        {
            var backend = PlayerSettings.GetScriptingBackend(isAndroid ? BuildTargetGroup.Android : BuildTargetGroup.iOS);
            return backend == ScriptingImplementation.IL2CPP;
        }

        /// <summary>
        /// Detects if the External Dependency Manager is present.
        /// </summary>
        /// <returns>True if External Dependency Manager is detected, false otherwise.</returns>
        internal static bool DetectExternalDependencyManager()
        {
            var edmAssemblies = new List<string>(new string[]
            {
                "Google.PackageManagerResolver",
                "Google.VersionHandler",
                "Google.JarResolver",
                "Google.IOSResolver",
                "Google.VersionHandlerImpl",
            });
            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    if (edmAssemblies.Contains(assembly.GetName().Name))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Imports a Unity package from the specified URL.
        /// </summary>
        /// <param name="url">The URL to download the package from.</param>
        /// <returns>Coroutine enumerator.</returns>
        internal static IEnumerator ImportPackageFromUrl(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            while (!request.isDone || request.responseCode == -1)
            {
                yield return null; // what we yield is ignored anyway
            }

            if (request.responseCode < 200 || request.responseCode > 299)
            {
                Debug.LogError("Failed to download url: " + request.error + " with status " + request.responseCode);
                yield break;
            }

            string tempFile = Path.GetTempFileName();
            File.WriteAllBytes(tempFile, request.downloadHandler.data);
            AssetDatabase.ImportPackage(tempFile, true);

            // sadly we can't clean up the temp file as importing the EDM will cause a recompile of the scripts, causing this
            // routine to be unloaded before we can do anything else...
        }

        /// <summary>
        /// Cache of API token verification results.
        /// </summary>
#pragma warning disable SA1401 // Fields should be private
        internal static Dictionary<string, VerificationResultData> VerificationResults = new Dictionary<string, VerificationResultData>();
#pragma warning restore SA1401

        /// <summary>
        /// Verifies an API token for the specified package ID.
        /// </summary>
        /// <param name="packageId">The package ID to verify for.</param>
        /// <param name="apiToken">The API token to verify.</param>
        /// <param name="repaint">Action to call when verification completes.</param>
        /// <returns>The current verification result.</returns>
        internal static VerificationResultData VerifyApiToken(string packageId, string apiToken, Action repaint)
        {
            if (string.IsNullOrEmpty(packageId) || string.IsNullOrEmpty(apiToken))
            {
                return VerificationResultData.Missing();
            }

            var key = packageId + "\u2063" + apiToken;
            lock (VerificationResults)
            {
                if (VerificationResults.ContainsKey(key))
                {
                    return VerificationResults[key];
                }

                VerificationResults[key] = VerificationResultData.Pending();
            }

            CoroutineRuntime.StartCoroutine(DoVerifyApiToken(packageId, apiToken, (success, valid, errorMessage) =>
            {
                lock (VerificationResults)
                {
                    if (success)
                    {
                        VerificationResults[key] = valid ? VerificationResultData.Valid() : VerificationResultData.Invalid(errorMessage);
                    }
                    else
                    {
                        VerificationResults[key] = VerificationResultData.Unknown();
                    }
                }

                repaint();
            }));

            return VerificationResultData.Pending();
        }

#pragma warning disable SA1401 // Fields should be private (required for JSON serialization)
        [System.Serializable]
        private class ApiErrorResponse
        {
            public string Err = string.Empty;
        }
#pragma warning restore SA1401

        private static IEnumerator DoVerifyApiToken(string packageId, string apiToken, Action<bool, bool, string> callback)
        {
            string url, token;
            if (apiToken.StartsWith("prod-"))
            {
                url = "https://ipv4.justtrack.io/v0/sign";
                token = apiToken.Substring("prod-".Length);
            }
            else if (apiToken.StartsWith("sandbox-"))
            {
                url = $"https://ipv4.{LocalCredentials.SandboxRoot}/v0/sign";
                token = apiToken.Substring("sandbox-".Length);
            }
            else
            {
                url = "https://ipv4.justtrack.io/v0/sign";
                token = apiToken;
            }

            if (apiToken.Contains(" "))
            {
                callback(true, false, "API token contains spaces");
                yield break;
            }

            var request = UnityWebRequest.Get(url);
            request.SetRequestHeader("X-CLIENT-ID", packageId);
            request.SetRequestHeader("X-CLIENT-TOKEN", token);
            yield return request.SendWebRequest();

            while (!request.isDone || request.responseCode == -1)
            {
                yield return null; // what we yield is ignored anyway
            }

            var success = request.result == UnityWebRequest.Result.Success || request.result == UnityWebRequest.Result.ProtocolError;
            var valid = request.responseCode >= 200 && request.responseCode <= 299 && request.result == UnityWebRequest.Result.Success;

            string errorMessage = "Your API token is invalid. Please follow the instructions at https://docs.justtrack.io/sdk/latest/overview/find-your-justtrack-token to obtain a valid API token.";
            if (!valid && !string.IsNullOrEmpty(request.downloadHandler.text))
            {
                try
                {
                    ApiErrorResponse errorResponse = JsonUtility.FromJson<ApiErrorResponse>(request.downloadHandler.text);
                    if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Err))
                    {
                        errorMessage = errorResponse.Err;
                    }
                }
                catch (Exception)
                {
                    errorMessage += " Failed to parse request body.";
                }
            }

            callback(success, valid, errorMessage);
        }
    }
}
