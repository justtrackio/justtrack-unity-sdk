using NUnit.Framework;
using JustTrack;
using System.Collections.Generic;

namespace JustTrack.Tests.Editor
{
    /// <summary>
    /// Unit tests for the JustTrackUtils class.
    /// Tests internal utility methods for the justtrack SDK.
    /// </summary>
    public class JustTrackUtilsTests
    {
        [Test]
        public void DetectExternalDependencyManager_ReturnsBoolean()
        {
            // Act
            var result = JustTrackUtils.DetectExternalDependencyManager();

            // Assert
            // The result can be true or false depending on the environment
            // We just verify it returns without throwing
            Assert.That(result, Is.TypeOf<bool>());
        }

        [Test]
        public void IsIL2CPP_Android_ReturnsBoolean()
        {
            // Act
            var result = JustTrackUtils.IsIL2CPP(isAndroid: true);

            // Assert
            Assert.That(result, Is.TypeOf<bool>());
        }

        [Test]
        public void IsIL2CPP_iOS_ReturnsBoolean()
        {
            // Act
            var result = JustTrackUtils.IsIL2CPP(isAndroid: false);

            // Assert
            Assert.That(result, Is.TypeOf<bool>());
        }

        [Test]
        public void GetFacebookAudienceNetworkIntegration_EmptyAndroidToken_ReturnsNoIntegration()
        {
            // Arrange
            var settings = UnityEngine.ScriptableObject.CreateInstance<JustTrackSettings>();
            settings.AndroidApiToken = "";
            settings.IosApiToken = "";

            // Act
            var result = JustTrackUtils.GetFacebookAudienceNetworkIntegration(settings);

            // Assert
            Assert.AreEqual(FacebookAudienceNetworkIntegration.NoIntegration, result);
        }

        [Test]
        public void GetFacebookAudienceNetworkIntegration_EmptyiOSToken_ReturnsNoIntegration()
        {
            // Arrange
            var settings = UnityEngine.ScriptableObject.CreateInstance<JustTrackSettings>();
            settings.AndroidApiToken = "test-token";
            settings.IosApiToken = "";

            // Act
            var result = JustTrackUtils.GetFacebookAudienceNetworkIntegration(settings);

            // Assert
            Assert.AreEqual(FacebookAudienceNetworkIntegration.NoIntegration, result);
        }

        [Test]
        public void GetFacebookAudienceNetworkIntegration_WithiOSToken_ReturnsConfiguredIntegration()
        {
            // Arrange
            var settings = UnityEngine.ScriptableObject.CreateInstance<JustTrackSettings>();
            settings.IosApiToken = "test-token";
            settings.IosTrackingSettings = new iOSTrackingSettings
            {
                FacebookAudienceNetworkIntegration = FacebookAudienceNetworkIntegration.UnityIntegration
            };

            // Act
            var result = JustTrackUtils.GetFacebookAudienceNetworkIntegration(settings);

            // Assert
            Assert.AreEqual(FacebookAudienceNetworkIntegration.UnityIntegration, result);
        }

        [Test]
        public void GetFacebookAudienceNetworkIntegration_WithiOSToken_NativeIntegration_ReturnsNativeIntegration()
        {
            // Arrange
            var settings = UnityEngine.ScriptableObject.CreateInstance<JustTrackSettings>();
            settings.IosApiToken = "test-token";
            settings.IosTrackingSettings = new iOSTrackingSettings
            {
                FacebookAudienceNetworkIntegration = FacebookAudienceNetworkIntegration.NativeIntegration
            };

            // Act
            var result = JustTrackUtils.GetFacebookAudienceNetworkIntegration(settings);

            // Assert
            Assert.AreEqual(FacebookAudienceNetworkIntegration.NativeIntegration, result);
        }

        [Test]
        public void VerificationResults_CacheDictionary_IsNotNull()
        {
            // Arrange & Act
            var cache = JustTrackUtils.VerificationResults;

            // Assert
            Assert.IsNotNull(cache);
            Assert.That(cache, Is.TypeOf<Dictionary<string, VerificationResultData>>());
        }

        [Test]
        public void EDM_DOWNLOAD_PAGE_IsValidUrl()
        {
            // Act
            var url = JustTrackUtils.EdmDownloadPage;

            // Assert
            Assert.IsNotEmpty(url);
            Assert.That(url, Does.StartWith("https://"));
            Assert.That(url, Does.Contain("developers.google.com"));
        }

        [Test]
        public void EDM_PACKAGE_URL_IsValidUrl()
        {
            // Act
            var url = JustTrackUtils.EdmPackageUrl;

            // Assert
            Assert.IsNotEmpty(url);
            Assert.That(url, Does.StartWith("https://"));
            Assert.That(url, Does.Contain("github.com"));
            Assert.That(url, Does.EndWith(".unitypackage"));
        }

        [Test]
        public void ValidationResult_StructHasWarningsAndErrors()
        {
            // Arrange
            var validationResult = new JustTrackUtils.ValidationResult
            {
                Warnings = new List<string> { "Warning 1", "Warning 2" },
                Errors = new List<string> { "Error 1" }
            };

            // Assert
            Assert.IsNotNull(validationResult.Warnings);
            Assert.IsNotNull(validationResult.Errors);
            Assert.AreEqual(2, validationResult.Warnings.Count);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.AreEqual("Warning 1", validationResult.Warnings[0]);
            Assert.AreEqual("Error 1", validationResult.Errors[0]);
        }

        [Test]
        public void ValidationMode_HasThreeValues()
        {
            // Arrange & Act
            var androidMode = JustTrackUtils.ValidationMode.ValidateAndroid;
            var iosMode = JustTrackUtils.ValidationMode.ValidateIOS;
            var allMode = JustTrackUtils.ValidationMode.ValidateAll;

            // Assert
            Assert.AreNotEqual(androidMode, iosMode);
            Assert.AreNotEqual(androidMode, allMode);
            Assert.AreNotEqual(iosMode, allMode);
        }
    }
}
