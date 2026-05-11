using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace JustTrack
{
    /// <summary>
    /// Generates and manages injected code for third-party integrations.
    /// </summary>
    internal class JustTrackCodeGenerator
    {
        private const string DependencyDictionary = "Assets/JustTrack/Editor";
        private const string InjectedCodeDirectory = "Assets/JustTrack/Injected";
        private const string FbAudienceNetworkAdptFile = "/FacebookAudienceNetworkAdapter.cs";
        private const string RemoveKeyAsked = "io.justtrack.unity.shouldRemoveInjectedCode.asked";
        private const string RemoveKeyResult = "io.justtrack.unity.shouldRemoveInjectedCode.result";
        private const string InjectKeyAsked = "io.justtrack.unity.shouldInjectCode.asked";
        private const string InjectKeyResult = "io.justtrack.unity.shouldInjectCode.result";

        private static readonly string MainTemplatePath = "Assets/Plugins/Android/mainTemplate.gradle";

        /// <summary>
        /// Runs the code generation process based on current settings and requirements.
        /// </summary>
        /// <param name="facebookAudienceNetworkIntegration">The Facebook Audience Network integration mode.</param>
        /// <param name="settings">The JustTrack settings to use.</param>
        /// <param name="force">Whether to force code generation regardless of current state.</param>
        internal static void Run(FacebookAudienceNetworkIntegration facebookAudienceNetworkIntegration, JustTrackSettings settings, bool force)
        {
            if (facebookAudienceNetworkIntegration == FacebookAudienceNetworkIntegration.NoIntegration)
            {
                if (force || ShouldRemoveInjectedCode(settings))
                {
                    RemoveInjectedCode();
                }
            }
            else
            {
                if (force || Directory.Exists(InjectedCodeDirectory) || ShouldInjectCode(settings))
                {
                    GenerateInjectedCode(facebookAudienceNetworkIntegration);
                }
            }
        }

        /// <summary>
        /// Checks if the Facebook Audience Network adapter file exists.
        /// </summary>
        /// <returns><c>true</c> if the Facebook Audience Network adapter exists; otherwise, <c>false</c>.</returns>
        internal static bool HasFBAudienceNetworkAdpt()
        {
            var path = InjectedCodeDirectory + FbAudienceNetworkAdptFile;
            return File.Exists(path);
        }

        private static bool ShouldRemoveInjectedCode(JustTrackSettings settings)
        {
            if (!Directory.Exists(InjectedCodeDirectory))
            {
                // nothing to remove, no need to remove anything
                return false;
            }

            if (settings.AlwaysUpdateInjectedCode)
            {
                return true;
            }

            if (SessionState.GetBool(RemoveKeyAsked, false))
            {
                return SessionState.GetBool(RemoveKeyResult, false);
            }

            SessionState.SetBool(RemoveKeyAsked, true);
            var result = EditorUtility.DisplayDialogComplex(
                "justtrack SDK",
                "The code injected by the justtrack SDK into your game (located in " + InjectedCodeDirectory + ") is no longer needed. Do you want to remove it?",
                "Remove injected code",
                "Keep injected code",
                "Always update injected code automatically");
            switch (result)
            {
                case 2:
                    // always ok
                    JustTrackUtils.SetAlwaysUpdateInjectedCode(true);

                    // fall through to handling the ok case
                    goto case 0;
                case 0:
                    // ok
                    // reset the state of the other message to be able to ask again
                    SessionState.SetBool(RemoveKeyResult, true);
                    SessionState.SetBool(InjectKeyAsked, false);
                    return true;
                case 1:
                default:
                    // cancel
                    SessionState.SetBool(RemoveKeyResult, false);
                    return false;
            }
        }

        private static bool ShouldInjectCode(JustTrackSettings settings)
        {
            if (settings.AlwaysUpdateInjectedCode)
            {
                return true;
            }

            if (SessionState.GetBool(InjectKeyAsked, false))
            {
                return SessionState.GetBool(InjectKeyResult, false);
            }

            SessionState.SetBool(InjectKeyAsked, true);
            var result = EditorUtility.DisplayDialogComplex(
                "justtrack SDK",
                "The justtrack SDK needs to add some code to your game to help with the integration of some of the SDKs (like the IronSource SDK). It will be placed in " + InjectedCodeDirectory + " and needs to be compiled in the default assembly (Assembly-CSharp.dll).",
                "Create or update the injected code",
                "Don't add code to my project",
                "Always update injected code automatically");
            switch (result)
            {
                case 2:
                    // always ok
                    JustTrackUtils.SetAlwaysUpdateInjectedCode(true);

                    // fall through to handling the ok case
                    goto case 0;
                case 0:
                    // ok
                    // reset the state of the other message to be able to ask again
                    SessionState.SetBool(InjectKeyResult, true);
                    SessionState.SetBool(RemoveKeyAsked, false);
                    return true;
                case 1:
                default:
                    // cancel
                    SessionState.SetBool(InjectKeyResult, false);
                    return false;
            }
        }

        private static void RemoveInjectedCode()
        {
            if (!Directory.Exists(InjectedCodeDirectory))
            {
                return;
            }

            Directory.Delete(InjectedCodeDirectory, true);
            File.Delete(InjectedCodeDirectory + ".meta");
            JustTrackEditorLoad.RefreshAssetDatabase();
        }

        /// <summary>
        /// Generates injected code files based on current integration settings.
        /// </summary>
        /// <param name="facebookAudienceNetworkIntegration">The Facebook Audience Network integration mode.</param>
        internal static void GenerateInjectedCode(FacebookAudienceNetworkIntegration facebookAudienceNetworkIntegration)
        {
            Directory.CreateDirectory(InjectedCodeDirectory);
            bool needsRefresh = false;
            var readmeFile = InjectedCodeDirectory + "/ReadMe.md";
            var facebookAudienceNetworkAdapterFile = InjectedCodeDirectory + FbAudienceNetworkAdptFile;

            if (!File.Exists(readmeFile) || File.ReadAllText(readmeFile) != GetReadmeString())
            {
                needsRefresh = true;
                File.WriteAllText(readmeFile, GetReadmeString());
            }

            if (facebookAudienceNetworkIntegration != FacebookAudienceNetworkIntegration.NoIntegration)
            {
                bool unityIntegration = facebookAudienceNetworkIntegration == FacebookAudienceNetworkIntegration.UnityIntegration;
                if (!File.Exists(facebookAudienceNetworkAdapterFile) || File.ReadAllText(facebookAudienceNetworkAdapterFile) != GetFacebookAudienceNetworkAdapterString(unityIntegration))
                {
                    needsRefresh = true;
                    File.WriteAllText(facebookAudienceNetworkAdapterFile, GetFacebookAudienceNetworkAdapterString(unityIntegration));
                }
            }
            else if (File.Exists(facebookAudienceNetworkAdapterFile))
            {
                needsRefresh = true;
                File.Delete(facebookAudienceNetworkAdapterFile);
                File.Delete(facebookAudienceNetworkAdapterFile + ".meta");
            }

            if (needsRefresh)
            {
                JustTrackEditorLoad.RefreshAssetDatabase();
            }
        }

        /// <summary>
        /// Generates the dependency file for native SDK dependencies.
        /// </summary>
        internal static void GenerateDependencyFile()
        {
            Debug.Log("[JustTrack] Generating native dependency file...");
            bool hasGradleTemplate = File.Exists(MainTemplatePath);

            Directory.CreateDirectory(DependencyDictionary);
            bool needsRefresh = false;
            var dependencyFile = DependencyDictionary + "/JustTrackSDKNativeDependencies.xml";

            var expectedContents = GetDependenciesXML(hasGradleTemplate);

            if (!File.Exists(dependencyFile) || File.ReadAllText(dependencyFile) != expectedContents)
            {
                needsRefresh = true;
                File.WriteAllText(dependencyFile, expectedContents);
            }

            if (needsRefresh)
            {
                JustTrackEditorLoad.RefreshAssetDatabase();
            }
        }

        private static string GetReadmeString()
        {
            return @"# justtrack SDK - Injected Code

This folder contains code managed by the justtrack SDK.

## Why is this needed?

To make it easy to integrate and update the justtrack SDK it is provided as a Unity package.
However, this means that the C# code of the justtrack SDK can't access code from the default
assembly (Assembly-CSharp.dll). This is normally a good thing and can speed up builds by some
bit, but it also comes with some drawbacks. Mainly, some SDKs like the IronSource SDK are
provided as a .unitypackage by default and therefore end up in the default assembly if no futher
changes are made to them by a developer. This means, we can only access them via reflection at
runtime.

For the most part, using reflection is sufficient to perform the needed tasks. However, sometimes
we need to generate a small amount of code at runtime and this might not be supported if you are
using the IL2CPP backend instead of the Mono backend for greater speed. Thus, we need to add some
small bridging code to the default assembly to avoid having to generate that code at runtime.

## Can I edit these files?

There should be no need to change any of these files and your changes might be overwritten at any
time during futher development by the code which generated them in the first place. Instead, if
you need to make some changes, please tell us why you need to change them, what needs to be
changed and we will find a solution fitting you and improving the justtrack SDK for everyone
else, too. You can contact us at <mailto:support@justtrack.io> or <https://justtrack.io/contact/>.

## Should I add these files to version control?

You should add the files in this folder to version control. This will make it simpler along your
team to work with the justtrack SDK as not every developer has to make the decision whether to
actually create the generated code in the first place. It is also needed if you are building your
game in some CI pipeline to ensure a consistent version of your game is produced every time.
";
        }

        private static string GetFacebookAudienceNetworkAdapterString(bool unityIntegration)
        {
            if (unityIntegration)
            {
                return MakeInjectedFile(string.Empty, @"public class FacebookAudienceNetworkAdapter {
        #if UNITY_IOS
        [Preserve]
        public static void SetAdvertiserTrackingEnabled(bool enabled) {
            AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(enabled);
        }
        #endif
    }");
            }

            return MakeInjectedFile(
                @"
using System.Runtime.InteropServices;", @"public class FacebookAudienceNetworkAdapter {
        #if UNITY_IOS
        [DllImport(""__Internal"")]
        private static extern void FBAdSettingsBridgeSetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled);

        [Preserve]
        public static void SetAdvertiserTrackingEnabled(bool enabled) {
            FBAdSettingsBridgeSetAdvertiserTrackingEnabled(enabled);
        }
        #endif
    }");
        }

        private static string MakeInjectedFile(string imports, string contents)
        {
            return @"using System;
using JustTrack;" + imports + @"
using UnityEngine.Scripting;

// DO NOT EDIT - AUTOMATICALLY GENERATED BY THE JUSTTRACK SDK

namespace JustTrackInjected {
    [Preserve]
    " + contents + @"
}
";
        }

        private static string ResolveAdapterPackage(string version, string dependencyName, string dependencyPackage, string jtPackage)
        {
            string adapterVersion = version;
            try
            {
                var resolved = AndroidDependencyVersionResolver.ResolveAdapterVersionFromConfig(dependencyName, dependencyPackage, adapterVersion);
                if (!string.IsNullOrEmpty(resolved))
                {
                    adapterVersion = resolved;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[JustTrack] Error resolving {dependencyName} adapter version: {ex}");
            }

            var package = $"{jtPackage}:{adapterVersion}";
            return $"    <androidPackage spec=\"{package}\" />";
        }

        private static void AppendIOSDependency(StringBuilder builder, string name, string version)
        {
            builder.AppendLine($"    <iosPod name=\"{name}\" version=\"~> {version}\">");
            builder.AppendLine("      <sources></sources>");
            builder.AppendLine("    </iosPod>");
        }

        private static string GetDependenciesXML(bool hasGradleTemplate)
        {
            JustTrackSettings settings = JustTrackUtils.GetSettings();

            var xmlBuilder = new StringBuilder();

            // file header
            xmlBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            xmlBuilder.AppendLine("<dependencies>");

            // resolve android deps
            xmlBuilder.AppendLine("  <androidPackages>");

            foreach (var dependency in JustTrackDependencyDefinition.Dependencies)
            {
                var package = $"{dependency.GroupId}:{dependency.ArtifactId}:";

                if (hasGradleTemplate)
                {
                    package += $"[{dependency.Version},{dependency.UpperBoundVersion ?? string.Empty})";
                }
                else
                {
                    package += $"{dependency.Version}+";
                }

                xmlBuilder.AppendLine($"    <androidPackage spec=\"{package}\" />");
            }

            // resolve integrations adapters for Android
            if (settings != null && !string.IsNullOrEmpty(settings.AndroidApiToken))
            {
                if (settings.AndroidAppLovinIntegration)
                {
                    xmlBuilder.AppendLine(ResolveAdapterPackage(AdapterVersions.AndroidApplovin, "applovin", "com.applovin:applovin-sdk", "io.justtrack:adapter-applovin"));
                }

                if (settings.AndroidFirebaseIntegration)
                {
                    xmlBuilder.AppendLine(ResolveAdapterPackage(AdapterVersions.AndroidFirebase, "firebase", "com.google.firebase:firebase-analytics", "io.justtrack:adapter-firebase"));
                }

                if (settings.AndroidIronSourceIntegration)
                {
                    string adapterVersion = AdapterVersions.AndroidIronsource;
                    try
                    {
                        if (AndroidDependencyVersionResolver.GetAndroidDependencyVersion("com.ironsource.sdk:mediationsdk") != null)
                        {
                            // The old dependency "com.ironsource.sdk:mediationsdk" exist. Use version 1.x
                            adapterVersion = "1.+";
                        }
                        else if (AndroidDependencyVersionResolver.GetAndroidDependencyVersion("com.unity3d.ads-mediation:mediation-sdk") != null)
                        {
                            // The new dependency "com.unity3d.ads-mediation:mediation-sdk" exist. Use version 2.x
                            adapterVersion = "2.+";
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"[JustTrack] Error resolving Ironsource adapter {ex}");
                    }

                    var package = "io.justtrack:adapter-ironsource:" + adapterVersion;
                    xmlBuilder.AppendLine("    <androidPackage spec=\"" + package + "\" />");
                }

                if (settings.AndroidUnityAdsIntegration)
                {
                    xmlBuilder.AppendLine(ResolveAdapterPackage(AdapterVersions.AndroidUnityads, "unity", "com.unity3d.ads:unity-ads", "io.justtrack:adapter-unityads"));
                }
            }

            xmlBuilder.AppendLine("  </androidPackages>");

            // resolve iOS deps
            xmlBuilder.AppendLine("  <iosPods>");

            if (settings != null && !string.IsNullOrEmpty(settings.IosApiToken))
            {
                if (settings.IosAppLovinIntegration)
                {
                    AppendIOSDependency(xmlBuilder, "JustTrackSDKAppLovinAdapter", AdapterVersions.IosApplovin);
                }

                if (settings.IosFirebaseIntegration)
                {
                    AppendIOSDependency(xmlBuilder, "JustTrackSDKFirebaseAdapter", AdapterVersions.IosFirebase);
                }

                if (settings.IosIronSourceIntegration)
                {
                    AppendIOSDependency(xmlBuilder, "JustTrackSDKIronSourceAdapter", AdapterVersions.IosIronsource);
                }

                if (settings.IosUnityAdsIntegration)
                {
                    AppendIOSDependency(xmlBuilder, "JustTrackSDKUnityAdsAdapter", AdapterVersions.IosUnityads);
                }

                if (settings.IosGoogleOdmIntegration)
                {
                    AppendIOSDependency(xmlBuilder, "JustTrackSDKGoogleOdmAdapter", AdapterVersions.IosGoogleodm);
                }
            }

            xmlBuilder.AppendLine("  </iosPods>");

            xmlBuilder.AppendLine("</dependencies>");

            return xmlBuilder.ToString();
        }
    }

    /// <summary>
    /// Utility class for managing Unity package dependencies and scoped registries.
    /// </summary>
    internal class Dependencies
    {
        /// <summary>
        /// Adds a package dependency to the Unity manifest.json file.
        /// </summary>
        /// <param name="pPackage">The package name to add.</param>
        /// <param name="pVersionOrUrl">The version or URL of the package.</param>
        public static void AddDependency(string pPackage, string pVersionOrUrl)
        {
            var manifestPath = Path.Combine(Application.dataPath, "..", "Packages/manifest.json");
            var manifestJson = File.ReadAllText(manifestPath);

            var manifest = JsonConvert.DeserializeObject<ManifestJson>(manifestJson);
            if (manifest == null)
            {
                return;
            }

            manifest.dependencies[pPackage] = pVersionOrUrl;

            File.WriteAllText(manifestPath, JsonConvert.SerializeObject(manifest, Formatting.Indented));
        }

        /// <summary>
        /// Checks if a package dependency exists in the manifest.json file.
        /// </summary>
        /// <param name="package">The package name to check.</param>
        /// <returns><c>true</c> if the dependency exists; otherwise, <c>false</c>.</returns>
        public static bool HasDependency(string package)
        {
            var manifestPath = Path.Combine(Application.dataPath, "..", "Packages/manifest.json");
            var manifestJson = File.ReadAllText(manifestPath);

            var manifest = JsonConvert.DeserializeObject<ManifestJson>(manifestJson);
            if (manifest == null)
            {
                return false;
            }

            return manifest.dependencies.ContainsKey(package);
        }

        /// <summary>
        /// Gets the version of a package dependency from the manifest.json file.
        /// </summary>
        /// <param name="package">The package name to get the version for.</param>
        /// <returns>The version of the package, or <c>null</c> if not found.</returns>
        public static Version? GetVersion(string package)
        {
            var manifestPath = Path.Combine(Application.dataPath, "..", "Packages/manifest.json");
            var manifestJson = File.ReadAllText(manifestPath);

            var manifest = JsonConvert.DeserializeObject<ManifestJson>(manifestJson);
            if (manifest == null)
            {
                return null;
            }

            if (!manifest.dependencies.ContainsKey(package))
            {
                return null;
            }

            manifest.dependencies.TryGetValue(package, out string? version);
            return version != null ? new Version(version) : null;
        }

        /// <summary>
        /// Adds a scoped registry to the Unity manifest.json file.
        /// </summary>
        /// <param name="pScopeRegistry">The scoped registry to add.</param>
        public static void AddScopedRegistry(ScopedRegistry pScopeRegistry)
        {
            if (HasScopedRegistry(pScopeRegistry.Name))
            {
                return;
            }

            var manifestPath = Path.Combine(Application.dataPath, "..", "Packages/manifest.json");
            var manifestJson = File.ReadAllText(manifestPath);

            var manifest = JsonConvert.DeserializeObject<ManifestJson>(manifestJson);
            if (manifest == null)
            {
                return;
            }

            manifest.scopedRegistries.Add(pScopeRegistry);

            File.WriteAllText(manifestPath, JsonConvert.SerializeObject(manifest, Formatting.Indented));
        }

        /// <summary>
        /// Checks if a scoped registry with the specified scope exists.
        /// </summary>
        /// <param name="pScope">The scope to check for.</param>
        /// <returns><c>true</c> if a scoped registry with the scope exists; otherwise, <c>false</c>.</returns>
        public static bool HasScopedRegistry(string pScope)
        {
            var manifestPath = Path.Combine(Application.dataPath, "..", "Packages/manifest.json");
            var manifestJson = File.ReadAllText(manifestPath);

            var manifest = JsonConvert.DeserializeObject<ManifestJson>(manifestJson);
            if (manifest == null)
            {
                return false;
            }

            foreach (var scopedRegistry in manifest.scopedRegistries)
            {
                if (scopedRegistry.Scopes.Contains(pScope))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Represents a scoped registry configuration for Unity packages.
        /// </summary>
#pragma warning disable SA1401, SA1307 // Fields should be private; field names should begin with upper-case letter (required for JSON serialization)
        public class ScopedRegistry
        {
            /// <summary>
            /// The name of the scoped registry.
            /// </summary>
            public string Name = string.Empty;

            /// <summary>
            /// The URL of the scoped registry.
            /// </summary>
            public string Url = string.Empty;

            /// <summary>
            /// The scopes handled by this registry.
            /// </summary>
            public string[] Scopes = Array.Empty<string>();
        }

        /// <summary>
        /// Represents the structure of Unity's manifest.json file.
        /// </summary>
        public class ManifestJson
        {
            /// <summary>
            /// The package dependencies in the manifest.
            /// </summary>
            public Dictionary<string, string> dependencies = new Dictionary<string, string>();

            /// <summary>
            /// The scoped registries configured in the manifest.
            /// </summary>
            public List<ScopedRegistry> scopedRegistries = new List<ScopedRegistry>();
        }
#pragma warning restore SA1401, SA1307
    }
}
