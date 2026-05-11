#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.Build;
using System;
using System.IO;

namespace JustTrack {
    public static class PListPostProcessor {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath) {
            if (buildTarget != BuildTarget.iOS) {
                return;
            }

            JustTrackSettings settings = JustTrackUtils.GetSettings();
            if (settings == null) {
                throw new BuildFailedException("justtrack SDK configuration was not found");
            }

            // Configure Info.plist to contain our SKAdNetwork endpoint
            string pListPath = buildPath + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(pListPath));

            // set the attribution endpoint for our app to our backend
            if (!settings.IosTrackingSettings.UseCustomAdvertisingAttributionReportEndpoint) {
                plist.root.SetString("NSAdvertisingAttributionReportEndpoint", "https://justtrack-skadnetwork.io");
            }

            if (!String.IsNullOrEmpty(settings.IosTrackingSettings.TrackingPermissionDescription)) {
                plist.root.SetString("NSUserTrackingUsageDescription", settings.IosTrackingSettings.TrackingPermissionDescription);
            }

            // Write to file
            File.WriteAllText(pListPath, plist.WriteToString());
        }
    }
}
#endif
