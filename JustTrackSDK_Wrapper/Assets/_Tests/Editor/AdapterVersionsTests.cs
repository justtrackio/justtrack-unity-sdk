using NUnit.Framework;
using JustTrack;
using System.Text.RegularExpressions;

namespace JustTrack.Tests.Editor
{
    /// <summary>
    /// Unit tests for the AdapterVersions class.
    /// Tests all adapter version constants to ensure they are valid.
    /// </summary>
    public class AdapterVersionsTests
    {
        #region Android Adapter Version Tests

        [Test]
        public void AndroidApplovin_IsNotEmpty()
        {
            // Assert
            Assert.IsNotNull(AdapterVersions.AndroidApplovin);
            Assert.IsNotEmpty(AdapterVersions.AndroidApplovin);
        }

        [Test]
        public void AndroidFirebase_IsNotEmpty()
        {
            // Assert
            Assert.IsNotNull(AdapterVersions.AndroidFirebase);
            Assert.IsNotEmpty(AdapterVersions.AndroidFirebase);
        }

        [Test]
        public void AndroidIronsource_IsNotEmpty()
        {
            // Assert
            Assert.IsNotNull(AdapterVersions.AndroidIronsource);
            Assert.IsNotEmpty(AdapterVersions.AndroidIronsource);
        }

        [Test]
        public void AndroidUnityads_IsNotEmpty()
        {
            // Assert
            Assert.IsNotNull(AdapterVersions.AndroidUnityads);
            Assert.IsNotEmpty(AdapterVersions.AndroidUnityads);
        }

        #endregion

        #region iOS Adapter Version Tests

        [Test]
        public void IosApplovin_IsNotEmpty()
        {
            // Assert
            Assert.IsNotNull(AdapterVersions.IosApplovin);
            Assert.IsNotEmpty(AdapterVersions.IosApplovin);
        }

        [Test]
        public void IosFirebase_IsNotEmpty()
        {
            // Assert
            Assert.IsNotNull(AdapterVersions.IosFirebase);
            Assert.IsNotEmpty(AdapterVersions.IosFirebase);
        }

        [Test]
        public void IosIronsource_IsNotEmpty()
        {
            // Assert
            Assert.IsNotNull(AdapterVersions.IosIronsource);
            Assert.IsNotEmpty(AdapterVersions.IosIronsource);
        }

        [Test]
        public void IosUnityads_IsNotEmpty()
        {
            // Assert
            Assert.IsNotNull(AdapterVersions.IosUnityads);
            Assert.IsNotEmpty(AdapterVersions.IosUnityads);
        }

        [Test]
        public void IosGoogleodm_IsNotEmpty()
        {
            // Assert
            Assert.IsNotNull(AdapterVersions.IosGoogleodm);
            Assert.IsNotEmpty(AdapterVersions.IosGoogleodm);
        }

        #endregion

        #region Version Format Tests

        [Test]
        public void AndroidApplovin_MatchesSemanticVersionPattern()
        {
            // Assert
            Assert.That(AdapterVersions.AndroidApplovin, Does.Match(@"^\d+\.\d+\.\d+$"),
                $"AndroidApplovin '{AdapterVersions.AndroidApplovin}' should match semantic version pattern (X.Y.Z)");
        }

        [Test]
        public void AndroidFirebase_MatchesSemanticVersionPattern()
        {
            // Assert
            Assert.That(AdapterVersions.AndroidFirebase, Does.Match(@"^\d+\.\d+\.\d+$"),
                $"AndroidFirebase '{AdapterVersions.AndroidFirebase}' should match semantic version pattern (X.Y.Z)");
        }

        [Test]
        public void AndroidIronsource_MatchesSemanticVersionPattern()
        {
            // Assert
            Assert.That(AdapterVersions.AndroidIronsource, Does.Match(@"^\d+\.\d+\.\d+$"),
                $"AndroidIronsource '{AdapterVersions.AndroidIronsource}' should match semantic version pattern (X.Y.Z)");
        }

        [Test]
        public void AndroidUnityads_MatchesSemanticVersionPattern()
        {
            // Assert
            Assert.That(AdapterVersions.AndroidUnityads, Does.Match(@"^\d+\.\d+\.\d+$"),
                $"AndroidUnityads '{AdapterVersions.AndroidUnityads}' should match semantic version pattern (X.Y.Z)");
        }

        [Test]
        public void IosApplovin_MatchesSemanticVersionPattern()
        {
            // Assert
            Assert.That(AdapterVersions.IosApplovin, Does.Match(@"^\d+\.\d+\.\d+$"),
                $"IosApplovin '{AdapterVersions.IosApplovin}' should match semantic version pattern (X.Y.Z)");
        }

        [Test]
        public void IosFirebase_MatchesSemanticVersionPattern()
        {
            // Assert
            Assert.That(AdapterVersions.IosFirebase, Does.Match(@"^\d+\.\d+\.\d+$"),
                $"IosFirebase '{AdapterVersions.IosFirebase}' should match semantic version pattern (X.Y.Z)");
        }

        [Test]
        public void IosIronsource_MatchesSemanticVersionPattern()
        {
            // Assert
            Assert.That(AdapterVersions.IosIronsource, Does.Match(@"^\d+\.\d+\.\d+$"),
                $"IosIronsource '{AdapterVersions.IosIronsource}' should match semantic version pattern (X.Y.Z)");
        }

        [Test]
        public void IosUnityads_MatchesSemanticVersionPattern()
        {
            // Assert
            Assert.That(AdapterVersions.IosUnityads, Does.Match(@"^\d+\.\d+\.\d+$"),
                $"IosUnityads '{AdapterVersions.IosUnityads}' should match semantic version pattern (X.Y.Z)");
        }

        [Test]
        public void IosGoogleodm_MatchesSemanticVersionPattern()
        {
            // Assert
            Assert.That(AdapterVersions.IosGoogleodm, Does.Match(@"^\d+\.\d+\.\d+$"),
                $"IosGoogleodm '{AdapterVersions.IosGoogleodm}' should match semantic version pattern (X.Y.Z)");
        }

        #endregion

        #region Cross-Platform Consistency Tests

        [Test]
        public void AllAndroidVersions_AreValid()
        {
            // Arrange
            var androidVersions = new[]
            {
                AdapterVersions.AndroidApplovin,
                AdapterVersions.AndroidFirebase,
                AdapterVersions.AndroidIronsource,
                AdapterVersions.AndroidUnityads
            };

            // Act & Assert
            foreach (var version in androidVersions)
            {
                Assert.IsNotEmpty(version);
                Assert.That(version, Does.Match(@"^\d+\.\d+\.\d+$"));
            }
        }

        [Test]
        public void AllIOSVersions_AreValid()
        {
            // Arrange
            var iosVersions = new[]
            {
                AdapterVersions.IosApplovin,
                AdapterVersions.IosFirebase,
                AdapterVersions.IosGoogleodm,
                AdapterVersions.IosIronsource,
                AdapterVersions.IosUnityads
            };

            // Act & Assert
            foreach (var version in iosVersions)
            {
                Assert.IsNotEmpty(version);
                Assert.That(version, Does.Match(@"^\d+\.\d+\.\d+$"));
            }
        }

        #endregion
    }
}
