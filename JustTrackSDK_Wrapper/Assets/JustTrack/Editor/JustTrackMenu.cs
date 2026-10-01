using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JustTrack
{
    /// <summary>
    /// Provides Unity editor menu items for the justtrack SDK integration and configuration.
    /// </summary>
    public class JustTrackMenu
    {
        /// <summary>
        /// Opens the justtrack SDK settings in the Unity Project Settings window.
        /// </summary>
        [MenuItem("justtrack/Show Settings", false, 100)]
        public static void ShowSettings()
        {
            SettingsService.OpenProjectSettings(JustTrackSettingsIMGUIRegister.SettingsPath);
        }

        /// <summary>
        /// Validates the current justtrack SDK configuration and displays any errors or warnings.
        /// </summary>
        [MenuItem("justtrack/Validate Configuration", false, 101)]
        public static void ValidateConfiguration()
        {
            Debug.ClearDeveloperConsole();
            ClearConsole();

            var settings = JustTrackUtils.GetOrCreateSettings();
            if (settings == null)
            {
                Debug.LogError("justtrack SDK configuration was not correctly loaded");
                return;
            }

            JustTrackUtils.Validate(settings, (validateResult) =>
            {
                foreach (string error in validateResult.Errors)
                {
                    Debug.LogError(error);
                }

                foreach (string warning in validateResult.Warnings)
                {
                    Debug.LogWarning(warning);
                }

                if (validateResult.Errors.Count == 0)
                {
                    Debug.Log("justtrack configuration is valid");
                }
            });
        }

        /// <summary>
        /// Creates an instance of the justtrack SDK prefab in the current scene.
        /// </summary>
        [MenuItem("justtrack/Create SDK Instance", false, 102)]
        public static void CreateInstance()
        {
            var path = "Packages/io.justtrack.justtrack-unity-sdk/Prefabs/JustTrackSDK.prefab";
            var fallback = "Assets/JustTrack/Prefabs/JustTrackSDK.prefab";
            UnityEngine.Object prefab;

            if (File.Exists(path))
            {
                prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            }
            else
            {
                prefab = AssetDatabase.LoadAssetAtPath(fallback, typeof(GameObject));
            }

            var instance = PrefabUtility.InstantiatePrefab(prefab);
            if (instance != null)
            {
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }

        /// <summary>
        /// Generates and displays a comprehensive diagnostic report of the justtrack SDK integration.
        /// </summary>
        [MenuItem("justtrack/Print Diagnostics", false, 103)]
        public static void PrintDiagnostics()
        {
            JustTrackSettings settings = JustTrackUtils.GetSettings();

            string[] thirdPartySDKs = new string[]
            {
                "com.ironsource.sdk",
                "com.appsflyer.af-android-sdk",
                "firebase-installations",
                "com.unity3d.ads",
                "com.chartboost:chartboost-sdk",
                "com.chartboost.mediation",
                "com.applovin.applovin-sdk",
            };

            var commonInfoLines = new List<string>();
            var androidInfoLines = new List<string>();
            var iOSInfoLines = new List<string>();

            commonInfoLines.Add($"Unity Version: {Application.unityVersion}");
            commonInfoLines.Add($"Version: {JustTrackSDK.GetVersion()}");

            var generatedFiles = new List<string>();
            if (JustTrackCodeGenerator.HasFBAudienceNetworkAdpt())
            {
                generatedFiles.Add("FacebookAudienceNetworkAdapter.cs");
            }

            commonInfoLines.Add("Generated Files: " + JoinElements(generatedFiles));

            commonInfoLines.Add("Scene Count: " + SceneManager.sceneCount);
            commonInfoLines.Add(AnalyizeScene("Main Scene", SceneManager.GetSceneByBuildIndex(0)));
            for (int i = 1; i < SceneManager.sceneCount; i++)
            {
                commonInfoLines.Add(AnalyizeScene("Scene " + i, SceneManager.GetSceneByBuildIndex(i)));
            }

            androidInfoLines.Add("API Token: " + settings.AndroidApiToken);
            androidInfoLines.Add("Package ID: " + PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android));
            androidInfoLines.Add("Scripting Backend: " + (JustTrackUtils.IsIL2CPP(true) ? "IL2CPP" : "Mono"));
            androidInfoLines.Add("Automatic Purchase Tracking: " + !settings.AndroidDisableAutomaticInAppPurchaseTracking);

            iOSInfoLines.Add("API Token: " + settings.IosApiToken);
            iOSInfoLines.Add("Package ID: " + PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS));
            iOSInfoLines.Add("Scripting Backend: " + (JustTrackUtils.IsIL2CPP(false) ? "IL2CPP" : "Mono"));
            iOSInfoLines.Add("Automatic Purchase Tracking: " + !settings.IosDisableAutomaticInAppPurchaseTracking);

            var justtrackSdks = new List<string>();
            var nativeSdks = new List<string>();

            // Check external libs in Assets folder
            string[] sdkExtensions = new string[] { ".dll", ".jar", ".a", ".framework", ".aar" };
            foreach (string assetPath in AssetDatabase.GetAllAssetPaths())
            {
                if (!ArrayUtility.Contains(sdkExtensions, Path.GetExtension(assetPath)))
                {
                    continue;
                }

                string fileName = Path.GetFileName(assetPath);
                Match match = Regex.Match(fileName, @"^(.*)-(\d+(\.\d+)*.*)$");
                string sdkName = fileName;
                string sdkVersion = string.Empty;

                if (match.Success)
                {
                    sdkName = match.Groups[1].Value;
                    sdkVersion = match.Groups[2].Value;
                }

                foreach (var thirdPartySDK in thirdPartySDKs)
                {
                    if (fileName.Contains(thirdPartySDK))
                    {
                        nativeSdks.Add($"{sdkName}: {sdkVersion}");
                        break;
                    }
                }

                if (fileName.Contains("justtrack"))
                {
                    justtrackSdks.Add($"{sdkName}: {sdkVersion} (native)");
                }
            }

            nativeSdks.Sort();

            JustTrackUtils.Validate(settings, (validateResult) =>
            {
                var verificationBlock = new List<string>();
                foreach (string error in validateResult.Errors)
                {
                    verificationBlock.Add("Error: " + error);
                }

                foreach (string warning in validateResult.Warnings)
                {
                    verificationBlock.Add("Warning: " + warning);
                }

                if (validateResult.Errors.Count == 0)
                {
                    verificationBlock.Add("Verification Status: valid");
                }
                else
                {
                    verificationBlock.Add("Verification Status: invalid");
                }

                var report = CombineBlock("Common", commonInfoLines) +
                    CombineBlock("Native version", justtrackSdks) +
                    CombineBlock("Android", androidInfoLines) +
                    CombineBlock("iOS", iOSInfoLines) +
                    CombineBlock("Native Dependencies", nativeSdks) +
                    CombineBlock("Verification", verificationBlock);
                DisplayDiagnoseDialog(report);
            });
        }

        private static string AnalyizeScene(string prefix, Scene scene)
        {
            if (scene == null)
            {
                return prefix + ": No scene found or loaded";
            }

            if (!scene.IsValid())
            {
                return prefix + ": " + scene.name + " (INVALID)";
            }

            GameObject[] rootObjects;
            try
            {
                rootObjects = scene.GetRootGameObjects();
            }
            catch (Exception e)
            {
                return prefix + ": " + scene.name + " (ERROR: " + e + ")";
            }

            foreach (GameObject go in rootObjects)
            {
                if (HasJustTrackSDK(go))
                {
                    return prefix + ": " + scene.name + " (contains JustTrack SDK)";
                }
            }

            return prefix + ": " + scene.name + " (Does not contain JustTrack SDK)";
        }

        private static string JoinElements(List<string> elements)
        {
            if (elements.Count == 0)
            {
                return "-";
            }

            return string.Join(", ", elements.ToArray());
        }

        private static bool HasJustTrackSDK(GameObject gameObject)
        {
            return gameObject.GetComponentInChildren<JustTrackSDKBehaviour>() != null;
        }

        private static string CombineBlock(string name, List<string> block)
        {
            if (block.Count == 0)
            {
                block.Add("- empty -");
            }

            return "\n### " + name + " ###\n\n" + string.Join("\n", block.ToArray()) + "\n";
        }

        private static void DisplayDiagnoseDialog(string dialogText)
        {
            int option = EditorUtility.DisplayDialogComplex("Integration Scan", dialogText, "OK", "Copy", null);
            if (option == 1)
            {
                GUIUtility.systemCopyBuffer = dialogText;
            }
        }

        /// <summary>
        /// Opens the justtrack SDK Android documentation in the default web browser.
        /// </summary>
        [MenuItem("justtrack/Android Docs", false, 200)]
        public static void AndroidDocs()
        {
            Application.OpenURL("https://docs.justtrack.io/sdk/overview");
        }

        /// <summary>
        /// Opens the justtrack SDK iOS documentation in the default web browser.
        /// </summary>
        [MenuItem("justtrack/iOS Docs", false, 201)]
        public static void IOSDocs()
        {
            Application.OpenURL("https://docs.justtrack.io/sdk/overview");
        }

        /// <summary>
        /// Opens the justtrack SDK Unity documentation in the default web browser.
        /// </summary>
        [MenuItem("justtrack/Unity Docs", false, 202)]
        public static void UnityDocs()
        {
            Application.OpenURL("https://docs.justtrack.io/sdk/overview");
        }

        /// <summary>
        /// Opens the justtrack SDK WebGL documentation in the default web browser.
        /// </summary>
        [MenuItem("justtrack/WebGL Docs", false, 202)]
        public static void WebGLDocs()
        {
            Application.OpenURL("https://docs.justtrack.io/sdk/overview");
        }

        private static void ClearConsole()
        {
            try
            {
                var assembly = Assembly.GetAssembly(typeof(SceneView));
                var type = assembly.GetType("UnityEditor.LogEntries");
                var method = type.GetMethod("Clear");
                method.Invoke(new object(), null);
            }
            catch (Exception)
            {
                // well... then it seems like we don't clear the console today
            }
        }
    }
}
