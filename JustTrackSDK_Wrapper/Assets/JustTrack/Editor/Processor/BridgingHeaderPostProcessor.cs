#if UNITY_IOS
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System;
using System.IO;
using System.Collections.Generic;

namespace JustTrack {
    public static class BridgingHeaderPostProcessor {
        private const string StartMarker = "// MARK: Other bridging headers";
        private const string EndMarker   = "// MARK: Swift bridging header";

        [PostProcessBuild(0x7FFFFFFF)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath) {
            if (buildTarget != BuildTarget.iOS) {
                return;
            }

            // We need to construct our own PBX project path that correctly refers to the Bridging header
            var projPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
            var proj = new PBXProject();
            proj.ReadFromFile(projPath);

            var headers = getBridgingHeaders(proj, buildPath);
            var headerLines = new List<string>();
            foreach (var header in headers) {
                headerLines.Add($"#include <UnityFramework/{header}>");
            }
            headerLines.Sort();

            if (headerLines.Count == 0) {
                Debug.Log($"No bridging headers found, no need to process JustTrackSDK.mm");
                return;
            }

            var objcBridgeFilePaths = Directory.GetFiles(buildPath, "JustTrackSDK.mm", SearchOption.AllDirectories);
            if (objcBridgeFilePaths.Length == 0) {
                Debug.LogError($"Failed to locate and patch JustTrackSDK.mm in {buildPath}. This currently happens when Symlink Unity Libraries is enabled.");
                Debug.LogError("You might have to add these lines manually to JustTrackSDK.mm:\n" + String.Join("\n", headerLines));
                return;
            }

            foreach (var objcBridgeFilePath in objcBridgeFilePaths) {
                var objcBridgeFile = File.ReadAllText(objcBridgeFilePath);
                var objcBridgeFileParts1 = objcBridgeFile.Split(new string[] { StartMarker }, 2, StringSplitOptions.None);
                var objcBridgeFileParts2 = objcBridgeFileParts1[1].Split(new string[] { EndMarker }, 2, StringSplitOptions.None);
                var objcBridgeFileLines = new string[2 + headerLines.Count];
                objcBridgeFileLines[0] = objcBridgeFileParts1[0];
                objcBridgeFileLines[headerLines.Count + 1] = objcBridgeFileParts2[1];
                for (int i = 0; i < headerLines.Count; i++) {
                    objcBridgeFileLines[i + 1] = headerLines[i];
                }
                objcBridgeFile = String.Join("\n", objcBridgeFileLines);
                File.WriteAllText(objcBridgeFilePath, objcBridgeFile);

                Debug.Log($"Added {headerLines.Count} headers to {objcBridgeFilePath}");
            }
        }

        private static ISet<string> getBridgingHeaders(PBXProject proj, string buildPath) {
            var bridgingHeaders = new HashSet<string>();
            var targetGuid = proj.GetUnityFrameworkTargetGuid();

            foreach (var configName in proj.BuildConfigNames()) {
                var configGuid = proj.BuildConfigByName(targetGuid, configName);
                var currentValue = proj.GetBuildPropertyForConfig(configGuid, "SWIFT_INCLUDE_PATHS");
                if (currentValue == null || currentValue == "") {
                    continue;
                }

                foreach (var path in currentValue.Split(' ')) {
                    foreach (var file in Directory.EnumerateFiles(buildPath + "/" + path)) {
                        if (file.EndsWith("-Bridging-Header.h")) {
                            bridgingHeaders.Add(Path.GetFileName(file));
                        }
                    }
                }
            }

            return bridgingHeaders;
        }
    }
}
#endif
