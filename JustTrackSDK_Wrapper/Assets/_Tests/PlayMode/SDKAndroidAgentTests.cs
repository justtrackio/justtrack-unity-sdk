using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using JustTrack;

namespace JustTrack.Tests.PlayMode
{
    /// <summary>
    /// PlayMode tests for the JustTrack SDK agent.
    ///
    /// When run on a real Android device (-testPlatform Android) these exercise SDKAndroidAgent
    /// and verify that every AndroidJavaObject/AndroidJavaClass call resolves against the native SDK.
    /// When run in the Editor (-testPlatform PlayMode) they exercise SDKEditorAgent.
    ///
    /// A JustTrackSDKBehaviour instance is created during setup so that callbacks dispatched via
    /// JustTrackSDKBehaviour.CallOnMainThread are pumped through its Update() loop, and so the SDK
    /// is initialized with the API token from JustTrackSettings.
    /// </summary>
    [Timeout(10000)]
    public class SDKAndroidAgentTests
    {
        // Kept below the per-test [Timeout] so a missing callback is reported by the assertion
        // ("callback was not invoked") rather than by the harness killing the test.
        private const float CallbackTimeoutSeconds = 7f;
        private const float InitTimeoutSeconds = 30f;

        private static bool s_setupDone;

        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            if (s_setupDone)
            {
                yield break;
            }

            // Create the behaviour that pumps the main-thread callback queue and
            // auto-initializes the SDK from JustTrackSettings (Resources).
            if (UnityEngine.Object.FindObjectOfType<JustTrackSDKBehaviour>() == null)
            {
                var go = new GameObject("JustTrackSDKBehaviour");
                go.AddComponent<JustTrackSDKBehaviour>();
            }

            float start = Time.realtimeSinceStartup;
            while (!JustTrackSDK.IsRunning() && Time.realtimeSinceStartup - start < InitTimeoutSeconds)
            {
                yield return null;
            }

            s_setupDone = true;
        }

        private IEnumerator WaitForCallback(Func<bool> condition, float timeoutSeconds = CallbackTimeoutSeconds)
        {
            float start = Time.realtimeSinceStartup;
            while (!condition() && Time.realtimeSinceStartup - start < timeoutSeconds)
            {
                yield return null;
            }
        }

        #region Initialization

        [UnityTest, Order(1)]
        public IEnumerator Init_SetsRunning()
        {
            yield return null;
            Assert.IsTrue(JustTrackSDK.IsRunning(), "SDK did not reach running state after initialization.");
        }

        [UnityTest, Order(2)]
        public IEnumerator GetVersion_ReturnsNonEmpty()
        {
            yield return null;
            string version = JustTrackSDK.GetVersion();
            Assert.IsNotNull(version);
            Assert.IsNotEmpty(version);
        }

        #endregion

        #region Events

        // NOTE: The publishEvent success callback only fires after the native SDK flushes the
        // event to the server, which is batched and can exceed any reasonable test timeout.
        // These tests therefore verify that the native call path (AppEvent / Money / dimension
        // AndroidJavaObject construction and the publishEvent bridge call) executes without
        // throwing. The callback round-trip itself is covered by the other callback-based tests
        // (GetAttribution, Anonymize, SetExperimentVariant, RemoteConfig.Fetch, ...).

        [UnityTest, Order(20)]
        public IEnumerator Track_WithCount_DoesNotThrow()
        {
            var evt = new AppEvent("test_count_event", 5.0, Unit.Count);
            Assert.DoesNotThrow(() => JustTrackSDK.Track(evt, () => { }, _ => { }));
            yield return null;
        }

        [UnityTest, Order(21)]
        public IEnumerator Track_WithMilliseconds_DoesNotThrow()
        {
            var evt = new AppEvent("test_ms_event", 1500.0, Unit.Milliseconds);
            Assert.DoesNotThrow(() => JustTrackSDK.Track(evt, () => { }, _ => { }));
            yield return null;
        }

