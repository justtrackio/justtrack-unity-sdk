using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;
using UnityEngine.TestTools;

// Fake types that simulate third-party SDKs for positive-path testing.
// Discovered by Reflection.cs via AppDomain.CurrentDomain.GetAssemblies().
namespace AppsFlyerSDK
{
    public static class AppsFlyer
    {
        public static string getAppsFlyerId() => "test-appsflyer-id-123";
    }
}

namespace AudienceNetwork
{
    public static class AdSettings
    {
        public static void SetAdvertiserTrackingEnabled(bool enabled) { }
    }
}

namespace JustTrack.Tests.PlayMode
{
    public class ReflectionTests
    {
        private static readonly FieldInfo AppsflyerAssemblyField =
            typeof(Reflection).GetField("appsflyerAssembly", BindingFlags.Static | BindingFlags.NonPublic);

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            AppsflyerAssemblyField.SetValue(null, null);
            yield return null;
        }

        #region GetAppsflyerAssembly

        [UnityTest]
        public IEnumerator GetAppsflyerAssembly_WhenAvailable_ReturnsNonNull()
        {
            yield return null;
            var result = Reflection.GetAppsflyerAssembly();
            Assert.IsNotNull(result);
        }

        [UnityTest]
        public IEnumerator GetAppsflyerAssembly_CachesResult()
        {
            yield return null;
            var first = Reflection.GetAppsflyerAssembly();
            var second = Reflection.GetAppsflyerAssembly();
            Assert.AreSame(first, second);
        }

        [UnityTest]
        public IEnumerator GetAppsflyerAssembly_WhenCachePreSet_ReturnsCachedInstance()
        {
            // Pre-set the cache to cover the early-return path (line 120-122)
            var assembly = Reflection.GetAppsflyerAssembly();
            // Now call again — should return cached without scanning assemblies
            var cached = Reflection.GetAppsflyerAssembly();
            yield return null;
            Assert.AreSame(assembly, cached);
        }

        #endregion

        #region GetAppsflyerId

        [UnityTest]
        public IEnumerator GetAppsflyerId_WhenAvailable_ReturnsId()
        {
            yield return null;
            string id = Reflection.GetAppsflyerId();
            Assert.AreEqual("test-appsflyer-id-123", id);
        }

        [UnityTest]
        public IEnumerator GetAppsflyerId_WhenAssemblyNull_ReturnsEmpty()
        {
            // Inject a pre-cached value so GetAppsflyerAssembly returns it,
            // then clear it and rely on the broken approach.
            // Actually: set cache to trigger the null-assembly branch by injecting
            // a value that causes GetAppsflyerAssembly() itself to return null.
            // We achieve this by having GetAppsflyerId catch an exception:
            // inject a broken AppsflyerAssembly with null MethodInfo into cache.
            // When GetAppsflyerId calls GetAppsflyerAssembly -> returns cached (broken).
            // Then calls assembly.GetAppsflyerId() -> throws NullReferenceException -> catch returns "".
            var ctor = typeof(Reflection.AppsflyerAssembly).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            var broken = ctor.Invoke(new object[] { null });
            AppsflyerAssemblyField.SetValue(null, broken);
            yield return null;

            string id = Reflection.GetAppsflyerId();
            Assert.AreEqual(string.Empty, id);
        }

        #endregion

        #region HasFacebookAudienceNetworkUnity

        [UnityTest]
        public IEnumerator HasFacebookAudienceNetworkUnity_WhenAvailable_ReturnsTrue()
        {
            yield return null;
            Assert.IsTrue(Reflection.HasFacebookAudienceNetworkUnity());
        }

        #endregion

        #region SetAdvertiserTrackingEnabled

        [UnityTest]
        public IEnumerator SetAdvertiserTrackingEnabled_DoesNotThrow()
        {
            yield return null;
            // Looks for JustTrackInjected.FacebookAudienceNetworkAdapter which doesn't exist.
            // Should gracefully do nothing.
            Assert.DoesNotThrow(() => Reflection.SetAdvertiserTrackingEnabled(true));
            Assert.DoesNotThrow(() => Reflection.SetAdvertiserTrackingEnabled(false));
        }

        #endregion
    }
}
