using System;
using System.Reflection;

namespace JustTrack
{
    /// <summary>
    /// Utility class for reflection-based integrations with third-party SDKs.
    /// </summary>
    public class Reflection
    {
        /// <summary>
        /// Retrieves the AppsFlyer ID using reflection.
        /// </summary>
        /// <returns>The AppsFlyer ID, or an empty string if retrieval fails.</returns>
        internal static string GetAppsflyerId()
        {
            try
            {
                var assembly = GetAppsflyerAssembly();
                if (assembly == null)
                {
                    return string.Empty;
                }

                return assembly.GetAppsflyerId();
            }
            catch (System.Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Checks if Facebook Audience Network Unity integration is available.
        /// </summary>
        /// <returns><c>true</c> if Facebook Audience Network Unity is available; otherwise, <c>false</c>.</returns>
        public static bool HasFacebookAudienceNetworkUnity()
        {
            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    var type = assembly.GetType("AudienceNetwork.AdSettings", false);
                    if (type != null)
                    {
                        var method = type.GetMethod("SetAdvertiserTrackingEnabled", BindingFlags.Public | BindingFlags.Static, null, CallingConventions.Any, new Type[] { typeof(bool) }, null);
                        return method != null;
                    }
                }

                return false;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Sets advertiser tracking enabled status for Facebook Audience Network on iOS.
        /// </summary>
        /// <param name="authorized">Whether advertiser tracking is authorized.</param>
        internal static void SetAdvertiserTrackingEnabled(bool authorized)
        {
#if UNITY_IOS
                // we only need to call this on iOS (the method also just exists on iOS), so we don't have to search for it on Android
                try {
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var assembly in assemblies) {
                        var type = assembly.GetType("JustTrackInjected.FacebookAudienceNetworkAdapter", false);
                        if (type != null) {
                            var method = type.GetMethod("SetAdvertiserTrackingEnabled", BindingFlags.Public | BindingFlags.Static, null, CallingConventions.Any, new Type[] { typeof(bool) }, null);
                            if (method != null) {
                                method.Invoke(null, new object[] { authorized });
                            }
                            return;
                        }
                    }
                } catch (System.Exception) {
                    return;
                }
#endif
        }

        /// <summary>
        /// Wrapper class for interacting with AppsFlyer SDK via reflection.
        /// </summary>
        public class AppsflyerAssembly
        {
            private readonly MethodInfo getAppsFlyerId;

            /// <summary>
            /// Initializes a new instance of the <see cref="AppsflyerAssembly"/> class.
            /// </summary>
            /// <param name="getAppsFlyerId">The method info for getting AppsFlyer ID.</param>
            internal AppsflyerAssembly(MethodInfo getAppsFlyerId)
            {
                this.getAppsFlyerId = getAppsFlyerId;
            }

            /// <summary>
            /// Gets the AppsFlyer ID.
            /// </summary>
            /// <returns>The AppsFlyer ID.</returns>
            internal string GetAppsflyerId()
            {
                return (string)getAppsFlyerId.Invoke(null, null);
            }
        }

        private static AppsflyerAssembly? appsflyerAssembly = null;

        /// <summary>
        /// Gets the AppsFlyer assembly wrapper, creating it if necessary.
        /// </summary>
        /// <returns>The AppsFlyer assembly wrapper, or <c>null</c> if AppsFlyer is not available.</returns>
        public static AppsflyerAssembly? GetAppsflyerAssembly()
        {
            if (appsflyerAssembly != null)
            {
                return appsflyerAssembly;
            }

            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    var type = assembly.GetType("AppsFlyerSDK.AppsFlyer", false);
                    if (type == null)
                    {
                        continue;
                    }

                    var method = type.GetMethod("getAppsFlyerId", BindingFlags.Public | BindingFlags.Static);
                    if (method == null)
                    {
                        continue;
                    }

                    appsflyerAssembly = new AppsflyerAssembly(method);
                    return appsflyerAssembly;
                }

                return null;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }
}
