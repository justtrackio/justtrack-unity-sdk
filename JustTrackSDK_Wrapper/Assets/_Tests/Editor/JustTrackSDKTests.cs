using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace JustTrack.Tests.Editor
{
    public class JustTrackSDKTests
    {
        [TearDown]
        public void TearDown()
        {
            var t = typeof(JustTrackSDK);
            var f = BindingFlags.Static | BindingFlags.NonPublic;
            t.GetField("initializedAgent", f)?.SetValue(null, null);
            t.GetField("onInit", f)?.SetValue(null, null);
            t.GetField("remoteConfig", f)?.SetValue(null, null);
            t.GetField("SOnAttributionResponse", f)?.SetValue(null, null);
            t.GetField("SOnRetargetingParameters", f)?.SetValue(null, null);
            t.GetField("SOnPreliminaryRetargetingParameters", f)?.SetValue(null, null);
        }

        private static void Init() =>
            JustTrackSDK.Init("test-key", null, null, null, false, false, false, false, null, null, null);

        [Test]
        public void Init_WithEmptyKey_DoesNotCreateRemoteConfig()
        {
            JustTrackSDK.Init("", null, null, null, false, false, false, false, null, null, null);
            Assert.IsNull(JustTrackSDK.GetRemoteConfig());
        }

        [Test]
        public void Init_WithValidKey_SetsIsRunning()
        {
            Init();
            Assert.IsTrue(JustTrackSDK.IsRunning());
        }

        [Test]
        public void Init_WithValidKey_CreatesRemoteConfig()
        {
            Init();
            Assert.IsNotNull(JustTrackSDK.GetRemoteConfig());
        }

        [Test]
        public void Init_CalledTwice_DoesNotThrow()
        {
            Init();
            Assert.DoesNotThrow(Init);
        }

        [Test]
        public void GetVersion_ReturnsNonEmpty()
        {
            Assert.IsNotEmpty(JustTrackSDK.GetVersion());
        }

        [Test]
        public void GetRemoteConfig_BeforeInit_ReturnsNull()
        {
            Assert.IsNull(JustTrackSDK.GetRemoteConfig());
        }

        [Test]
        public void GetPreliminaryRetargetingParameters_BeforeInit_ReturnsNull()
        {
            Assert.IsNull(JustTrackSDK.GetPreliminaryRetargetingParameters());
        }

        [Test]
        public void Track_EmptyName_InvokesOnFailure()
        {
            Init();
            string? error = null;
            JustTrackSDK.Track("", () => Assert.Fail(), err => error = err);
            Assert.IsNotNull(error);
        }

        [Test]
        public void Track_ValidName_InvokesOnSuccess()
        {
            Init();
            bool success = false;
            JustTrackSDK.Track("my_event", () => success = true, _ => Assert.Fail());
            Assert.IsTrue(success);
        }

        [Test]
        public void Track_NoCallback_EmptyName_DoesNotThrow()
        {
            Init();
            Assert.DoesNotThrow(() => JustTrackSDK.Track(""));
        }

        [Test]
        public void Track_WithDimensions_EmptyName_InvokesOnFailure()
        {
            Init();
            string? error = null;
            JustTrackSDK.Track("", new Dictionary<string, string> { ["k"] = "v" }, () => Assert.Fail(), err => error = err);
            Assert.IsNotNull(error);
        }

        [Test]
        public void Track_WithDimensions_ValidName_InvokesOnSuccess()
        {
            Init();
            bool success = false;
            JustTrackSDK.Track("dim_event", new Dictionary<string, string> { ["k"] = "v" }, () => success = true, _ => Assert.Fail());
            Assert.IsTrue(success);
        }

        [Test]
        public void Track_WithDictionary_NoCallback_EmptyName_DoesNotThrow()
        {
            Init();
            Assert.DoesNotThrow(() => JustTrackSDK.Track("", new Dictionary<string, string>()));
        }

        [Test]
        public void ForwardAdImpression_NegativeRevenue_InvokesOnFailure()
        {
            Init();
            string? error = null;
            JustTrackSDK.ForwardAdImpression(
                new AdImpression("unit", "sdk").SetRevenue(new Money(-1.0, "USD")),
                () => Assert.Fail(), err => error = err);
            Assert.IsNotNull(error);
        }

        [Test]
        public void ForwardAdImpression_InvalidCurrencyLength_InvokesOnFailure()
        {
            Init();
            string? error = null;
            JustTrackSDK.ForwardAdImpression(
                new AdImpression("unit", "sdk").SetRevenue(new Money(1.0, "US")),
                () => Assert.Fail(), err => error = err);
            Assert.IsNotNull(error);
        }

        [Test]
        public void ForwardAdImpression_ValidRevenue_DoesNotThrow()
        {
            Init();
            Assert.DoesNotThrow(() =>
                JustTrackSDK.ForwardAdImpression(new AdImpression("unit", "sdk").SetRevenue(new Money(0.5, "USD"))));
        }

        [Test]
        public void ForwardAdImpression_NullRevenue_DoesNotThrow()
        {
            Init();
            Assert.DoesNotThrow(() => JustTrackSDK.ForwardAdImpression(new AdImpression("unit", "sdk")));
        }

        [Test]
        [Obsolete]
        public void PublishEvent_EmptyName_InvokesOnFailure()
        {
            Init();
            string? error = null;
            JustTrackSDK.PublishEvent("", () => Assert.Fail(), err => error = err);
            Assert.IsNotNull(error);
        }

        [Test]
        [Obsolete]
        public void PublishEvent_NoCallback_EmptyName_DoesNotThrow()
        {
            Init();
            Assert.DoesNotThrow(() => JustTrackSDK.PublishEvent(""));
        }

        [Test]
        public void OnAttributionResponse_AddAndRemoveHandler_DoesNotThrow()
        {
            Init();
            Action<AttributionResponse> h = _ => { };
            Assert.DoesNotThrow(() => JustTrackSDK.OnAttributionResponse += h);
            Assert.DoesNotThrow(() => JustTrackSDK.OnAttributionResponse -= h);
        }

        [Test]
        public void OnRetargetingParameters_AddAndRemoveHandler_DoesNotThrow()
        {
            Init();
            Action<RetargetingParameters> h = _ => { };
            Assert.DoesNotThrow(() => JustTrackSDK.OnRetargetingParameters += h);
            Assert.DoesNotThrow(() => JustTrackSDK.OnRetargetingParameters -= h);
        }

        [Test]
        public void OnPreliminaryRetargetingParameters_AddAndRemoveHandler_DoesNotThrow()
        {
            Init();
            Action<PreliminaryRetargetingParameters> h = _ => { };
            Assert.DoesNotThrow(() => JustTrackSDK.OnPreliminaryRetargetingParameters += h);
            Assert.DoesNotThrow(() => JustTrackSDK.OnPreliminaryRetargetingParameters -= h);
        }

        [Test]
        public void SetGlobalDimension0_WithValue_DoesNotThrow()
        {
            Init();
            Assert.DoesNotThrow(() => JustTrackSDK.SetGlobalDimension0("warrior"));
        }

        [Test]
        public void SetGlobalDimension0_WithNull_DoesNotThrow()
        {
            Init();
            Assert.DoesNotThrow(() => JustTrackSDK.SetGlobalDimension0(null));
        }

        [Test]
        public void SetGlobalDimension1_WithValue_DoesNotThrow()
        {
            Init();
            Assert.DoesNotThrow(() => JustTrackSDK.SetGlobalDimension1("experiment_a"));
        }

        [Test]
        public void SetGlobalDimension1_WithNull_DoesNotThrow()
        {
            Init();
            Assert.DoesNotThrow(() => JustTrackSDK.SetGlobalDimension1(null));
        }

        [Test]
        public void SetGlobalDimension2_WithValue_DoesNotThrow()
        {
            Init();
            Assert.DoesNotThrow(() => JustTrackSDK.SetGlobalDimension2("level_5"));
        }

        [Test]
        public void SetGlobalDimension2_WithNull_DoesNotThrow()
        {
            Init();
            Assert.DoesNotThrow(() => JustTrackSDK.SetGlobalDimension2(null));
        }
    }
}
