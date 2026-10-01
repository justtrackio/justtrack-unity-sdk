using System;
using NUnit.Framework;
using JustTrack;

namespace JustTrack.Tests.Editor
{
    /// <summary>
    /// Unit tests for the <see cref="ApplicationVersion"/> class.
    /// </summary>
    public class ApplicationVersionTests
    {
        [Test]
        public void Constructor_WithBothValues_SetsProperties()
        {
            var applicationVersion = new ApplicationVersion("1.2.3", "42");

            Assert.AreEqual("1.2.3", applicationVersion.Version);
            Assert.AreEqual("42", applicationVersion.VersionCode);
        }

        [Test]
        public void Constructor_WithNullVersion_Throws()
        {
            Assert.Throws<ArgumentException>(() => new ApplicationVersion(null!, "42"));
            Assert.Throws<ArgumentException>(() => new ApplicationVersion(string.Empty, "42"));
        }

        [Test]
        public void Constructor_WithNullVersionCode_Throws()
        {
            Assert.Throws<ArgumentException>(() => new ApplicationVersion("1.2.3", null!));
            Assert.Throws<ArgumentException>(() => new ApplicationVersion("1.2.3", string.Empty));
        }

        [Test]
        public void Constructor_WithBothEmpty_Throws()
        {
            Assert.Throws<ArgumentException>(() => new ApplicationVersion(string.Empty, string.Empty));
        }
    }
}
