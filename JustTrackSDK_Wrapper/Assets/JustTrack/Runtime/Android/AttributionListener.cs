#if UNITY_ANDROID
using System;
using UnityEngine;

namespace JustTrack {
    internal class AttributionListener : AndroidJavaProxy {
        private const string Class = "AttributionListener";

        internal AttributionListener(Action<AndroidJavaObject> pOnAttributionReceived) : base($"{SDKAndroidAgent.Package}.{Class}") {
            m_OnAttributionReceived = pOnAttributionReceived;
        }

        Action<AndroidJavaObject> m_OnAttributionReceived;

        /**
        * Called every time we retrieve a new attribution.
        *
        * @param attribution The current attribution of the user.
        */
        void onAttributionReceived(AndroidJavaObject pAttribution) {
            using (pAttribution) {
                m_OnAttributionReceived(pAttribution);
            }
        }
    }
}
#endif
