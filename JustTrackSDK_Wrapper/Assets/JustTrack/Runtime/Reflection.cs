using System;
using System.Linq.Expressions;
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
        /// Represents impression data from IronSource ad network.
        /// </summary>
        internal class IronSourceImpressionData
        {
            /// <summary>
            /// Gets the ad unit identifier.
            /// </summary>
            internal string AdUnit { get; }

            /// <summary>
            /// Gets the placement identifier.
            /// </summary>
            internal string Placement { get; }

            /// <summary>
            /// Gets the ad network identifier.
            /// </summary>
            internal string AdNetwork { get; }

            /// <summary>
            /// Gets the A/B testing identifier.
            /// </summary>
            internal string AbTesting { get; }

            /// <summary>
            /// Gets the segment name.
            /// </summary>
            internal string SegmentName { get; }

            /// <summary>
            /// Gets the instance name.
            /// </summary>
            internal string InstanceName { get; }

            /// <summary>
            /// Gets the revenue amount.
            /// </summary>
            internal double Revenue { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="IronSourceImpressionData"/> class.
            /// </summary>
            /// <param name="adUnit">The ad unit identifier.</param>
            /// <param name="placement">The placement identifier.</param>
            /// <param name="adNetwork">The ad network identifier.</param>
            /// <param name="abTesting">The A/B testing identifier.</param>
            /// <param name="segmentName">The segment name.</param>
            /// <param name="instanceName">The instance name.</param>
            /// <param name="revenue">The revenue amount.</param>
            internal IronSourceImpressionData(string adUnit, string placement, string adNetwork, string abTesting, string segmentName, string instanceName, double revenue)
            {
                AdUnit = adUnit;
                Placement = placement;
                AdNetwork = adNetwork;
                AbTesting = abTesting;
                SegmentName = segmentName;
                InstanceName = instanceName;
                Revenue = revenue;
            }
        }

        /// <summary>
        /// Wrapper class for interacting with IronSource SDK via reflection.
        /// </summary>
        public class IronSourceAssembly
        {
            /// <summary>
            /// Gets the banner ad type identifier.
            /// </summary>
            internal string BannerType { get; }

            /// <summary>
            /// Gets the interstitial ad type identifier.
            /// </summary>
            internal string InterstitialType { get; }

            /// <summary>
            /// Gets the rewarded video ad type identifier.
            /// </summary>
            internal string RewardedVideoType { get; }

            /// <summary>
            /// Gets the offerwall ad type identifier.
            /// </summary>
            internal string OfferwallType { get; }

            private readonly PropertyInfo agentField;
            private readonly MethodInfo init;
            private readonly MethodInfo setUserId;
            private readonly Type impressionDataType;
            private readonly FieldInfo impressionDataAdUnit;
            private readonly FieldInfo impressionDataPlacement;
            private readonly FieldInfo impressionDataAdNetwork;
            private readonly FieldInfo impressionDataAbTesting;
            private readonly FieldInfo impressionDataSegmentName;
            private readonly FieldInfo impressionDataInstanceName;
            private readonly FieldInfo impressionDataRevenue;
            private readonly EventInfo? onImpressionEvent;

            private readonly IronSourceAdapter? ironSourceAdapter;

            /// <summary>
            /// Initializes a new instance of the <see cref="IronSourceAssembly"/> class.
            /// </summary>
            /// <param name="assembly">The IronSource assembly to wrap.</param>
            internal IronSourceAssembly(Assembly assembly)
            {
                var adUnitsType = assembly.GetType("IronSourceAdUnits", true);
                BannerType = (string)GetStaticField("BANNER", adUnitsType).GetValue(null);
                InterstitialType = (string)GetStaticField("INTERSTITIAL", adUnitsType).GetValue(null);
                RewardedVideoType = (string)GetStaticField("REWARDED_VIDEO", adUnitsType).GetValue(null);
                OfferwallType = (string)GetStaticField("OFFERWALL", adUnitsType).GetValue(null);

                var ironSourceType = assembly.GetType("IronSource", true);
                this.agentField = GetStaticField("Agent", ironSourceType);
                this.init = ironSourceType.GetMethod("init", BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { typeof(string), typeof(string[]) }, null);
                this.setUserId = ironSourceType.GetMethod("setUserId", BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { typeof(string) }, null);

                this.impressionDataType = assembly.GetType("IronSourceImpressionData", true);
                this.impressionDataAdUnit = this.impressionDataType.GetField("adUnit");
                this.impressionDataPlacement = this.impressionDataType.GetField("placement");
                this.impressionDataAdNetwork = this.impressionDataType.GetField("adNetwork");
                this.impressionDataAbTesting = this.impressionDataType.GetField("ab");
                this.impressionDataSegmentName = this.impressionDataType.GetField("segmentName");
                this.impressionDataInstanceName = this.impressionDataType.GetField("instanceName");
                this.impressionDataRevenue = this.impressionDataType.GetField("revenue");

                var eventsType = assembly.GetType("IronSourceEvents", true);

                var onImpressionSuccessInfo = eventsType.GetMember("onImpressionSuccessEvent", BindingFlags.Public | BindingFlags.Static);
                var onImpressionEvent = IronSourceAssembly.GetOnImpressionEvent(onImpressionSuccessInfo);

                if (onImpressionEvent == null)
                {
                    var onImpressionDataReadyInfo = eventsType.GetMember("onImpressionDataReadyEvent", BindingFlags.Public | BindingFlags.Static);
                    onImpressionEvent = IronSourceAssembly.GetOnImpressionEvent(onImpressionDataReadyInfo);
                }

                this.onImpressionEvent = onImpressionEvent;
                this.ironSourceAdapter = GetIronSourceAdapter();
            }

            /// <summary>
            /// Gets the impression event from the provided member info array.
            /// </summary>
            /// <param name="onImpressionEventInfo">The member info array to search.</param>
            /// <returns>The impression event info, or <c>null</c> if not found.</returns>
            private static EventInfo? GetOnImpressionEvent(System.Reflection.MemberInfo[] onImpressionEventInfo)
            {
                foreach (var onImpressionEvent in onImpressionEventInfo)
                {
                    if (onImpressionEvent.MemberType != MemberTypes.Event)
                    {
                        continue;
                    }

                    return (EventInfo)onImpressionEvent;
                }

                return null;
            }

            /// <summary>
            /// Initializes the IronSource SDK with the specified app key and ad units.
            /// </summary>
            /// <param name="appKey">The IronSource app key.</param>
            /// <param name="adUnits">The ad units to initialize.</param>
            internal void Init(string appKey, string[] adUnits)
            {
                var agent = agentField.GetValue(null);
                init.Invoke(agent, new object[] { appKey, adUnits });
            }

            /// <summary>
            /// Sets the user ID for the IronSource SDK.
            /// </summary>
            /// <param name="userId">The user ID to set.</param>
            internal void SetUserId(string userId)
            {
                var agent = agentField.GetValue(null);
                setUserId.Invoke(agent, new object[] { userId });
            }

            /// <summary>
            /// Converts an IronSource impression object to our impression data format.
            /// </summary>
            /// <param name="impression">The IronSource impression object.</param>
            /// <returns>The converted impression data.</returns>
            internal IronSourceImpressionData ToImpressionData(object impression)
            {
                var adUnit = (string)impressionDataAdUnit.GetValue(impression);
                var placement = (string)impressionDataPlacement.GetValue(impression);
                var adNetwork = (string)impressionDataAdNetwork.GetValue(impression);
                var abTesting = (string)impressionDataAbTesting.GetValue(impression);
                var segmentName = (string)impressionDataSegmentName.GetValue(impression);
                var instanceName = (string)impressionDataInstanceName.GetValue(impression);
                var revenue = (double?)impressionDataRevenue.GetValue(impression);

                return new IronSourceImpressionData(adUnit, placement, adNetwork, abTesting, segmentName, instanceName, revenue ?? 0);
            }

            /// <summary>
            /// Adds an impression listener to the IronSource SDK.
            /// </summary>
            /// <param name="proxy">The proxy action to handle impressions.</param>
            internal void AddImpressionListener(Action<object> proxy)
            {
                // with IL2CPP, we can't actually use reflection to generate code, so we use another trick:
                // - first, we use reflection to lookup a type we inject into the main assembly
                // - that type has direct code references to the ironSource assembly
                // - thus, we create an instance of that type via reflection and simply call it here
                if (ironSourceAdapter != null)
                {
                    ironSourceAdapter.SetIronSourceOnImpressionHandler(proxy);
                }
                else if (onImpressionEvent != null)
                {
                    onImpressionEvent.GetAddMethod().Invoke(null, new object[] { BuildDelegate(proxy, onImpressionEvent.EventHandlerType) });
                }
            }

            /// <summary>
            /// Builds a delegate for the specified proxy action and delegate type.
            /// </summary>
            /// <param name="proxy">The proxy action.</param>
            /// <param name="delegateType">The target delegate type.</param>
            /// <returns>The built delegate.</returns>
            private static object BuildDelegate(Action<object> proxy, Type delegateType)
            {
                ParameterInfo[] methodParams = delegateType.GetMethod("Invoke").GetParameters();
                ParameterExpression[] paramsOfDelegate = new ParameterExpression[methodParams.Length];

                for (int i = 0; i < methodParams.Length; i++)
                {
                    paramsOfDelegate[i] = Expression.Parameter(methodParams[i].ParameterType, methodParams[i].Name);
                }

                var expr = Expression.Lambda(
                    delegateType,
                    Expression.Invoke(Expression.Constant(proxy), paramsOfDelegate),
                    paramsOfDelegate);

                return expr.Compile();
            }

            /// <summary>
            /// Gets a static property by name from the specified type.
            /// </summary>
            /// <param name="fieldName">The name of the static property.</param>
            /// <param name="type">The type to search.</param>
            /// <returns>The property info.</returns>
            /// <exception cref="Exception">Thrown when the static property is not found.</exception>
            private static PropertyInfo GetStaticField(string fieldName, Type type)
            {
                var members = type.GetMember(fieldName, BindingFlags.Public | BindingFlags.Static);
                foreach (var member in members)
                {
                    if (member.MemberType != MemberTypes.Property)
                    {
                        continue;
                    }

                    return (PropertyInfo)member;
                }

                throw new Exception($"Missing static field {fieldName} on {type.ToString()}");
            }
        }

        private static IronSourceAssembly? ironSourceAssembly = null;

        /// <summary>
        /// Gets the IronSource assembly wrapper, creating it if necessary.
        /// </summary>
        /// <returns>The IronSource assembly wrapper, or <c>null</c> if IronSource is not available.</returns>
        public static IronSourceAssembly? GetIronSourceAssembly()
        {
            if (ironSourceAssembly != null)
            {
                return ironSourceAssembly;
            }

            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    var type = assembly.GetType("IronSource", false);
                    if (type != null)
                    {
                        ironSourceAssembly = new IronSourceAssembly(assembly);
                        return ironSourceAssembly;
                    }
                }

                return null;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the injected IronSource adapter implementation.
        /// </summary>
        /// <returns>The IronSource adapter, or <c>null</c> if not available.</returns>
        public static IronSourceAdapter? GetIronSourceAdapter()
        {
            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    var type = assembly.GetType("JustTrackInjected.IronSourceAdapterImpl", false);
                    if (type != null)
                    {
                        var constructor = type.GetConstructor(new Type[0]);
                        return (IronSourceAdapter)constructor.Invoke(null);
                    }
                }

                return null;
            }
            catch (System.Exception)
            {
                return null;
            }
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
