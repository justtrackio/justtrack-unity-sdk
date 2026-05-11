#if UNITY_ANDROID
using System;
using UnityEngine;

namespace JustTrack {
    internal class Callback : AndroidJavaProxy {
        private const string Class = "Callback";

        internal Callback(Action<AndroidJavaObject> pResolve, Action<string> pReject) : base($"{SDKAndroidAgent.Package}.{Class}") {
            m_OnResolve = pResolve;
            m_OnFailure = pReject;
        }

        Action<AndroidJavaObject> m_OnResolve;
        Action<string> m_OnFailure;

        /**
        * Called after the operation was successful.
        *
        * @param pResponse The data the operation produced.
        */
        void resolve(AndroidJavaObject pResponse) {
            using (pResponse) {
                m_OnResolve(pResponse);
            }
        }

        /**
        * Called in case an error which cannot be handled occurs. If this method is called, {@link #resolve(Object)}
        * will not be called anymore.
        *
        * @param pException The error which occurred.
        */
        void reject(AndroidJavaObject pException) {
            using (pException) {
                m_OnFailure.Invoke(pException.Call<string>("toString"));
            }
        }
    }
}
#endif
