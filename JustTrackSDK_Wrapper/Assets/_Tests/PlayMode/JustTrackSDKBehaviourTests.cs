using System.Collections;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace JustTrack.Tests.PlayMode
{
    public class JustTrackSDKBehaviourTests
    {
        private static readonly FieldInfo UnityMainThreadIdField =
            typeof(JustTrackSDKBehaviour).GetField("unityMainThreadId", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo ActionsOnMainThreadField =
            typeof(JustTrackSDKBehaviour).GetField("actionsOnMainThead", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo RemoteConfigField =
            typeof(JustTrackSDK).GetField("remoteConfig", BindingFlags.Static | BindingFlags.NonPublic);

        private GameObject? _go;

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (_go != null)
            {
                UnityEngine.Object.Destroy(_go);
                _go = null;
            }

            // Reset static state so tests are independent
            UnityMainThreadIdField.SetValue(null, -1);
            ActionsOnMainThreadField.SetValue(null, null);
            yield return null;
        }

        #region IsOnMainThread

        [UnityTest]
        public IEnumerator IsOnMainThread_OnMainThread_ReturnsTrue()
        {
            CreateBehaviour();
            yield return null;

            Assert.IsTrue(JustTrackSDKBehaviour.IsOnMainThread());
        }

        [UnityTest]
        public IEnumerator IsOnMainThread_OnBackgroundThread_ReturnsFalse()
        {
            CreateBehaviour();
            yield return null;

            bool? result = null;
            var thread = new Thread(() => { result = JustTrackSDKBehaviour.IsOnMainThread(); });
            thread.Start();
            thread.Join();

            Assert.IsFalse(result);
        }

        #endregion

        #region CallOnMainThread and Update drain

        [UnityTest]
        public IEnumerator CallOnMainThread_ActionExecutedAfterOneFrame()
        {
            CreateBehaviour();
            yield return null;

            bool executed = false;
            JustTrackSDKBehaviour.CallOnMainThread(() => executed = true);

            // Action should not have executed yet (Update hasn't run)
            Assert.IsFalse(executed);
            yield return null;

            Assert.IsTrue(executed);
        }

        [UnityTest]
        public IEnumerator CallOnMainThread_MultipleActions_AllExecutedInOneFrame()
        {
            CreateBehaviour();
            yield return null;

            int count = 0;
            JustTrackSDKBehaviour.CallOnMainThread(() => count++);
            JustTrackSDKBehaviour.CallOnMainThread(() => count++);
            JustTrackSDKBehaviour.CallOnMainThread(() => count++);

            yield return null;

            Assert.AreEqual(3, count);
        }

        [UnityTest]
        public IEnumerator CallOnMainThread_QueueEmptyAfterDrain()
        {
            CreateBehaviour();
            yield return null;

            int count = 0;
            JustTrackSDKBehaviour.CallOnMainThread(() => count++);
            yield return null; // drains
            yield return null; // second frame should not re-execute

            Assert.AreEqual(1, count);
        }

        [UnityTest]
        public IEnumerator CallOnMainThread_FromBackgroundThread_ExecutesOnMainThread()
        {
            CreateBehaviour();
            yield return null;

            int executedOnThreadId = -1;
            int mainThreadId = Thread.CurrentThread.ManagedThreadId;

            var thread = new Thread(() =>
            {
                JustTrackSDKBehaviour.CallOnMainThread(() =>
                {
                    executedOnThreadId = Thread.CurrentThread.ManagedThreadId;
                });
            });
            thread.Start();
            thread.Join();

            yield return null;

            Assert.AreEqual(mainThreadId, executedOnThreadId);
        }

        [UnityTest]
        public IEnumerator CallOnMainThread_FromMultipleThreads_AllExecuted()
        {
            CreateBehaviour();
            yield return null;

            int count = 0;
            var threads = new Thread[5];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {
                    JustTrackSDKBehaviour.CallOnMainThread(() => Interlocked.Increment(ref count));
                });
                threads[i].Start();
            }

            foreach (var t in threads)
                t.Join();

            yield return null;

            Assert.AreEqual(5, count);
        }

        #endregion

        #region Awake - Initialization

        [UnityTest]
        public IEnumerator Awake_SetsUnityMainThreadId()
        {
            CreateBehaviour();
            yield return null;

            int storedId = (int)UnityMainThreadIdField.GetValue(null);
            Assert.AreEqual(Thread.CurrentThread.ManagedThreadId, storedId);
        }

        [UnityTest]
        public IEnumerator Awake_CalledTwice_IsNoOp()
        {
            CreateBehaviour();
            yield return null;

            // Create a second behaviour — its Awake should be a no-op
            var go2 = new GameObject("SecondBehaviour");
            go2.AddComponent<JustTrackSDKBehaviour>();
            yield return null;

            // The original GO's name should still be "JustTrackSDKBehaviour"
            // (set by the first Awake), and the second GO retains its original name
            Assert.AreEqual("SecondBehaviour", go2.name);
            UnityEngine.Object.Destroy(go2);
        }

        [UnityTest]
        public IEnumerator Awake_DetachesFromParent()
        {
            var parent = new GameObject("Parent");
            _go = new GameObject("Child");
            _go.transform.parent = parent.transform;
            _go.AddComponent<JustTrackSDKBehaviour>();
            yield return null;

            Assert.IsNull(_go.transform.parent);
            UnityEngine.Object.Destroy(parent);
        }

        [UnityTest]
        public IEnumerator Awake_SetsGameObjectName()
        {
            _go = new GameObject("OriginalName");
            _go.AddComponent<JustTrackSDKBehaviour>();
            yield return null;

            Assert.AreEqual("JustTrackSDKBehaviour", _go.name);
        }

        [UnityTest]
        public IEnumerator Awake_WithUseRuntimeConstructor_DoesNotInitSDK()
        {
            // Save and clear remoteConfig set by prior tests
            var savedRemoteConfig = RemoteConfigField.GetValue(null);
            RemoteConfigField.SetValue(null, null);

            // JustTrackSettings has UseRuntimeConstructor=true, so Awake should not call Init.
            // RemoteConfig is only created during Init, so it should remain null.
            CreateBehaviour();
            yield return null;

            Assert.IsNull(JustTrackSDK.GetRemoteConfig());

            // Restore for other tests
            RemoteConfigField.SetValue(null, savedRemoteConfig);
        }

        #endregion

        private void CreateBehaviour()
        {
            _go = new GameObject("TestBehaviour");
            _go.AddComponent<JustTrackSDKBehaviour>();
        }
    }
}
