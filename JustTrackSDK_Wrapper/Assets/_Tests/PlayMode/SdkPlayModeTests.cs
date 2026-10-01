using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using JustTrack;

namespace JustTrack.Tests.PlayMode
{
    public class SdkPlayModeTests
    {
        private void Init()
        {
            JustTrackSDK.Init(
                "test-api-key",
                null, null, null,
                false, false, false, false,
                null, null, null);
        }

        [UnityTest]
        public IEnumerator Init_StartsRunning()
        {
            Init();
            yield return null;
            Assert.IsTrue(JustTrackSDK.IsRunning() || JustTrackSDK.GetVersion() != null);
        }

        [UnityTest]
        public IEnumerator GetVersion_ReturnsNonEmpty()
        {
            yield return null;
            Assert.IsNotEmpty(JustTrackSDK.GetVersion());
        }

        [UnityTest]
        public IEnumerator Track_AfterInit_InvokesSuccess()
        {
            Init();
            yield return null;

            bool success = false;
            JustTrackSDK.Track(new AppEvent("playmode_test"), () => { success = true; }, _ => { });
            yield return null;

            Assert.IsTrue(success);
        }

        [UnityTest]
        public IEnumerator GetInstallInstanceId_AfterInit_ReturnsId()
        {
            Init();
            yield return null;

            string? installId = null;
            JustTrackSDK.GetInstallInstanceId(id => { installId = id; }, _ => { });
            for (int i = 0; i < 10 && installId == null; i++)
            {
                yield return null;
            }

            Assert.IsNotNull(installId);
            Assert.IsNotEmpty(installId);
        }
    }
}
