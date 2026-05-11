#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace JustTrack
{
    /// <summary>
    /// Post-build processor that automatically integrates the justtrack Browser SDK into WebGL builds.
    /// This script copies the SDK JavaScript file and modifies the index.html to include it.
    /// </summary>
    public class WebGLPostProcessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.WebGL)
            {
                return;
            }

            string buildPath = report.summary.outputPath;
            Debug.Log($"[justtrack] Post-processing WebGL build at: {buildPath}");

            try
            {
                CopySDKToBuild(buildPath);

                ModifyIndexHtml(buildPath);

                Debug.Log("[justtrack] WebGL build post-processing completed successfully");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[justtrack] Failed to post-process WebGL build: {e.Message}");
                Debug.LogException(e);
            }
        }

        private void CopySDKToBuild(string buildPath)
        {
            string sdkSourcePath = Path.Combine(Application.dataPath, "JustTrack", "Plugins", "WebGL", "justtrack-sdk.umd.js");

            if (!File.Exists(sdkSourcePath))
            {
                string packagePath = Path.GetFullPath(Path.Combine("Packages", "io.justtrack.justtrack-unity-sdk", "Plugins", "WebGL", "justtrack-sdk.umd.js"));

                if (File.Exists(packagePath))
                {
                    sdkSourcePath = packagePath;
                }
                else
                {
                    throw new FileNotFoundException(
                        $"JustTrack SDK JavaScript file not found. Searched locations:\n" +
                        $"1. {sdkSourcePath}\n" +
                        $"2. {packagePath}\n" +
                        $"Please ensure the SDK is properly installed.");
                }
            }

            string sdkDestPath = Path.Combine(buildPath, "justtrack-sdk.umd.js");

            Debug.Log($"[justtrack] Copying SDK from {sdkSourcePath} to {sdkDestPath}");
            File.Copy(sdkSourcePath, sdkDestPath, true);
        }

        private void ModifyIndexHtml(string buildPath)
        {
            string indexPath = Path.Combine(buildPath, "index.html");

            if (!File.Exists(indexPath))
            {
                throw new FileNotFoundException($"index.html not found at: {indexPath}");
            }

            Debug.Log($"[justtrack] Modifying index.html at: {indexPath}");

            string htmlContent = File.ReadAllText(indexPath);

            if (htmlContent.Contains("justtrack-sdk.umd.js"))
            {
                Debug.Log("[justtrack] SDK script already included in index.html");
                return;
            }

            string sdkScriptTag = "  <script src=\"justtrack-sdk.umd.js\"></script>\n";

            if (htmlContent.Contains("</head>"))
            {
                htmlContent = htmlContent.Replace("</head>", sdkScriptTag + "</head>");
            }
            else if (htmlContent.Contains("</HEAD>"))
            {
                htmlContent = htmlContent.Replace("</HEAD>", sdkScriptTag + "</HEAD>");
            }
            else
            {
                Debug.LogWarning("[justtrack] Could not find </head> tag in index.html. Adding script at the beginning of the file.");
                htmlContent = sdkScriptTag + htmlContent;
            }

            File.WriteAllText(indexPath, htmlContent);
            Debug.Log("[justtrack] Successfully added SDK script to index.html");
        }
    }
}
#endif
