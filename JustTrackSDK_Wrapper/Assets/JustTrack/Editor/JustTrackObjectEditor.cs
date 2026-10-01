using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace JustTrack
{
    /// <summary>
    /// Custom Unity editor for the justtrack SDK behaviour component.
    /// </summary>
    [CustomEditor(typeof(JustTrackSDKBehaviour))]
    public class JustTrackObjectEditor : Editor
    {
        private bool justTrackFoldout = true;
        private int selectedApiTokenPlatform = 0;

        private bool attFoldout = true;

        private bool integrationsFoldout = true;

        private bool loggingFoldout = true;

        private bool ironsourceFoldout = true;
        private int selectedIronsourcePlatform = 0;

        private bool firebaseFoldout = true;
        private int selectedFirebasePlatform = 0;

        /// <summary>
        /// Renders the inspector GUI for the justtrack SDK behaviour.
        /// </summary>
        public override void OnInspectorGUI()
        {
            try
            {
                var settings = JustTrackUtils.GetSerializedSettings();
                if (settings == null)
                {
                    EditorGUILayout.HelpBox("No justtrack settings could be loaded.", MessageType.Error);
                    return;
                }

                RenderGUI(settings, ref justTrackFoldout, ref selectedApiTokenPlatform, ref attFoldout, ref integrationsFoldout, ref loggingFoldout, ref ironsourceFoldout, ref selectedIronsourcePlatform, ref firebaseFoldout, ref selectedFirebasePlatform, () =>
                {
                    Repaint();
                });
            }
            catch (Exception e)
            {
                EditorGUILayout.HelpBox("Failed to render settings: " + e.Message + "\n" + e.StackTrace, MessageType.Error);
            }
        }

        /// <summary>
        /// Renders the GUI elements for the justtrack SDK configuration.
        /// </summary>
        /// <param name="serializedObject">The serialized object to render.</param>
        /// <param name="justTrackFoldout">Reference to the justtrack foldout state.</param>
        /// <param name="selectedApiTokenPlatform">Reference to the selected API token platform.</param>
        /// <param name="attFoldout">Reference to the attribution foldout state.</param>
        /// <param name="integrationsFoldout">Reference to the integrations foldout state.</param>
        /// <param name="loggingFoldout">Reference to the console logging foldout state.</param>
        /// <param name="ironsourceFoldout">Reference to the ironSource foldout state.</param>
        /// <param name="selectedIronsourcePlatform">Reference to the selected ironSource platform.</param>
        /// <param name="firebaseFoldout">Reference to the Firebase foldout state.</param>
        /// <param name="selectedFirebasePlatform">Reference to the selected Firebase platform.</param>
        /// <param name="repaint">Action to trigger repaint of the inspector.</param>
        internal static void RenderGUI(SerializedObject serializedObject, ref bool justTrackFoldout, ref int selectedApiTokenPlatform, ref bool attFoldout, ref bool integrationsFoldout, ref bool loggingFoldout, ref bool ironsourceFoldout, ref int selectedIronsourcePlatform, ref bool firebaseFoldout, ref int selectedFirebasePlatform, Action repaint)
        {
            SerializedProperty manualStart = serializedObject.FindProperty("ManualStart");
            SerializedProperty useRuntimeConstructor = serializedObject.FindProperty("UseRuntimeConstructor");
            SerializedProperty androidApiToken = serializedObject.FindProperty("AndroidApiToken");
            SerializedProperty iosApiToken = serializedObject.FindProperty("IosApiToken");
            SerializedProperty webglApiToken = serializedObject.FindProperty("WebglApiToken");
            SerializedProperty webglBundleId = serializedObject.FindProperty("WebglBundleId");
            SerializedProperty webglAppVersion = serializedObject.FindProperty("WebglAppVersion");
            SerializedProperty webglAppCode = serializedObject.FindProperty("WebglAppCode");
            SerializedProperty webglUserId = serializedObject.FindProperty("WebglUserId");
            SerializedProperty androidBundleId = serializedObject.FindProperty("AndroidBundleId");
            SerializedProperty androidAppVersion = serializedObject.FindProperty("AndroidAppVersion");
            SerializedProperty androidAppCode = serializedObject.FindProperty("AndroidAppCode");
            SerializedProperty iosBundleId = serializedObject.FindProperty("IosBundleId");
            SerializedProperty iosAppVersion = serializedObject.FindProperty("IosAppVersion");
            SerializedProperty iosAppCode = serializedObject.FindProperty("IosAppCode");
            SerializedProperty serverUrl = serializedObject.FindProperty("ServerUrl");
            SerializedProperty iosTrackingSettings = serializedObject.FindProperty("IosTrackingSettings");
            SerializedProperty androidDisableAutomaticInAppPurchaseTracking = serializedObject.FindProperty("AndroidDisableAutomaticInAppPurchaseTracking");
            SerializedProperty iosDisableAutomaticInAppPurchaseTracking = serializedObject.FindProperty("IosDisableAutomaticInAppPurchaseTracking");

            SerializedProperty androidAppLovinIntegration = serializedObject.FindProperty("AndroidAppLovinIntegration");
            SerializedProperty iosAppLovinIntegration = serializedObject.FindProperty("IosAppLovinIntegration");
            SerializedProperty androidFirebaseIntegration = serializedObject.FindProperty("AndroidFirebaseIntegration");
            SerializedProperty iosFirebaseIntegration = serializedObject.FindProperty("IosFirebaseIntegration");
            SerializedProperty androidIronSourceIntegration = serializedObject.FindProperty("AndroidIronSourceIntegration");
            SerializedProperty iosIronSourceIntegration = serializedObject.FindProperty("IosIronSourceIntegration");
            SerializedProperty androidUnityAdsIntegration = serializedObject.FindProperty("AndroidUnityAdsIntegration");
            SerializedProperty iosUnityAdsIntegration = serializedObject.FindProperty("IosUnityAdsIntegration");
            SerializedProperty iosGoogleOdmIntegration = serializedObject.FindProperty("IosGoogleOdmIntegration");

            SerializedProperty iosTrackingSettingsRequestTrackingPermission = iosTrackingSettings.FindPropertyRelative("RequestTrackingPermission");
            SerializedProperty iosTrackingSettingsTrackingPermissionDescription = iosTrackingSettings.FindPropertyRelative("TrackingPermissionDescription");
            SerializedProperty iosTrackingSettingsTrackingPermissionDescriptionManaged = iosTrackingSettings.FindPropertyRelative("TrackingPermissionDescriptionManaged");
            SerializedProperty iosTrackingSettingsFacebookAudienceNetworkIntegration = iosTrackingSettings.FindPropertyRelative("FacebookAudienceNetworkIntegration");

            SerializedProperty enableConsoleLogging = serializedObject.FindProperty("EnableConsoleLogging");
            SerializedProperty enableConnectionTracking = serializedObject.FindProperty("EnableConnectionTracking");

            serializedObject.Update();

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                float previousLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 180;

                string[] platformNames = { "Android", "iOS", "WebGL" };
                var facebookAudienceNetworkIntegrationValues = Enum.GetValues(FacebookAudienceNetworkIntegration.NoIntegration.GetType());

                ////////////
                // HEADER //
                ////////////

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                Box("logo.png", 512, 128);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                ///////////////////////////////////////
                // EXTERNAL DEPENDENCY MANAGER CHECK //
                ///////////////////////////////////////

                if (!JustTrackUtils.DetectExternalDependencyManager())
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("The External Dependency Manager for Unity is missing. This is a required dependency of the justtrack SDK. Please go to " + JustTrackUtils.EdmDownloadPage + " and download version 1.2.167 or later.", MessageType.Error);

                    // Manual download button
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Open download page", new GUILayoutOption[] { GUILayout.Width(150) }))
                    {
                        Application.OpenURL(JustTrackUtils.EdmDownloadPage);
                    }

                    // Automatic download button
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Import External Dependency Manager for Unity", new GUILayoutOption[] { GUILayout.Width(300) }))
                    {
                        CoroutineRuntime.StartCoroutine(JustTrackUtils.ImportPackageFromUrl(JustTrackUtils.EdmPackageUrl));
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }

                ///////////////
                // API TOKEN //
                ///////////////

                bool hasAndroid = !string.IsNullOrEmpty(androidApiToken.stringValue);
                bool hasIOS = !string.IsNullOrEmpty(iosApiToken.stringValue);
                bool hasWebGL = !string.IsNullOrEmpty(webglApiToken.stringValue);

                EditorGUILayout.PropertyField(useRuntimeConstructor, new GUIContent("Use Runtime Constructor", "When enabled, the SDK will not auto-initialize. Call JustTrackSDK.Init() manually at runtime."));
                EditorGUILayout.Space();

                if (!useRuntimeConstructor.boolValue)
                {
                justTrackFoldout = EditorGUILayout.Foldout(justTrackFoldout, new GUIContent("justtrack SDK Settings"), true);

                if (justTrackFoldout)
                {
                    EditorGUILayout.Space();
                    selectedApiTokenPlatform = GUILayout.Toolbar(selectedApiTokenPlatform, platformNames);

                    string packageId = string.Empty;
                    string customBundleId = string.Empty;
                    switch (selectedApiTokenPlatform)
                    {
                        case 0:
                            customBundleId = androidBundleId.stringValue;
                            packageId = !string.IsNullOrEmpty(customBundleId) ? customBundleId : PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
                            break;
                        case 1:
                            customBundleId = iosBundleId.stringValue;
                            packageId = !string.IsNullOrEmpty(customBundleId) ? customBundleId : PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
                            break;
                        case 2:
                            packageId = webglBundleId.stringValue;
                            break;
                    }

                    // Only show Package ID for Android and iOS (not for WebGL)
                    if (selectedApiTokenPlatform != 2)
                    {
                        EditorGUILayout.LabelField(platformNames[selectedApiTokenPlatform] + " Package Id:", packageId);
                    }

                    EditorGUILayout.HelpBox("You have to configure the correct API token for each platform you want to build for. An API token should be a random 64 character string.", MessageType.Info);

                    string apiToken = string.Empty;
                    switch (selectedApiTokenPlatform)
                    {
                        case 0:
                            EditorGUILayout.PropertyField(androidApiToken, new GUIContent("Android API Token"));
                            apiToken = androidApiToken.stringValue;

                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("Android Configuration (Optional)", EditorStyles.boldLabel);
                            EditorGUILayout.HelpBox("Leave these fields empty to use the default values from PlayerSettings. Only set these if you need to override the bundle ID or app version.", MessageType.Info);
                            EditorGUILayout.PropertyField(androidBundleId, new GUIContent("Custom Bundle ID"));
                            EditorGUILayout.PropertyField(androidAppVersion, new GUIContent("Custom App Version"));
                            EditorGUILayout.PropertyField(androidAppCode, new GUIContent("Custom App Code"));
                            if (string.IsNullOrEmpty(androidAppVersion.stringValue) != string.IsNullOrEmpty(androidAppCode.stringValue))
                            {
                                EditorGUILayout.HelpBox("Custom App Version and Custom App Code must both be set or both be left empty.", MessageType.Error);
                            }

                            break;
                        case 1:
                            EditorGUILayout.PropertyField(iosApiToken, new GUIContent("iOS API Token"));
                            apiToken = iosApiToken.stringValue;

                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("iOS Configuration (Optional)", EditorStyles.boldLabel);
                            EditorGUILayout.HelpBox("Leave these fields empty to use the default values from PlayerSettings. Only set these if you need to override the bundle ID or app version.", MessageType.Info);
                            EditorGUILayout.PropertyField(iosBundleId, new GUIContent("Custom Bundle ID"));
                            EditorGUILayout.PropertyField(iosAppVersion, new GUIContent("Custom App Version"));
                            EditorGUILayout.PropertyField(iosAppCode, new GUIContent("Custom App Code"));
                            if (string.IsNullOrEmpty(iosAppVersion.stringValue) != string.IsNullOrEmpty(iosAppCode.stringValue))
                            {
                                EditorGUILayout.HelpBox("Custom App Version and Custom App Code must both be set or both be left empty.", MessageType.Error);
                            }

                            break;
                        case 2:
                            EditorGUILayout.PropertyField(webglApiToken, new GUIContent("WebGL API Token"));
                            apiToken = webglApiToken.stringValue;

                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("WebGL Configuration", EditorStyles.boldLabel);
                            EditorGUILayout.PropertyField(webglBundleId, new GUIContent("Bundle ID"));
                            EditorGUILayout.PropertyField(webglAppVersion, new GUIContent("App Version"));
                            EditorGUILayout.PropertyField(webglAppCode, new GUIContent("App Code"));
                            if (string.IsNullOrEmpty(webglAppVersion.stringValue) != string.IsNullOrEmpty(webglAppCode.stringValue))
                            {
                                EditorGUILayout.HelpBox("App Version and App Code must both be set or both be left empty.", MessageType.Error);
                            }

                            break;
                    }

                    var verificationResult = JustTrackUtils.VerifyApiToken(packageId, apiToken, repaint);
                    switch (verificationResult.Status)
                    {
                        case VerificationResult.VALID:
                            EditorGUILayout.HelpBox("Your API token is valid.", MessageType.None);
                            break;
                        case VerificationResult.INVALID:
                            string errorMessage = verificationResult.ErrorMessage
                                ?? "Your API token is invalid. Please follow the instructions at https://docs.justtrack.io/sdk/latest/overview/find-your-justtrack-token to obtain a valid API token.";
                            EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("Open documentation", new GUILayoutOption[] { GUILayout.Width(150) }))
                            {
                                Application.OpenURL("https://docs.justtrack.io/sdk/latest/overview/find-your-justtrack-token");
                            }

                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                            break;
                        case VerificationResult.PENDING:
                            EditorGUILayout.HelpBox("Verification of your API token is still pending. This should only take a few seconds...", MessageType.None);
                            break;
                        case VerificationResult.UNKNOWN:
                            EditorGUILayout.HelpBox("Verification of your API token could not be performed. Are you offline?", MessageType.Warning);
                            break;
                        case VerificationResult.MISSING:
                            EditorGUILayout.HelpBox("You did not configure an API token and can't use the justtrack SDK on " + platformNames[selectedApiTokenPlatform] + ".", MessageType.Error);
                            break;
                    }

                    switch (manualStart.boolValue)
                    {
                        case true:
                            EditorGUILayout.HelpBox("Manual start mode is enabled. After initializing the justtrack SDK, you must call the Start() method to make the SDK operational.", MessageType.Warning);
                            break;
                        case false:
                            EditorGUILayout.HelpBox("When manual start mode is disabled, the justtrack SDK begins operating immediately after initialization.", MessageType.Info);
                            break;
                    }

                    EditorGUILayout.PropertyField(manualStart, new GUIContent("Manual Start"));

                    EditorGUILayout.Space();

                    EditorGUILayout.HelpBox("Set a custom server URL for justtrack SDK. Leave empty to use the production server.", MessageType.Info);
                    EditorGUILayout.PropertyField(serverUrl, new GUIContent("Custom Server URL"));

                    EditorGUILayout.Space();

                    ///////////////////////
                    // TRACKING PROVIDER //
                    ///////////////////////

                    switch (selectedApiTokenPlatform)
                    {
                        case 0:
                            {
                                using (new EditorGUI.DisabledScope(!hasAndroid))
                                {
                                }

                                break;
                            }

                        case 1:
                            {
                                using (new EditorGUI.DisabledScope(!hasIOS))
                                {
                                    using (new EditorGUI.IndentLevelScope())
                                    {
                                        attFoldout = EditorGUILayout.Foldout(attFoldout, new GUIContent("App Tracking Transparency"), true);
                                        if (attFoldout)
                                        {
                                            EditorGUILayout.HelpBox("Here you can configure the justtrack SDK to automatically ask the user to opt in to Apple's App Tracking Transparency framework.", MessageType.Info);
                                            EditorGUILayout.PropertyField(iosTrackingSettingsRequestTrackingPermission, new GUIContent("Request Tracking Permission"));

                                            EditorGUILayout.HelpBox("If you request permission to track the user, you have to provide the NSUserTrackingUsageDescription string. You can configure a string here to automatically configure that setting for you.", MessageType.Info);
                                            EditorGUILayout.PropertyField(iosTrackingSettingsTrackingPermissionDescription, new GUIContent("Tracking Permission Description"));

                                            using (new EditorGUI.DisabledScope(!string.IsNullOrEmpty(iosTrackingSettingsTrackingPermissionDescription.stringValue) || !iosTrackingSettingsRequestTrackingPermission.boolValue))
                                            {
                                                EditorGUILayout.HelpBox("If you are setting NSUserTrackingUsageDescription yourself, set the below checkbox to silence the warning about it being empty.", MessageType.Info);
                                                EditorGUILayout.PropertyField(iosTrackingSettingsTrackingPermissionDescriptionManaged, new GUIContent("Ignore Empty Tracking Description"));
                                            }

                                            EditorGUILayout.HelpBox("If you are using the Facebook Audience Network, the justtrack SDK can provide the permission of the advertiser id automatically to it after requesting it. This will also ensure the permission was provided (if given) before the IronSource SDK is initialized by the justtrack SDK (if configured).", MessageType.Info);
                                            FacebookAudienceNetworkIntegration facebookAudienceNetworkIntegration = (FacebookAudienceNetworkIntegration)EditorGUILayout.EnumPopup("FAN Integration", (FacebookAudienceNetworkIntegration)facebookAudienceNetworkIntegrationValues.GetValue(iosTrackingSettingsFacebookAudienceNetworkIntegration.enumValueIndex));
                                            int facebookAudienceNetworkIntegrationIndex = Array.IndexOf(facebookAudienceNetworkIntegrationValues, facebookAudienceNetworkIntegration);
                                            iosTrackingSettingsFacebookAudienceNetworkIntegration.enumValueIndex = facebookAudienceNetworkIntegrationIndex;

                                            switch (facebookAudienceNetworkIntegration)
                                            {
                                                case FacebookAudienceNetworkIntegration.NoIntegration:
                                                    if (Reflection.HasFacebookAudienceNetworkUnity())
                                                    {
                                                        EditorGUILayout.HelpBox("You have not enabled the automatic integration with the Unity version of the Facebook Audience Network, but it was deteted in your game.", MessageType.Warning);
                                                    }

                                                    break;
                                                case FacebookAudienceNetworkIntegration.UnityIntegration:
                                                    if (!Reflection.HasFacebookAudienceNetworkUnity())
                                                    {
                                                        EditorGUILayout.HelpBox("The automatic integration with the Unity version of the Facebook Audience Network might not work. No assembly was found containing the code of the FAN.", MessageType.Warning);
                                                    }

                                                    break;
                                                case FacebookAudienceNetworkIntegration.NativeIntegration:
                                                    if (Reflection.HasFacebookAudienceNetworkUnity())
                                                    {
                                                        EditorGUILayout.HelpBox("You have chosen to integrate with the native version of the Facebook Audience Network, but the Unity version of it was detected.", MessageType.Warning);
                                                    }

                                                    break;
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                    }

                    // Only show revenue tracking for Android and iOS (not for WebGL)
                    if (selectedApiTokenPlatform != 2)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("The justtrack SDK automatically tracks the revenue your users generate with in-app product and subscription purchases. Here you can configure whether the tracking should be active by default:", MessageType.Info);

                        bool enableAutomaticInAppPurchaseTracking = false;
                        SerializedProperty? disableAutomaticInAppPurchaseTracking = null;
                        EditorGUI.BeginChangeCheck();
                        switch (selectedApiTokenPlatform)
                        {
                            case 0:
                                enableAutomaticInAppPurchaseTracking = EditorGUILayout.Toggle("Android Automatic IAP tracking", !androidDisableAutomaticInAppPurchaseTracking.boolValue);
                                disableAutomaticInAppPurchaseTracking = androidDisableAutomaticInAppPurchaseTracking;
                                break;
                            case 1:
                                enableAutomaticInAppPurchaseTracking = EditorGUILayout.Toggle("iOS Automatic IAP tracking", !iosDisableAutomaticInAppPurchaseTracking.boolValue);
                                disableAutomaticInAppPurchaseTracking = iosDisableAutomaticInAppPurchaseTracking;
                                break;
                        }

                        if (EditorGUI.EndChangeCheck() && disableAutomaticInAppPurchaseTracking != null)
                        {
                            disableAutomaticInAppPurchaseTracking.boolValue = !enableAutomaticInAppPurchaseTracking;
                        }
                    }
                }
                }

                ///////////////////
                // DOCUMENTATION //
                ///////////////////

                if (!useRuntimeConstructor.boolValue)
                {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("For more information on setting up the justtrack SDK check out the relevant docs.", MessageType.None);
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("justtrack Android Docs", new GUILayoutOption[] { GUILayout.Width(150) }))
                {
                    Application.OpenURL("https://docs.justtrack.io/sdk/overview");
                }

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("justtrack iOS Docs", new GUILayoutOption[] { GUILayout.Width(150) }))
                {
                    Application.OpenURL("https://docs.justtrack.io/sdk/overview");
                }

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("justtrack Unity Docs", new GUILayoutOption[] { GUILayout.Width(150) }))
                {
                    Application.OpenURL("https://docs.justtrack.io/sdk/overview");
                }

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("justtrack WebGL Docs", new GUILayoutOption[] { GUILayout.Width(150) }))
                {
                    Application.OpenURL("https://docs.justtrack.io/sdk/overview");
                }

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("justtrack WebGL Docs", new GUILayoutOption[] { GUILayout.Width(150) }))
                {
                    Application.OpenURL("https://docs.justtrack.io/sdk/latest/web/");
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                } // end if (!useRuntimeConstructor) for docs

                //////////////////
                // INTEGRATIONS //
                //////////////////

                // Only show integrations for Android and iOS (not for WebGL)
                if (selectedApiTokenPlatform != 2)
                {
                    EditorGUILayout.Space();
                    integrationsFoldout = EditorGUILayout.Foldout(integrationsFoldout, new GUIContent("justtrack Ad Network Integrations"), true);
                    if (integrationsFoldout)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("You can integrate the justtrack SDK with the SDKs of different ad networks via dedicated adapters.\n\nYou don't have to enable these integrations if you are only using these networks via a mediation provider like IronSource.", MessageType.Info);
                        EditorGUILayout.Space();

                        GUILayout.BeginHorizontal();

                        GUILayout.BeginVertical();
                        EditorGUILayout.LabelField("Android:");
                        EditorGUILayout.PropertyField(androidAppLovinIntegration, new GUIContent("Add AppLovin Adapter"));
                        EditorGUILayout.PropertyField(androidFirebaseIntegration, new GUIContent("Add Firebase Adapter"));
                        EditorGUILayout.PropertyField(androidIronSourceIntegration, new GUIContent("Add IronSource Adapter"));
                        EditorGUILayout.PropertyField(androidUnityAdsIntegration, new GUIContent("Add Unity Ads Adapter"));
                        GUILayout.EndVertical();

                        GUILayout.FlexibleSpace();

                        GUILayout.BeginVertical();
                        EditorGUILayout.LabelField("iOS:");
                        EditorGUILayout.PropertyField(iosAppLovinIntegration, new GUIContent("Add AppLovin Adapter"));
                        EditorGUILayout.PropertyField(iosFirebaseIntegration, new GUIContent("Add Firebase Adapter"));
                        EditorGUILayout.PropertyField(iosGoogleOdmIntegration, new GUIContent("Add Google ODM Adapter"));
                        EditorGUILayout.PropertyField(iosIronSourceIntegration, new GUIContent("Add IronSource Adapter"));
                        EditorGUILayout.PropertyField(iosUnityAdsIntegration, new GUIContent("Add Unity Ads Adapter"));
                        GUILayout.EndVertical();

                        GUILayout.EndHorizontal();
                    }
                }

                ////////////////////////
                // CONNECTION TRACKING //
                ////////////////////////

                if (!useRuntimeConstructor.boolValue && (selectedApiTokenPlatform == 0 || selectedApiTokenPlatform == 1))
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Connection tracking attaches a connection type dimension to every tracked event.", MessageType.Info);
                    EditorGUILayout.PropertyField(enableConnectionTracking, new GUIContent("Enable Connection Tracking"));
                }

                /////////////////////
                // CONSOLE LOGGING //
                /////////////////////

                if (!useRuntimeConstructor.boolValue)
                {
                EditorGUILayout.Space();
                loggingFoldout = EditorGUILayout.Foldout(loggingFoldout, new GUIContent("justtrack Console Logging"), true);
                if (loggingFoldout)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("You can enable console logging of the justtrack SDK to help with debugging issues.", MessageType.Info);
                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(enableConsoleLogging, new GUIContent("Enable Console Logging"));
                }
                } // end if (!useRuntimeConstructor) for logging

                // Only show regenerate dependencies for Android and iOS (not for WebGL)
                if (selectedApiTokenPlatform != 2)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("If you changed files outside of this inspector or something seems out of sync, you can manually regenerate the dependency file.", MessageType.None);
                    if (GUILayout.Button("Regenerate JustTrack Dependencies"))
                    {
                        JustTrackCodeGenerator.GenerateDependencyFile();
                    }
                }

                if (serializedObject.ApplyModifiedProperties())
                {
                    JustTrackCodeGenerator.GenerateDependencyFile();
                }

                EditorGUIUtility.labelWidth = previousLabelWidth;
            }
        }

        /// <summary>
        /// Renders a box with an image for the justtrack SDK editor.
        /// </summary>
        /// <param name="image">The image file name to display.</param>
        /// <param name="width">The width of the box.</param>
        /// <param name="height">The height of the box.</param>
        internal static void Box(string image, int width, int height)
        {
            var path = "Packages/io.justtrack.justtrack-unity-sdk/Editor/" + image;
            var fallback = "Assets/JustTrack/Editor/" + image;

            if (File.Exists(path))
            {
                GUILayout.Box((Texture)AssetDatabase.LoadAssetAtPath(path, typeof(Texture)), new GUILayoutOption[] { GUILayout.Width(width), GUILayout.Height(height) });
            }
            else
            {
                GUILayout.Box((Texture)AssetDatabase.LoadAssetAtPath(fallback, typeof(Texture)), new GUILayoutOption[] { GUILayout.Width(width), GUILayout.Height(height) });
            }
        }
    }
}