        [UnityTest, Order(22)]
        public IEnumerator Track_WithSeconds_DoesNotThrow()
        {
            var evt = new AppEvent("test_sec_event", 30.0, Unit.Seconds);
            Assert.DoesNotThrow(() => JustTrackSDK.Track(evt, () => { }, _ => { }));
            yield return null;
        }

        [UnityTest, Order(23)]
        public IEnumerator Track_WithCurrency_DoesNotThrow()
        {
            var evt = new AppEvent("test_money_event", new Money(9.99, "USD"));
            Assert.DoesNotThrow(() => JustTrackSDK.Track(evt, () => { }, _ => { }));
            yield return null;
        }

        [UnityTest, Order(24)]
        public IEnumerator Track_WithDimensions_DoesNotThrow()
        {
            var evt = new AppEvent("test_dim_event")
                .AddDimension("dim_key", "dim_value")
                .AddDimension("another_key", "another_value");
            Assert.DoesNotThrow(() => JustTrackSDK.Track(evt, () => { }, _ => { }));
            yield return null;
        }

        #endregion

        #region Attribution

        [UnityTest, Order(30)]
        public IEnumerator GetAttribution_InvokesCallback()
        {
            bool called = false;
            JustTrackSDK.GetAttribution(_ => { called = true; }, _ => { called = true; });
            yield return WaitForCallback(() => called);
            Assert.IsTrue(called, "GetAttribution callback was not invoked.");
        }

        [UnityTest, Order(31)]
        public IEnumerator GetRetargetingParameters_InvokesCallback()
        {
            bool called = false;
            JustTrackSDK.GetRetargetingParameters(_ => { called = true; }, _ => { called = true; });
            yield return WaitForCallback(() => called);
            Assert.IsTrue(called, "GetRetargetingParameters callback was not invoked.");
        }

        [UnityTest, Order(32)]
        public IEnumerator GetPreliminaryRetargetingParameters_DoesNotThrow()
        {
            yield return null;
            Assert.DoesNotThrow(() => JustTrackSDK.GetPreliminaryRetargetingParameters());
        }

        #endregion

        #region Integrations

        [UnityTest, Order(40)]
        public IEnumerator IntegrateWithAppLovin_InvokesCallback()
        {
            bool called = false;
            JustTrackSDK.IntegrateWithAppLovin(null, () => { called = true; }, _ => { called = true; });
            yield return WaitForCallback(() => called);
            Assert.IsTrue(called, "IntegrateWithAppLovin callback was not invoked.");
        }

        [UnityTest, Order(41)]
        public IEnumerator IntegrateWithIronSource_InvokesCallback()
        {
            bool called = false;
            JustTrackSDK.IntegrateWithIronSource(null, () => { called = true; }, _ => { called = true; });
            yield return WaitForCallback(() => called);
            Assert.IsTrue(called, "IntegrateWithIronSource callback was not invoked.");
        }

        [UnityTest, Order(42)]
        public IEnumerator IntegrateWithUnityAds_InvokesCallback()
        {
            bool called = false;
            JustTrackSDK.IntegrateWithUnityAds(() => { called = true; }, _ => { called = true; });
            yield return WaitForCallback(() => called);
            Assert.IsTrue(called, "IntegrateWithUnityAds callback was not invoked.");
        }

        [UnityTest, Order(43)]
        public IEnumerator IntegrateWithFirebase_InvokesCallback()
        {
            bool called = false;
            JustTrackSDK.IntegrateWithFirebase(() => { called = true; }, _ => { called = true; });
            yield return WaitForCallback(() => called);
            Assert.IsTrue(called, "IntegrateWithFirebase callback was not invoked.");
        }

        #endregion

        #region Remote Config

        [UnityTest, Order(50)]
        public IEnumerator RemoteConfig_Fetch_InvokesCallback()
        {
            var rc = JustTrackSDK.GetRemoteConfig();
            Assert.IsNotNull(rc, "GetRemoteConfig returned null.");

            bool called = false;
            rc!.Fetch(() => { called = true; }, _ => { called = true; });
            yield return WaitForCallback(() => called);
            Assert.IsTrue(called, "RemoteConfig.Fetch callback was not invoked.");
        }

