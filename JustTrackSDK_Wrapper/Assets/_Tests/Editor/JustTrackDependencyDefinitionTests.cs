using NUnit.Framework;
using JustTrack;
using System.Linq;

namespace JustTrack.Tests.Editor
{
    /// <summary>
    /// Unit tests for the JustTrackDependencyDefinition class.
    /// Tests the dependency list and Dependency data class.
    /// </summary>
    public class JustTrackDependencyDefinitionTests
    {
        #region DEPENDENCIES List Tests

        [Test]
        public void DEPENDENCIES_IsNotNull()
        {
            // Assert
            Assert.IsNotNull(JustTrackDependencyDefinition.Dependencies);
        }

        [Test]
        public void DEPENDENCIES_IsNotEmpty()
        {
            // Assert
            Assert.IsNotEmpty(JustTrackDependencyDefinition.Dependencies);
            Assert.Greater(JustTrackDependencyDefinition.Dependencies.Count, 0);
        }

        [Test]
        public void DEPENDENCIES_AllHaveNonEmptyGroupId()
        {
            // Act & Assert
            foreach (var dependency in JustTrackDependencyDefinition.Dependencies)
            {
                Assert.IsNotNull(dependency.GroupId, "Dependency groupId should not be null");
                Assert.IsNotEmpty(dependency.GroupId, "Dependency groupId should not be empty");
            }
        }

        [Test]
        public void DEPENDENCIES_AllHaveNonEmptyArtifactId()
        {
            // Act & Assert
            foreach (var dependency in JustTrackDependencyDefinition.Dependencies)
            {
                Assert.IsNotNull(dependency.ArtifactId, "Dependency artifactId should not be null");
                Assert.IsNotEmpty(dependency.ArtifactId, "Dependency artifactId should not be empty");
            }
        }

        [Test]
        public void DEPENDENCIES_AllHaveNonEmptyVersion()
        {
            // Act & Assert
            foreach (var dependency in JustTrackDependencyDefinition.Dependencies)
            {
                Assert.IsNotNull(dependency.Version, "Dependency version should not be null");
                Assert.IsNotEmpty(dependency.Version, "Dependency version should not be empty");
            }
        }

        [Test]
        public void DEPENDENCIES_ContainsExpectedDependencies()
        {
            // Arrange
            var groupIds = JustTrackDependencyDefinition.Dependencies.Select(d => d.GroupId).ToList();

            // Assert - Verify some key dependencies exist
            Assert.Contains("com.google.android.play", groupIds);
            Assert.Contains("androidx.lifecycle", groupIds);
            Assert.Contains("com.google.android.gms", groupIds);
        }

        #endregion

        #region Dependency Class Tests

        [Test]
        public void DEPENDENCIES_VersionsAreValid()
        {
            // Act & Assert
            foreach (var dependency in JustTrackDependencyDefinition.Dependencies)
            {
                // Check version format (should contain dots for semantic versioning)
                Assert.That(dependency.Version, Does.Match(@"^\d+(\.\d+)*$"), 
                    $"Version '{dependency.Version}' for {dependency.GroupId}:{dependency.ArtifactId} should be valid semantic version");
            }
        }

        #endregion
    }
}
