using NUnit.Framework;

namespace JustTrack.Tests.Editor
{
    /// <summary>
    /// Unit tests for the AndroidDependencyVersionResolver class.
    /// Tests version parsing logic from various specification formats.
    /// </summary>
    public class AndroidDependencyVersionResolverTests
    {
        #region ParseMajorFromVersionSpec Tests

        [Test]
        [TestCase(">=13.0.0", ExpectedResult = 13)]
        [TestCase(">=0.1.0", ExpectedResult = 0)]
        [TestCase(">=100.0.0", ExpectedResult = 100)]
        public int? ParseMajorFromVersionSpec_GreaterThanOrEqual_ReturnsMajorVersion(string version)
        {
            // Act
            return AndroidDependencyVersionResolver.ParseMajorFromVersionSpec(version);
        }

        [Test]
        [TestCase("13.+", ExpectedResult = 13)]
        [TestCase("0.+", ExpectedResult = 0)]
        [TestCase("999.+", ExpectedResult = 999)]
        public int? ParseMajorFromVersionSpec_DynamicPlusVersion_ReturnsMajorVersion(string version)
        {
            // Act
            return AndroidDependencyVersionResolver.ParseMajorFromVersionSpec(version);
        }

        [Test]
        [TestCase("13.x", ExpectedResult = 13)]
        [TestCase("0.x", ExpectedResult = 0)]
        [TestCase("42.x", ExpectedResult = 42)]
        public int? ParseMajorFromVersionSpec_DynamicXVersion_ReturnsMajorVersion(string version)
        {
            // Act
            return AndroidDependencyVersionResolver.ParseMajorFromVersionSpec(version);
        }

        [Test]
        [TestCase("[12.0.0,12.10.0[", ExpectedResult = 12)]
        [TestCase("[5.0.0,6.0.0)", ExpectedResult = 5)]
        [TestCase("(3.0.0,4.0.0]", ExpectedResult = 3)]
        [TestCase("(10.5.0,11.0.0[", ExpectedResult = 10)]
        public int? ParseMajorFromVersionSpec_MavenRanges_ReturnsMajorVersion(string version)
        {
            // Act
            return AndroidDependencyVersionResolver.ParseMajorFromVersionSpec(version);
        }

        [Test]
        [TestCase("13.2.1", ExpectedResult = 13)]
        [TestCase("0.0.1", ExpectedResult = 0)]
        [TestCase("42.7.9", ExpectedResult = 42)]
        public int? ParseMajorFromVersionSpec_ExactVersionThreePart_ReturnsMajorVersion(string version)
        {
            // Act
            return AndroidDependencyVersionResolver.ParseMajorFromVersionSpec(version);
        }

        [Test]
        [TestCase("13.2", ExpectedResult = 13)]
        [TestCase("0.1", ExpectedResult = 0)]
        [TestCase("99.0", ExpectedResult = 99)]
        public int? ParseMajorFromVersionSpec_ExactVersionTwoPart_ReturnsMajorVersion(string version)
        {
            // Act
            return AndroidDependencyVersionResolver.ParseMajorFromVersionSpec(version);
        }

        [Test]
        [TestCase("13", ExpectedResult = 13)]
        [TestCase("0", ExpectedResult = 0)]
        [TestCase("123", ExpectedResult = 123)]
        public int? ParseMajorFromVersionSpec_MajorVersionOnly_ReturnsMajorVersion(string version)
        {
            // Act
            return AndroidDependencyVersionResolver.ParseMajorFromVersionSpec(version);
        }

        [Test]
        [TestCase("", ExpectedResult = null)]
        [TestCase("   ", ExpectedResult = null)]
        [TestCase("\t", ExpectedResult = null)]
        [TestCase("\n", ExpectedResult = null)]
        public int? ParseMajorFromVersionSpec_EmptyOrWhitespace_ReturnsNull(string version)
        {
            // Act
            return AndroidDependencyVersionResolver.ParseMajorFromVersionSpec(version);
        }

        [Test]
        public void ParseMajorFromVersionSpec_Null_ReturnsNull()
        {
            // Act
            var result = AndroidDependencyVersionResolver.ParseMajorFromVersionSpec(null!);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCase("abc", ExpectedResult = null)]
        [TestCase("not.a.Version", ExpectedResult = null)]
        [TestCase("x.y.z", ExpectedResult = null)]
        [TestCase("latest", ExpectedResult = null)]
        public int? ParseMajorFromVersionSpec_InvalidFormats_ReturnsNull(string version)
        {
            // Act
            return AndroidDependencyVersionResolver.ParseMajorFromVersionSpec(version);
        }

        #endregion

        #region AdapterRule Tests

        [Test]
        public void AdapterRule_CanSetAndGetThirdpartyVersionRequirement()
        {
            // Arrange
            var rule = new AndroidDependencyVersionResolver.AdapterRule
            {
                ThirdpartyVersionRequirement = "13.+"
            };

            // Assert
            Assert.AreEqual("13.+", rule.ThirdpartyVersionRequirement);
        }

        [Test]
        public void AdapterRule_CanSetAndGetVersion()
        {
            // Arrange
            var rule = new AndroidDependencyVersionResolver.AdapterRule
            {
                Version = "2.0.0"
            };

            // Assert
            Assert.AreEqual("2.0.0", rule.Version);
        }

        [Test]
        public void AdapterRule_DefaultsToEmptyStrings()
        {
            // Arrange
            var rule = new AndroidDependencyVersionResolver.AdapterRule();

            // Assert
            Assert.AreEqual(string.Empty, rule.ThirdpartyVersionRequirement);
            Assert.AreEqual(string.Empty, rule.Version);
        }

        #endregion

        #region AdapterDynamicVersioning Tests

        [Test]
        public void AdapterDynamicVersioning_CanSetAndGetAndroidDictionary()
        {
            // Arrange
            var versioning = new AndroidDependencyVersionResolver.AdapterDynamicVersioning
            {
                android = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<AndroidDependencyVersionResolver.AdapterRule>>()
            };

            // Assert
            Assert.IsNotNull(versioning.android);
            Assert.IsEmpty(versioning.android);
        }

        [Test]
        public void AdapterDynamicVersioning_AndroidDictionary_CanAddEntries()
        {
            // Arrange
            var versioning = new AndroidDependencyVersionResolver.AdapterDynamicVersioning
            {
                android = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<AndroidDependencyVersionResolver.AdapterRule>>
                {
                    ["applovin"] = new System.Collections.Generic.List<AndroidDependencyVersionResolver.AdapterRule>
                    {
                        new AndroidDependencyVersionResolver.AdapterRule
                        {
                            ThirdpartyVersionRequirement = "13.+",
                            Version = "2.0.0"
                        }
                    }
                }
            };

            // Assert
            Assert.AreEqual(1, versioning.android.Count);
            Assert.IsTrue(versioning.android.ContainsKey("applovin"));
            Assert.AreEqual(1, versioning.android["applovin"].Count);
            Assert.AreEqual("13.+", versioning.android["applovin"][0].ThirdpartyVersionRequirement);
            Assert.AreEqual("2.0.0", versioning.android["applovin"][0].Version);
        }

        #endregion
    }
}