        [UnityTest, Order(51)]
        public IEnumerator RemoteConfig_FetchAndActivate_InvokesCallback()
        {
            var rc = JustTrackSDK.GetRemoteConfig();
            Assert.IsNotNull(rc);

            bool called = false;
            rc!.FetchAndActivate(() => { called = true; }, _ => { called = true; });
            yield return WaitForCallback(() => called);
            Assert.IsTrue(called, "RemoteConfig.FetchAndActivate callback was not invoked.");
        }

        [UnityTest, Order(52)]
        public IEnumerator RemoteConfig_SetConfig_DoesNotThrow()
        {
            var rc = JustTrackSDK.GetRemoteConfig();
            Assert.IsNotNull(rc);
            yield return null;
            Assert.DoesNotThrow(() => rc!.SetConfig(new JusttrackRemoteConfigSettings(300)));
        }

        [UnityTest, Order(53)]
        public IEnumerator RemoteConfig_GetAll_ReturnsArray()
        {
            var rc = JustTrackSDK.GetRemoteConfig();
            Assert.IsNotNull(rc);
            yield return null;
            Assignment[] assignments = rc!.GetAll();
            Assert.IsNotNull(assignments);
        }

        [UnityTest, Order(54)]
        public IEnumerator RemoteConfig_GetString_ReturnsNullForUnknownKey()
        {
            var rc = JustTrackSDK.GetRemoteConfig();
            Assert.IsNotNull(rc);
            yield return null;
            Assert.IsNull(rc!.GetString("nonexistent_key_12345"));
        }

        [UnityTest, Order(55)]
        public IEnumerator RemoteConfig_GetBoolean_ReturnsNullForUnknownKey()
        {
            var rc = JustTrackSDK.GetRemoteConfig();
            Assert.IsNotNull(rc);
            yield return null;
            Assert.IsNull(rc!.GetBoolean("nonexistent_key_12345"));
        }

        [UnityTest, Order(56)]
        public IEnumerator RemoteConfig_GetInt_ReturnsNullForUnknownKey()
        {
            var rc = JustTrackSDK.GetRemoteConfig();
            Assert.IsNotNull(rc);
            yield return null;
            Assert.IsNull(rc!.GetInt("nonexistent_key_12345"));
        }

        [UnityTest, Order(57)]
        public IEnumerator RemoteConfig_GetDouble_ReturnsNullForUnknownKey()
        {
            var rc = JustTrackSDK.GetRemoteConfig();
            Assert.IsNotNull(rc);
            yield return null;
            Assert.IsNull(rc!.GetDouble("nonexistent_key_12345"));
        }

        [UnityTest, Order(58)]
        public IEnumerator RemoteConfig_GetLong_ReturnsNullForUnknownKey()
        {
            var rc = JustTrackSDK.GetRemoteConfig();
            Assert.IsNotNull(rc);
            yield return null;
            Assert.IsNull(rc!.GetLong("nonexistent_key_12345"));
        }

        #endregion

        #region Ad Impression

        [UnityTest, Order(60)]
        public IEnumerator ForwardAdImpression_DoesNotThrow()
        {
            yield return null;
            var impression = new AdImpression("test_unit", "test_sdk")
                .SetNetwork("test_network")
                .SetPlacement("test_placement")
                .SetRevenue(new Money(0.01, "USD"));
            Assert.DoesNotThrow(() => JustTrackSDK.ForwardAdImpression(impression));
            yield return null;
        }

        [UnityTest, Order(61)]
        public IEnumerator ForwardAdImpression_WithoutRevenue_DoesNotThrow()
        {
            yield return null;
            var impression = new AdImpression("test_unit_no_rev", "test_sdk");
            Assert.DoesNotThrow(() => JustTrackSDK.ForwardAdImpression(impression));
            yield return null;
        }

        #endregion

        #region Experiment Variant

