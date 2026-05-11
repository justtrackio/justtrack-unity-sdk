#if UNITY_ANDROID
using UnityEngine;
#endif

namespace JustTrack
{
    /// <summary>
    /// A representation of a channel.
    /// </summary>
    public class AttributionChannel
    {
        private AttributionChannel(int pId, string pName, bool pIncent)
        {
            this.Id = pId;
            this.Name = pName;
            this.IsIncent = pIncent;
        }

        /// <summary>
        /// Gets the unique identifier of the channel.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the name of the channel.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the channel is incentivized.
        /// </summary>
        public bool IsIncent { get; private set; }

#if UNITY_ANDROID
            internal static AttributionChannel FromAndroidObject(AndroidJavaObject pChannel) {
                return new AttributionChannel(
                    pChannel.Call<int>("getId"),
                    pChannel.Call<string>("getName"),
                    pChannel.Call<bool>("isIncent")
                );
            }
#endif
#if UNITY_IOS || UNITY_WEBGL
            internal static AttributionChannel CreateChannel(int pId, string pName, bool pIncent) {
                return new AttributionChannel(pId, pName, pIncent);
            }
#endif
#if UNITY_EDITOR
            internal static AttributionChannel CreateFakeChannel(int pId, string pName, bool pIncent) {
                return new AttributionChannel(pId, pName, pIncent);
            }
#endif
    }
}