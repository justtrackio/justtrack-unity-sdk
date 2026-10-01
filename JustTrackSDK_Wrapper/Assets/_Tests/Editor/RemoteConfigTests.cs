using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace JustTrack.Tests.Editor
{
    public class RemoteConfigTests
    {
        private class MockAgent : ISDKAgent
        {
            public bool Initialized { get; set; } = true;
            public bool FetchFails { get; set; }
            public bool ActivateFails { get; set; }
            public bool FetchAndActivateFails { get; set; }
            public Dictionary<string, bool?> Booleans { get; } = new();
            public Dictionary<string, double?> Doubles { get; } = new();
            public Dictionary<string, int?> Ints { get; } = new();
            public Dictionary<string, long?> Longs { get; } = new();
            public Dictionary<string, string?> Strings { get; } = new();
            public Assignment[] Assignments { get; set; } = Array.Empty<Assignment>();
            public JusttrackRemoteConfigSettings? LastSettings { get; private set; }

            public void FetchRemoteConfig(Action s, Action<string> f) { if (FetchFails) f("fetch error"); else s(); }
            public void ActivateRemoteConfig(string[] e, Action s, Action<string> f) { if (ActivateFails) f("activate error"); else s(); }
            public void FetchAndActivateRemoteConfig(Action s, Action<string> f) { if (FetchAndActivateFails) f("fetchAndActivate error"); else s(); }
            public void SetRemoteConfigSettings(JusttrackRemoteConfigSettings settings) => LastSettings = settings;
            public Assignment[] GetAllAssignments() => Assignments;
            public bool? GetRemoteConfigBoolean(string k) => Booleans.TryGetValue(k, out var v) ? v : null;
            public double? GetRemoteConfigDouble(string k) => Doubles.TryGetValue(k, out var v) ? v : null;
            public int? GetRemoteConfigInt(string k) => Ints.TryGetValue(k, out var v) ? v : null;
            public long? GetRemoteConfigLong(string k) => Longs.TryGetValue(k, out var v) ? v : null;
            public string? GetRemoteConfigString(string k) => Strings.TryGetValue(k, out var v) ? v : null;
            public bool IsInitialized() => Initialized;
            public bool IsRunning() => true;
            public void Initialize(string a, string? b, string? c, string? d, bool e, bool f, bool g, bool connectionTracking, string? h, string? i, string? j, string? k) { }
            public void Start() { }
            public void Stop() { }
            public void Anonymize(Action s, Action<string> f) => s();
            public void PublishEvent(AppEvent e, Action? s = null, Action<string>? f = null) => s?.Invoke();
            public void RegisterAttributionListener(Action<AttributionResponse> l) { }
            public void GetAttribution(Action<AttributionResponse> s, Action<string> f) => s(AttributionResponse.CreateFakeResponse());
            public void RegisterRetargetingParameterListener(Action<RetargetingParameters> l) { }
            public void RegisterPreliminaryRetargetingListener(Action<PreliminaryRetargetingParameters> l) { }
            public void IntegrateWithAppLovin(string? u, Action s, Action<string> f) => s();
            public void IntegrateWithFirebase(Action s, Action<string> f) => s();
            public void IntegrateWithIronSource(string? u, Action s, Action<string> f) => s();
            public void IntegrateWithUnityAds(Action s, Action<string> f) => s();
            public void IntegrateWithGoogleOdm(Action s, Action<string> f) => s();
            public void ForwardAdImpression(AdImpression i, Action s, Action<string> f) => s();
            public void SetGlobalDimension0(string? value) { }
            public void SetGlobalDimension1(string? value) { }
            public void SetGlobalDimension2(string? value) { }
            public void SetCustomUserId(string u) { }
            public void SetAutomaticInAppPurchaseTracking(bool e) { }
            public void SetFirebaseAppInstanceId(string id) { }
            public void GetInstallInstanceId(Action<string> s, Action<string> f) => s("mock-id");
            public void GetAdvertiserIdInfo(Action<AdvertiserIdInfo> s, Action<string> f) => s(new AdvertiserIdInfo(null, false));
            public void GetRetargetingParameters(Action<RetargetingParameters?> s, Action<string> f) => s(null);
            public PreliminaryRetargetingParameters? GetPreliminaryRetargetingParameters() => null;
            public void SetExperimentVariant(string e, string v, string[]? t, DateTime? h, Action s, Action<string> f) => s();
#if UNITY_ANDROID
            public void ForwardTransaction(string token, string productId, Money money, ProductType productType) { }
#endif
#if UNITY_IOS
            public void ForwardTransactionId(string transactionId, string productId, int quantity) { }
            public AttAuthorizationStatus GetTrackingAuthorizationStatus() => AttAuthorizationStatus.NotAvailable;
#endif
        }

        private MockAgent agent = null!;
        private RemoteConfig rc = null!;

        [SetUp]
        public void SetUp()
        {
            agent = new MockAgent();
            rc = new RemoteConfig(agent, action => action());
        }

        // --- Fetch ---

        [Test]
        public void Fetch_WhenAgentSucceeds_InvokesOnSuccess()
        {
            bool success = false;
            rc.Fetch(() => success = true, _ => Assert.Fail("onFailure should not be called"));
            Assert.IsTrue(success);
        }

        [Test]
        public void Fetch_WhenAgentFails_InvokesOnFailure()
        {
            agent.FetchFails = true;
            string? error = null;
            rc.Fetch(() => Assert.Fail("onSuccess should not be called"), err => error = err);
            Assert.AreEqual("fetch error", error);
        }

        [Test]
        public void Fetch_DelegatesToWaitForInitialization()
        {
            bool deferred = false;
            new RemoteConfig(agent, _ => deferred = true).Fetch(() => { }, _ => { });
            Assert.IsTrue(deferred);
        }

        // --- Activate ---

        [Test]
        public void Activate_WhenAgentSucceeds_InvokesOnSuccess()
        {
            bool success = false;
            rc.Activate(new[] { "exp1" }, () => success = true, _ => Assert.Fail());
            Assert.IsTrue(success);
        }

        [Test]
        public void Activate_WhenAgentFails_InvokesOnFailure()
        {
            agent.ActivateFails = true;
            string? error = null;
            rc.Activate(new[] { "exp1" }, () => Assert.Fail(), err => error = err);
            Assert.AreEqual("activate error", error);
        }

        // --- FetchAndActivate ---

        [Test]
        public void FetchAndActivate_WhenAgentSucceeds_InvokesOnSuccess()
        {
            bool success = false;
            rc.FetchAndActivate(() => success = true, _ => Assert.Fail());
            Assert.IsTrue(success);
        }

        [Test]
        public void FetchAndActivate_WhenAgentFails_InvokesOnFailure()
        {
            agent.FetchAndActivateFails = true;
            string? error = null;
            rc.FetchAndActivate(() => Assert.Fail(), err => error = err);
            Assert.AreEqual("fetchAndActivate error", error);
        }

        // --- SetConfig ---

        [Test]
        public void SetConfig_ForwardsSettingsToAgent()
        {
            rc.SetConfig(new JusttrackRemoteConfigSettings(600));
            Assert.AreEqual(600, agent.LastSettings?.minimumFetchIntervalInSeconds);
        }

        // --- GetAll ---

        [Test]
        public void GetAll_WhenInitialized_ReturnsAgentAssignments()
        {
            agent.Assignments = new[] { new Assignment("k", "v", "exp", false) };
            Assert.AreEqual(1, rc.GetAll().Length);
        }

        [Test]
        public void GetAll_WhenNotInitialized_ReturnsEmptyArray()
        {
            agent.Initialized = false;
            Assert.IsEmpty(rc.GetAll());
        }

        // --- Typed getters — not initialized → null ---

        [Test] public void GetBoolean_WhenNotInitialized_ReturnsNull() { agent.Initialized = false; Assert.IsNull(rc.GetBoolean("k")); }
        [Test] public void GetDouble_WhenNotInitialized_ReturnsNull()  { agent.Initialized = false; Assert.IsNull(rc.GetDouble("k")); }
        [Test] public void GetInt_WhenNotInitialized_ReturnsNull()     { agent.Initialized = false; Assert.IsNull(rc.GetInt("k")); }
        [Test] public void GetLong_WhenNotInitialized_ReturnsNull()    { agent.Initialized = false; Assert.IsNull(rc.GetLong("k")); }
        [Test] public void GetString_WhenNotInitialized_ReturnsNull()  { agent.Initialized = false; Assert.IsNull(rc.GetString("k")); }

        // --- Typed getters — key present ---

        [Test] public void GetBoolean_WhenKeyExists_ReturnsValue() { agent.Booleans["f"] = true;          Assert.AreEqual(true, rc.GetBoolean("f")); }
        [Test] public void GetDouble_WhenKeyExists_ReturnsValue()  { agent.Doubles["d"] = 3.14;            Assert.AreEqual(3.14, rc.GetDouble("d"), 0.0001); }
        [Test] public void GetInt_WhenKeyExists_ReturnsValue()     { agent.Ints["i"] = 42;                 Assert.AreEqual(42, rc.GetInt("i")); }
        [Test] public void GetLong_WhenKeyExists_ReturnsValue()    { agent.Longs["l"] = 9_000_000_000L;   Assert.AreEqual(9_000_000_000L, rc.GetLong("l")); }
        [Test] public void GetString_WhenKeyExists_ReturnsValue()  { agent.Strings["s"] = "hi";            Assert.AreEqual("hi", rc.GetString("s")); }

        // --- Typed getters — key absent → null ---

        [Test] public void GetBoolean_WhenKeyAbsent_ReturnsNull() => Assert.IsNull(rc.GetBoolean("missing"));
        [Test] public void GetDouble_WhenKeyAbsent_ReturnsNull()  => Assert.IsNull(rc.GetDouble("missing"));
        [Test] public void GetInt_WhenKeyAbsent_ReturnsNull()     => Assert.IsNull(rc.GetInt("missing"));
        [Test] public void GetLong_WhenKeyAbsent_ReturnsNull()    => Assert.IsNull(rc.GetLong("missing"));
        [Test] public void GetString_WhenKeyAbsent_ReturnsNull()  => Assert.IsNull(rc.GetString("missing"));
    }
}
