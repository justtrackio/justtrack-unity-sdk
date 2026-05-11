using UnityEditor;
using UnityEngine;

namespace JustTrack
{
    /// <summary>
    /// Handles loading and initialization of justtrack SDK in the Unity editor.
    /// </summary>
    internal class JustTrackEditorLoad
    {
        private const string EdmDialogKey = "io.justtrack.unity.edmDialogShown";
        private static bool androidUsesIL2CPP = false;
        private static bool iOSUsesIL2CPP = false;
        private static bool refreshNeeded = false;
        private static ImportAssetOptions refreshOptions = ImportAssetOptions.Default;

        /// <summary>
        /// Called when the project is loaded in the Unity editor.
        /// </summary>
        [InitializeOnLoadMethod]
        private static void OnProjectLoadedInEditor()
        {
            androidUsesIL2CPP = JustTrackUtils.IsIL2CPP(true);
            iOSUsesIL2CPP = JustTrackUtils.IsIL2CPP(false);

            // ensure everything is loaded before we maybe work on a partially loaded asset database
            RefreshAssetDatabase(ImportAssetOptions.ForceSynchronousImport);

            // ensure settings are created and everything is initalized nicely
            var settings = JustTrackUtils.GetOrCreateSettings();
            if (settings != null)
            {
                ValidateInitialSettings(settings);
            }
            else
            {
                EditorApplication.update += ValidateInitialSettingsDelayed;
            }

            EditorApplication.update += Update;

            JustTrackCodeGenerator.GenerateDependencyFile();

            if (!JustTrackUtils.DetectExternalDependencyManager() && !SessionState.GetBool(EdmDialogKey, false))
            {
                SessionState.SetBool(EdmDialogKey, true);
                var result = EditorUtility.DisplayDialogComplex(
                    "justtrack SDK",
                    "The External Dependency Manager for Unity is missing. This is a required dependency of the justtrack SDK. Please go to " + JustTrackUtils.EdmDownloadPage + " and download version 1.2.167 or later. You can also let the justtrack SDK import the newest version directly.",
                    "Import External Dependency Manager for Unity",
                    "Skip for now",
                    "Open download page");
                switch (result)
                {
                    case 0:
                        CoroutineRuntime.StartCoroutine(JustTrackUtils.ImportPackageFromUrl(JustTrackUtils.EdmPackageUrl));
                        break;
                    case 1:
                        break;
                    case 2:
                        Application.OpenURL(JustTrackUtils.EdmDownloadPage);
                        break;
                }
            }
        }

        private static void ValidateInitialSettingsDelayed()
        {
            JustTrackSettings? settings = JustTrackUtils.GetOrCreateSettings();
            if (settings == null)
            {
                return;
            }

            EditorApplication.update -= ValidateInitialSettingsDelayed;
            ValidateInitialSettings(settings);
        }

        private static void ValidateInitialSettings(JustTrackSettings settings)
        {
            OnIL2CPPChanged(settings);

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
            });
        }

        private static void Update()
        {
            RefreshIfNeeded();
            JustTrackSettings settings = JustTrackUtils.GetSettings();
            if (settings == null)
            {
                return;
            }

            if ((androidUsesIL2CPP != JustTrackUtils.IsIL2CPP(true)) || (iOSUsesIL2CPP != JustTrackUtils.IsIL2CPP(false)))
            {
                androidUsesIL2CPP = JustTrackUtils.IsIL2CPP(true);
                iOSUsesIL2CPP = JustTrackUtils.IsIL2CPP(false);
                OnIL2CPPChanged(settings);
            }

            JustTrackUtils.ConfigurePreprocessorDefines(BuildTargetGroup.Android);
            JustTrackUtils.ConfigurePreprocessorDefines(BuildTargetGroup.iOS);
        }

        private static void OnIL2CPPChanged(JustTrackSettings settings)
        {
            JustTrackUtils.OnUnityLoaded(settings);
        }

        private static void RefreshIfNeeded()
        {
            if (refreshNeeded)
            {
                refreshNeeded = false;
                RefreshAssetDatabase(refreshOptions);
            }
        }

        /// <summary>
        /// Refreshes the Unity asset database with the specified import options.
        /// </summary>
        /// <param name="options">The import asset options to use during refresh.</param>
        internal static void RefreshAssetDatabase(ImportAssetOptions options = ImportAssetOptions.Default)
        {
            if (!EditorApplication.isUpdating)
            {
                AssetDatabase.Refresh(options);
            }
            else
            {
                refreshNeeded = true;
                refreshOptions = options;
            }
        }
    }
}
