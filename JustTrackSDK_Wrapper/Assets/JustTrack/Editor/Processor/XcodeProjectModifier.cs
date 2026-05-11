#if UNITY_IOS
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEngine;

namespace JustTrack
{
    public static class XcodeProjectModifier
    {
        private const string UseFrameworksPodfileLine = "use_frameworks!";
        private const string UseFrameworksDynamicPodfileLine = "use_frameworks! :linkage => :dynamic";
        private const string UseFrameworksStaticPodfileLine = "use_frameworks! :linkage => :static";

        private static readonly string[] VendoredFrameworkPods =
        {
            "JustTrackSDK",
            "JustTrackSDKAppLovinAdapter",
            "JustTrackSDKFirebaseAdapter",
            "JustTrackSDKGoogleOdmAdapter",
            "JustTrackSDKIronSourceAdapter",
            "JustTrackSDKUnityAdsAdapter",
        };

        [PostProcessBuild(100)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget != BuildTarget.iOS)
            {
                return;
            }

            JustTrackSettings settings = JustTrackUtils.GetSettings();
            if (settings == null)
            {
                return;
            }

            ModifyPbxProj(buildPath, settings);
        }

        private static void ModifyPbxProj(string buildPath, JustTrackSettings settings)
        {
            string pbxProjectPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(pbxProjectPath);

            string frameworkGuid = pbxProject.GetUnityFrameworkTargetGuid();
            string mainTargetGuid = pbxProject.GetUnityMainTargetGuid();

            ConfigureSwiftCompilationConditions(pbxProject, frameworkGuid, settings);
            ConfigureFrameworkLinkage(pbxProject, frameworkGuid, mainTargetGuid, buildPath);

            pbxProject.WriteToFile(pbxProjectPath);
        }

        private static void ConfigureSwiftCompilationConditions(PBXProject pbxProject, string frameworkGuid, JustTrackSettings settings)
        {
            string propertyName = "SWIFT_ACTIVE_COMPILATION_CONDITIONS";
            string propertyValue = pbxProject.GetBuildPropertyForConfig(frameworkGuid, propertyName) + " JUSTTRACK_UNITY";

            if (settings.IosAppLovinIntegration)
            {
                propertyValue += " JUSTTRACK_UNITY_APPLOVIN";
            }

            if (settings.IosFirebaseIntegration)
            {
                propertyValue += " JUSTTRACK_UNITY_FIREBASE";
            }

            if (settings.IosIronSourceIntegration)
            {
                propertyValue += " JUSTTRACK_UNITY_IRONSOURCE";
            }

            if (settings.IosUnityAdsIntegration)
            {
                propertyValue += " JUSTTRACK_UNITY_UNITYADS";
            }

            if (settings.IosGoogleOdmIntegration)
            {
                propertyValue += " JUSTTRACK_UNITY_GOOGLE_ODM";
            }

            pbxProject.SetBuildProperty(frameworkGuid, propertyName, propertyValue.Trim());
        }

        /// <summary>
        /// Configures the Xcode project based on the framework linkage type detected in the Podfile.
        /// When static linkage is used, additional build settings are required for Swift compatibility.
        /// </summary>
        private static void ConfigureFrameworkLinkage(PBXProject pbxProject, string frameworkGuid, string mainTargetGuid, string buildPath)
        {
            bool isStaticLinkage = IsStaticFrameworkLinkage(buildPath);

            if (isStaticLinkage)
            {
                Debug.Log("[JustTrack] Static framework linkage detected. Configuring Xcode project for Swift compatibility.");

                pbxProject.SetBuildProperty(mainTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
                pbxProject.SetBuildProperty(frameworkGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");

                EmbedVendoredFrameworks(pbxProject, mainTargetGuid, buildPath);
            }
            else
            {
                Debug.Log("[JustTrack] Dynamic framework linkage detected. Using default Xcode project configuration.");
            }

            pbxProject.SetBuildProperty(frameworkGuid, "CLANG_ENABLE_MODULES", "YES");
        }

        /// <summary>
        /// Embeds vendored dynamic JustTrack XCFrameworks into the main target.
        /// This is necessary when static linkage is used because CocoaPods does not
        /// automatically embed vendored dynamic frameworks in static linkage mode.
        /// </summary>
        private static void EmbedVendoredFrameworks(PBXProject pbxProject, string mainTargetGuid, string buildPath)
        {
            string podsDir = Path.Combine(buildPath, "Pods");
            if (!Directory.Exists(podsDir))
            {
                Debug.LogWarning("[JustTrack] Pods directory not found. Cannot embed vendored frameworks.");
                return;
            }

            foreach (string podName in VendoredFrameworkPods)
            {
                string xcframeworkPath = Path.Combine(podsDir, podName, podName + ".xcframework");
                if (!Directory.Exists(xcframeworkPath))
                {
                    continue;
                }

                string relativePath = "Pods/" + podName + "/" + podName + ".xcframework";
                string projectPath = "Frameworks/" + podName + ".xcframework";

                string fileGuid = pbxProject.AddFile(relativePath, projectPath, PBXSourceTree.Source);
                PBXProjectExtensions.AddFileToEmbedFrameworks(pbxProject, mainTargetGuid, fileGuid);

                Debug.Log("[JustTrack] Embedded " + podName + ".xcframework in main target for static linkage compatibility.");
            }
        }

        /// <summary>
        /// Determines if the Podfile uses static framework linkage.
        /// Checks for 'use_frameworks! :linkage => :static' directive.
        /// </summary>
        private static bool IsStaticFrameworkLinkage(string buildPath)
        {
            string podfilePath = Path.Combine(buildPath, "Podfile");

            if (!File.Exists(podfilePath))
            {
                Debug.Log("[JustTrack] Podfile not found, assuming dynamic linkage.");
                return false;
            }

            try
            {
                string[] lines = File.ReadAllLines(podfilePath);

                int useFrameworksStaticLineIndex = Array.FindIndex(lines, line => line.Contains(UseFrameworksStaticPodfileLine));

                if (useFrameworksStaticLineIndex == -1)
                {
                    return false;
                }

                int useFrameworksLineIndex = Array.FindIndex(lines, line => line.Trim() == UseFrameworksPodfileLine);
                int useFrameworksDynamicLineIndex = Array.FindIndex(lines, line => line.Contains(UseFrameworksDynamicPodfileLine));

                return useFrameworksLineIndex < useFrameworksStaticLineIndex
                    && useFrameworksDynamicLineIndex < useFrameworksStaticLineIndex;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[JustTrack] Failed to read Podfile: " + ex.Message + ". Assuming dynamic linkage.");
                return false;
            }
        }
    }
}
#endif