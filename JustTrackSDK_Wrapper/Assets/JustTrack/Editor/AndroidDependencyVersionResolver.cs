using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Provides utilities for resolving Android dependency versions from various sources.
/// </summary>
internal static class AndroidDependencyVersionResolver
{
    /// <summary>
    /// Gets the version of a specific Android dependency from project configuration files.
    /// </summary>
    /// <param name="dependencyPackage">The package name to search for.</param>
    /// <returns>The version string if found; otherwise, null.</returns>
    public static string? GetAndroidDependencyVersion(string dependencyPackage)
    {
        // If the dependency is added by EDM4U
        foreach (var path in SafeEnumerateFiles("Assets", "*Dependencies*.xml"))
        {
            try
            {
                var xdoc = XDocument.Load(path);
                foreach (var package in xdoc.Descendants("androidPackage"))
                {
                    var attribute = (string)package.Attribute("spec");
                    if (string.IsNullOrEmpty(attribute))
                    {
                        continue;
                    }

                    if (attribute.StartsWith($"{dependencyPackage}:", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = attribute.Split(':');
                        if (parts.Length >= 3)
                        {
                            return parts[2].Trim();
                        }
                    }
                }
            }
            catch
            { /* ignored */
            }
        }

        // If the dependency is added by Asset/Manually
        var gradlePatterns = new[] { "*.gradle", "*.gradle.kts" };
        foreach (var pattern in gradlePatterns)
        {
            foreach (var file in SafeEnumerateFiles("Assets", pattern))
            {
                try
                {
                    var text = File.ReadAllText(file);
                    var rx = new Regex($@"['""]{Regex.Escape(dependencyPackage)}:([^'""\s)]+)['""]");
                    var m = rx.Match(text);
                    if (m.Success)
                    {
                        return m.Groups[1].Value.Trim();
                    }
                }
                catch
                {
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Extracts the major version number from various version specification formats.
    /// </summary>
    /// <param name="version">The version specification.</param>
    /// <returns>The major version number if parseable; otherwise, null.</returns>
    public static int? ParseMajorFromVersionSpec(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            return null;
        }

        // ">=13.0.0" => 13
        var ge = Regex.Match(version, @"^>=\s*(\d+)\.");
        if (ge.Success)
        {
            return int.Parse(ge.Groups[1].Value);
        }

        // "13.+" or "13.x" => 13
        var dyn = Regex.Match(version, @"^(\d+)\s*\.\s*(\+|x)$");
        if (dyn.Success)
        {
            return int.Parse(dyn.Groups[1].Value);
        }

        // Maven range "[12.0.0,12.10.0[" => 12
        var range = Regex.Match(version, @"^[\[\(]\s*(\d+)\.");
        if (range.Success)
        {
            return int.Parse(range.Groups[1].Value);
        }

        // "13.2.1" or "13.2" => 13
        var exact = Regex.Match(version, @"^(\d+)\.");
        if (exact.Success)
        {
            return int.Parse(exact.Groups[1].Value);
        }

        // "13" => 13
        var majorOnly = Regex.Match(version, @"^(\d+)$");
        if (majorOnly.Success)
        {
            return int.Parse(majorOnly.Groups[1].Value);
        }

        return null;
    }

    private static IEnumerable<string> SafeEnumerateFiles(string root, string pattern)
    {
        try
        {
            return Directory.GetFiles(root, pattern, SearchOption.AllDirectories);
        }
        catch
        {
            return Enumerable.Empty<string>();
        }
    }

    /// <summary>
    /// Resolves the appropriate adapter version based on the dependency configuration and versioning rules.
    /// </summary>
    /// <param name="dependencyName">The name of the dependency adapter.</param>
    /// <param name="dependencyPackage">The package identifier for the dependency.</param>
    /// <param name="defaultVersion">The default version to use if resolution fails.</param>
    /// <returns>The resolved adapter version or the default version if resolution fails.</returns>
    public static string ResolveAdapterVersionFromConfig(string dependencyName, string dependencyPackage, string defaultVersion)
    {
        Debug.Log($"[JustTrack] Resolving adapter version for {dependencyName} ({dependencyPackage})");
        string? dependencyVersion = GetAndroidDependencyVersion(dependencyPackage);

        if (dependencyVersion == null)
        {
            Debug.LogWarning($"[JustTrack] Unable to retrieve dependency version for {dependencyPackage}. Using default adapter version '{defaultVersion}'.");
            return defaultVersion;
        }

        const string adapterVersioningJsonPath = "Packages/io.justtrack.justtrack-unity-sdk/Editor/adapter_dynamic_versioning.json";
        var adapterVersioningJsonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(adapterVersioningJsonPath);
        if (!adapterVersioningJsonAsset)
        {
            Debug.LogWarning($"[JustTrack] Not found at {adapterVersioningJsonPath}. Check your package name in package.json and that the JSON is in Editor/.");
            return defaultVersion;
        }

        var adapterJson = adapterVersioningJsonAsset.text;

        try
        {
            var adapterVersionConfig = JsonConvert.DeserializeObject<AdapterDynamicVersioning>(adapterJson);
            if (adapterVersionConfig?.android == null)
            {
                return defaultVersion;
            }

            if (!adapterVersionConfig.android.TryGetValue(dependencyName, out var rules) || rules == null || rules.Count == 0)
            {
                return defaultVersion;
            }

            var actualMajorVersion = ParseMajorFromVersionSpec(dependencyVersion);
            foreach (var rule in rules)
            {
                var req = rule.ThirdpartyVersionRequirement;
                if (string.IsNullOrEmpty(req) || req == "*")
                {
                    return rule.Version;
                }

                // Major-only dynamic: "13.+" or "13.x"
                if (Regex.IsMatch(req, @"^\s*\d+\s*\.\s*(\+|x)\s*$"))
                {
                    var reqMajor = ParseMajorFromVersionSpec(req);
                    if (actualMajorVersion.HasValue && reqMajor.HasValue && actualMajorVersion.Value == reqMajor.Value)
                    {
                        return rule.Version;
                    }

                    continue;
                }

                // Exact or partial version like "13.2.1" or "13.2": match by major equality
                if (Regex.IsMatch(req, @"^\s*\d+(\.\d+){0,2}\s*$"))
                {
                    var reqMajor = ParseMajorFromVersionSpec(req);
                    if (actualMajorVersion.HasValue && reqMajor.HasValue && actualMajorVersion.Value == reqMajor.Value)
                    {
                        return rule.Version;
                    }

                    continue;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[JustTrack] Error ResolveAdapterVersionFromConfig: {ex}");
        }

        // fallback to version if can not retreive version.
        return defaultVersion;
    }

    /// <summary>
    /// Represents a versioning rule for adapter dependencies.
    /// </summary>
#pragma warning disable SA1401, SA1307 // Fields should be private; field names should begin with upper-case letter (required for JSON serialization)
    [Serializable]
    internal class AdapterRule
    {
        /// <summary>
        /// Gets or sets the third-party version requirement pattern.
        /// </summary>
        public string ThirdpartyVersionRequirement = string.Empty;

        /// <summary>
        /// Gets or sets the adapter version to use when the requirement is met.
        /// </summary>
        public string Version = string.Empty;
    }

    /// <summary>
    /// Represents the dynamic versioning configuration for adapters.
    /// </summary>
    [Serializable]
    internal class AdapterDynamicVersioning
    {
        /// <summary>
        /// Gets or sets the Android adapter versioning rules mapped by dependency name.
        /// </summary>
        public Dictionary<string, List<AdapterRule>> android = new Dictionary<string, List<AdapterRule>>();
    }
#pragma warning restore SA1401, SA1307
}
