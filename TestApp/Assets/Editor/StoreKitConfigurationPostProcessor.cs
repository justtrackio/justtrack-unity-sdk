#if UNITY_IOS
using System.IO;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

/// <summary>
/// Copies the local StoreKit configuration file (<c>TestApp/StoreKit/Products.storekit</c>)
/// into the generated Xcode project and wires it into the <c>Unity-iPhone</c> scheme so
/// developers can run the IAP scene locally without any App Store Connect configuration.
/// Runs automatically on every iOS build.
/// </summary>
public static class StoreKitConfigurationPostProcessor
{
    private const string StoreKitFileName = "Products.storekit";
    private const string SourceRelativePath = "StoreKit/Products.storekit";
    private const string XcodeGroupName = "StoreKit";
    private const string SchemeName = "Unity-iPhone";

    [PostProcessBuild(100)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        string sourcePath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", SourceRelativePath));
        if (!File.Exists(sourcePath))
        {
            Debug.LogWarning($"[StoreKitConfigurationPostProcessor] Source StoreKit file not found at {sourcePath}. Skipping.");
            return;
        }

        string destinationPath = Path.Combine(buildPath, StoreKitFileName);
        try
        {
            File.Copy(sourcePath, destinationPath, overwrite: true);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[StoreKitConfigurationPostProcessor] Failed to copy StoreKit file: {e}");
            return;
        }

        AddStoreKitFileToXcodeProject(buildPath);
        AddStoreKitConfigurationToScheme(buildPath);
    }

    private static void AddStoreKitFileToXcodeProject(string buildPath)
    {
        string projectPath = PBXProject.GetPBXProjectPath(buildPath);
        if (!File.Exists(projectPath))
        {
            Debug.LogWarning($"[StoreKitConfigurationPostProcessor] Xcode project not found at {projectPath}.");
            return;
        }

        var project = new PBXProject();
        project.ReadFromFile(projectPath);

        // Add the file only as a project reference (not to any build phase). StoreKit
        // configuration files must not be packaged into the app bundle - they are only
        // consumed by Xcode when running the scheme.
        string fileGuid = project.AddFile(StoreKitFileName, XcodeGroupName + "/" + StoreKitFileName, PBXSourceTree.Source);
        if (string.IsNullOrEmpty(fileGuid))
        {
            Debug.LogWarning("[StoreKitConfigurationPostProcessor] Could not add StoreKit file to Xcode project.");
            return;
        }

        project.WriteToFile(projectPath);
    }

    private static void AddStoreKitConfigurationToScheme(string buildPath)
    {
        string schemePath = Path.Combine(
            buildPath,
            SchemeName + ".xcodeproj",
            "xcshareddata",
            "xcschemes",
            SchemeName + ".xcscheme");

        if (!File.Exists(schemePath))
        {
            Debug.LogWarning($"[StoreKitConfigurationPostProcessor] Scheme file not found at {schemePath}. " +
                "StoreKit file was copied but not wired into the scheme. You can select it manually in " +
                "Xcode via Product > Scheme > Edit Scheme > Run > Options > StoreKit Configuration.");
            return;
        }

        try
        {
            XDocument scheme = XDocument.Load(schemePath);
            XElement launchAction = scheme.Root?.Element("LaunchAction");
            if (launchAction == null)
            {
                Debug.LogWarning("[StoreKitConfigurationPostProcessor] LaunchAction not found in scheme.");
                return;
            }

            // Remove any previously-injected StoreKitConfigurationFileReference so we don't duplicate on rebuild.
            launchAction.Elements("StoreKitConfigurationFileReference").Remove();

            var storeKitReference = new XElement("StoreKitConfigurationFileReference",
                new XAttribute("identifier", "../../" + StoreKitFileName));

            launchAction.Add(storeKitReference);

            scheme.Save(schemePath);
            Debug.Log($"[StoreKitConfigurationPostProcessor] Wired {StoreKitFileName} into the {SchemeName} scheme.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[StoreKitConfigurationPostProcessor] Failed to modify scheme: {e}. " +
                "You can select the StoreKit file manually in Xcode via Product > Scheme > Edit Scheme > Run > Options > StoreKit Configuration.");
        }
    }
}
#endif
