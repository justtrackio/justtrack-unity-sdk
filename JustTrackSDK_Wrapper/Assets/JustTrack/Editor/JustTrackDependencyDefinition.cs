using System.Collections.Generic;

// DO NOT EDIT, AUTOMATICALLY GENERATED
namespace JustTrack
{
    /// <summary>
    /// Defines dependencies for the justtrack SDK.
    /// </summary>
    internal class JustTrackDependencyDefinition
    {
        /// <summary>
        /// Gets the list of dependencies required by the justtrack SDK.
        /// </summary>
        internal static List<Dependency> Dependencies { get; } = new List<Dependency>
        {
            new Dependency("com.google.android.play", "integrity", "1.4.0"),
            new Dependency("androidx.lifecycle", "lifecycle-process", "2.1.0"),
            new Dependency("com.google.android.gms", "play-services-ads-identifier", "16.0.0"),
            new Dependency("com.google.android.gms", "play-services-appset", "16.0.0"),
            new Dependency("org.jetbrains.kotlinx", "kotlinx-coroutines-android", "1.9.0"),
            new Dependency("io.justtrack", "integrity", "1.0.1"),
            new Dependency("org.jetbrains.kotlin", "kotlin-stdlib-jdk7", "1.8.21"),
            new Dependency("org.jetbrains.kotlin", "kotlin-stdlib", "1.9.25"),
            new Dependency("org.jetbrains.kotlin", "kotlin-stdlib-jdk8", "1.8.21"),
            new Dependency("org.jetbrains", "annotations", "13.0"),
        };

        /// <summary>
        /// Represents a dependency with group ID, artifact ID, and version information.
        /// </summary>
        internal class Dependency
        {
            /// <summary>
            /// Gets the group ID of the dependency.
            /// </summary>
            internal string GroupId { get; }

            /// <summary>
            /// Gets the artifact ID of the dependency.
            /// </summary>
            internal string ArtifactId { get; }

            /// <summary>
            /// Gets the version of the dependency.
            /// </summary>
            internal string Version { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Dependency"/> class.
            /// </summary>
            /// <param name="pGroupId">The group ID of the dependency.</param>
            /// <param name="pArtifactId">The artifact ID of the dependency.</param>
            /// <param name="pVersion">The version of the dependency.</param>
            internal Dependency(string pGroupId, string pArtifactId, string pVersion)
            {
                this.GroupId = pGroupId;
                this.ArtifactId = pArtifactId;
                this.Version = pVersion;
            }
        }
    }
}