        [UnityTest, Order(70)]
        public IEnumerator SetExperimentVariant_WithoutTagsOrDate_InvokesCallback()
        {
            bool called = false;
            JustTrackSDK.SetExperimentVariant("test_exp", "variant_a", null, null, () => { called = true; }, _ => { called = true; });
            yield return WaitForCallback(() => called);
            Assert.IsTrue(called, "SetExperimentVariant (no tags/date) callback was not invoked.");
        }

        [UnityTest, Order(71)]
        public IEnumerator SetExperimentVariant_WithTags_InvokesCallback()
        {
            bool called = false;
            JustTrackSDK.SetExperimentVariant("test_exp_tags", "variant_b", new[] { "tag1", "tag2" }, null, () => { called = true; }, _ => { called = true; });
            yield return WaitForCallback(() => called);
            Assert.IsTrue(called, "SetExperimentVariant (tags) callback was not invoked.");
        }

        [UnityTest, Order(72)]
        public IEnumerator SetExperimentVariant_WithHappenedAt_InvokesCallback()
        {
            bool called = false;
            JustTrackSDK.SetExperimentVariant("test_exp_date", "variant_c", null, DateTime.UtcNow, () => { called = true; }, _ => { called = true; });
            yield return WaitForCallback(() => called);
            Assert.IsTrue(called, "SetExperimentVariant (happenedAt) callback was not invoked.");
        }

        [UnityTest, Order(73)]
        public IEnumerator SetExperimentVariant_WithTagsAndDate_InvokesCallback()
        {
            bool called = false;
            JustTrackSDK.SetExperimentVariant("test_exp_both", "variant_d", new[] { "tag1" }, DateTime.UtcNow, () => { called = true; }, _ => { called = true; });
            yield return WaitForCallback(() => called);
            Assert.IsTrue(called, "SetExperimentVariant (tags + date) callback was not invoked.");
        }

        #endregion

        #region User Management

        [UnityTest, Order(80)]
        public IEnumerator SetUserId_DoesNotThrow()
        {
            yield return null;
            Assert.DoesNotThrow(() => JustTrackSDK.SetUserId("test_user_123"));
            yield return null;
        }

        [UnityTest, Order(81)]
        public IEnumerator SetFirebaseAppInstanceId_DoesNotThrow()
        {
            yield return null;
            Assert.DoesNotThrow(() => JustTrackSDK.SetFirebaseAppInstanceId("fake_firebase_id"));
            yield return null;
        }

        [UnityTest, Order(82)]
        public IEnumerator SetAutomaticInAppPurchaseTracking_DoesNotThrow()
        {
            yield return null;
            Assert.DoesNotThrow(() => JustTrackSDK.SetAutomaticInAppPurchaseTracking(false));
            Assert.DoesNotThrow(() => JustTrackSDK.SetAutomaticInAppPurchaseTracking(true));
            yield return null;
        }

        #endregion

        #region Install Instance ID and Advertiser ID

        [UnityTest, Order(90)]
        public IEnumerator GetInstallInstanceId_ReturnsNonEmpty()
        {
            string? installId = null;
            JustTrackSDK.GetInstallInstanceId(id => { installId = id; }, _ => { });
            yield return WaitForCallback(() => installId != null);
            Assert.IsNotNull(installId, "GetInstallInstanceId did not return an id.");
            Assert.IsNotEmpty(installId);
        }

        [UnityTest, Order(91)]
        public IEnumerator GetAdvertiserIdInfo_InvokesCallback()
        {
            bool called = false;
            JustTrackSDK.GetAdvertiserIdInfo(_ => { called = true; }, _ => { called = true; });
            yield return WaitForCallback(() => called);
            Assert.IsTrue(called, "GetAdvertiserIdInfo callback was not invoked.");
        }

        #endregion

        #region Anonymize (runs last - resets user state)

        [UnityTest, Order(200)]
        public IEnumerator Anonymize_InvokesCallback()
        {
            bool called = false;
            JustTrackSDK.Anonymize(() => { called = true; }, _ => { called = true; });
            yield return WaitForCallback(() => called);
            Assert.IsTrue(called, "Anonymize callback was not invoked.");
        }

        #endregion
    }
}
