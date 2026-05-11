using NUnit.Framework;
using JustTrack;
using System.Collections.Generic;

namespace JustTrack.Tests.Editor
{
    /// <summary>
    /// Unit tests for the JustTrackCodeGenerator class.
    /// Tests data classes and public methods related to code generation.
    /// </summary>
    public class JustTrackCodeGeneratorTests
    {
        #region HasFBAudienceNetworkAdpt Tests

        [Test]
        public void HasFBAudienceNetworkAdpt_ReturnsBoolean()
        {
            // Act
            var result = JustTrackCodeGenerator.HasFBAudienceNetworkAdpt();

            // Assert
            Assert.That(result, Is.TypeOf<bool>());
        }

        #endregion

        #region Dependencies.ScopedRegistry Tests

        [Test]
        public void ScopedRegistry_CanSetAndGetName()
        {
            // Arrange
            var registry = new Dependencies.ScopedRegistry
            {
                Name = "TestRegistry"
            };

            // Assert
            Assert.AreEqual("TestRegistry", registry.Name);
        }

        [Test]
        public void ScopedRegistry_CanSetAndGetUrl()
        {
            // Arrange
            var registry = new Dependencies.ScopedRegistry
            {
                Url = "https://registry.example.com"
            };

            // Assert
            Assert.AreEqual("https://registry.example.com", registry.Url);
        }

        [Test]
        public void ScopedRegistry_CanSetAndGetScopes()
        {
            // Arrange
            var scopes = new[] { "com.example", "com.test" };
            var registry = new Dependencies.ScopedRegistry
            {
                Scopes = scopes
            };

            // Assert
            Assert.AreSame(scopes, registry.Scopes);
            Assert.AreEqual(2, registry.Scopes.Length);
            Assert.AreEqual("com.example", registry.Scopes[0]);
            Assert.AreEqual("com.test", registry.Scopes[1]);
        }

        [Test]
        public void ScopedRegistry_AllFieldsCanBeSet()
        {
            // Arrange & Act
            var registry = new Dependencies.ScopedRegistry
            {
                Name = "MyRegistry",
                Url = "https://npm.example.com",
                Scopes = new[] { "com.mycompany" }
            };

            // Assert
            Assert.AreEqual("MyRegistry", registry.Name);
            Assert.AreEqual("https://npm.example.com", registry.Url);
            Assert.AreEqual(1, registry.Scopes.Length);
            Assert.AreEqual("com.mycompany", registry.Scopes[0]);
        }

        #endregion

        #region Dependencies.ManifestJson Tests

        [Test]
        public void ManifestJson_Dependencies_DefaultsToEmptyDictionary()
        {
            // Arrange & Act
            var manifest = new Dependencies.ManifestJson();

            // Assert
            Assert.IsNotNull(manifest.dependencies);
            Assert.IsEmpty(manifest.dependencies);
            Assert.That(manifest.dependencies, Is.TypeOf<Dictionary<string, string>>());
        }

        [Test]
        public void ManifestJson_ScopedRegistries_DefaultsToEmptyList()
        {
            // Arrange & Act
            var manifest = new Dependencies.ManifestJson();

            // Assert
            Assert.IsNotNull(manifest.scopedRegistries);
            Assert.IsEmpty(manifest.scopedRegistries);
            Assert.That(manifest.scopedRegistries, Is.TypeOf<List<Dependencies.ScopedRegistry>>());
        }

        [Test]
        public void ManifestJson_Dependencies_CanAddEntries()
        {
            // Arrange
            var manifest = new Dependencies.ManifestJson();

            // Act
            manifest.dependencies["com.unity.test"] = "1.0.0";
            manifest.dependencies["com.unity.another"] = "2.0.0";

            // Assert
            Assert.AreEqual(2, manifest.dependencies.Count);
            Assert.AreEqual("1.0.0", manifest.dependencies["com.unity.test"]);
            Assert.AreEqual("2.0.0", manifest.dependencies["com.unity.another"]);
        }

        [Test]
        public void ManifestJson_ScopedRegistries_CanAddEntries()
        {
            // Arrange
            var manifest = new Dependencies.ManifestJson();
            var registry1 = new Dependencies.ScopedRegistry
            {
                Name = "Registry1",
                Url = "https://example1.com",
                Scopes = new[] { "com.example1" }
            };
            var registry2 = new Dependencies.ScopedRegistry
            {
                Name = "Registry2",
                Url = "https://example2.com",
                Scopes = new[] { "com.example2" }
            };

            // Act
            manifest.scopedRegistries.Add(registry1);
            manifest.scopedRegistries.Add(registry2);

            // Assert
            Assert.AreEqual(2, manifest.scopedRegistries.Count);
            Assert.AreEqual("Registry1", manifest.scopedRegistries[0].Name);
            Assert.AreEqual("Registry2", manifest.scopedRegistries[1].Name);
        }

        [Test]
        public void ManifestJson_CanBeFullyPopulated()
        {
            // Arrange & Act
            var manifest = new Dependencies.ManifestJson
            {
                dependencies = new Dictionary<string, string>
                {
                    ["com.unity.test"] = "1.0.0",
                    ["com.unity.another"] = "2.0.0"
                },
                scopedRegistries = new List<Dependencies.ScopedRegistry>
                {
                    new Dependencies.ScopedRegistry
                    {
                        Name = "TestRegistry",
                        Url = "https://npm.test.com",
                        Scopes = new[] { "com.test" }
                    }
                }
            };

            // Assert
            Assert.AreEqual(2, manifest.dependencies.Count);
            Assert.AreEqual(1, manifest.scopedRegistries.Count);
            Assert.AreEqual("1.0.0", manifest.dependencies["com.unity.test"]);
            Assert.AreEqual("TestRegistry", manifest.scopedRegistries[0].Name);
        }

        #endregion
    }
}
