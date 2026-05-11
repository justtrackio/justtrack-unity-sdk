using System;
using System.Collections.Generic;
using UnityEditor;

namespace JustTrack
{
    /// <summary>
    /// Registers the justtrack SDK settings provider for Unity's Project Settings window.
    /// </summary>
    internal static class JustTrackSettingsIMGUIRegister
    {
        /// <summary>
        /// The path where the justtrack SDK settings appear in the Project Settings window.
        /// </summary>
        internal const string SettingsPath = "Project/JustTrackIMGUISettings";

        /// <summary>
        /// Creates a settings provider for the justtrack SDK configuration.
        /// </summary>
        /// <returns>A configured settings provider for the justtrack SDK.</returns>
        [SettingsProvider]
        public static SettingsProvider CreateJustTrackSettingsProvider()
        {
            bool justTrackFoldout = true;
            int selectedApiTokenPlatform = 0;
            bool attFoldout = true;
            bool integrationsFoldout = true;
            bool ironsourceFoldout = true;
            int selectedIronsourcePlatform = 0;
            bool firebaseFoldout = true;
            int selectedFirebasePlatform = 0;
            bool loggingFoldout = true;

            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider(SettingsPath, SettingsScope.Project);

            // By default the last token of the path is used as display name if no label is provided.
            provider.label = "justtrack";

            // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
            provider.guiHandler = (searchContext) =>
            {
                try
                {
                    var settings = JustTrackUtils.GetSerializedSettings();
                    if (settings == null)
                    {
                        EditorGUILayout.HelpBox("No justtrack settings could be loaded.", MessageType.Error);
                        return;
                    }

                    JustTrackObjectEditor.RenderGUI(settings, ref justTrackFoldout, ref selectedApiTokenPlatform, ref attFoldout, ref integrationsFoldout, ref loggingFoldout, ref ironsourceFoldout, ref selectedIronsourcePlatform, ref firebaseFoldout, ref selectedFirebasePlatform, () =>
                    {
                        provider.Repaint();
                    });
                }
                catch (Exception e)
                {
                    EditorGUILayout.HelpBox("Failed to render settings: " + e.Message + "\n" + e.StackTrace, MessageType.Error);
                }
            };

            // Populate the search keywords to enable smart search filtering and label highlighting:
            provider.keywords = new HashSet<string>(new[] { "justtrack", "IronSource", "Firebase" });

            return provider;
        }
    }
}
