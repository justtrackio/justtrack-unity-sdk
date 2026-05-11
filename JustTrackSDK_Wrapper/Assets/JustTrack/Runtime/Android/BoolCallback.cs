#if UNITY_ANDROID
using System;
using UnityEngine;
namespace JustTrack
{
    internal class BoolCallback : AndroidJavaProxy {
        private const string Class = "Callback";

        internal BoolCallback(Action<bool?> pResolve, Action<string> pReject) : base($"{SDKAndroidAgent.Package}.{Class}") {
            m_OnResolve = pResolve;
            m_OnFailure = pReject;
        }

        Action<bool?> m_OnResolve;
        Action<string> m_OnFailure;

        /**
        * Called after the operation completes successfully.
        *
        * @param pResponse The data produced by the operation.
        */
        void resolve(bool? pResponse) {
            m_OnResolve(pResponse);
        }

        void resolve(AndroidJavaObject pResponse) {
            using (pResponse) {
                if (pResponse == null) {
                    m_OnResolve(null);
                } else {
                    m_OnResolve(pResponse.Call<bool>("booleanValue"));
                }
            }
        }

        /**
        * Called when an unhandleable error occurs. After this method is called, {@link #resolve(Object)}
        * will not be called.
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