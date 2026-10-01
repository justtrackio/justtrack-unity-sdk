#if UNITY_ANDROID
using UnityEngine;
#endif

namespace JustTrack
{
    /// <summary>
    /// A representation of a campaign.
    /// </summary>
    public class AttributionCampaign
    {
        private AttributionCampaign(string pId, string pName, string pType)
        {
            this.Id = pId;
            this.Name = pName;
            this.Type = pType;
        }

        /// <summary>
        /// Gets the ID of the campaign.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the name of the campaign.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of the campaign.
        /// </summary>
        public string Type { get; private set; }

#if UNITY_ANDROID
            internal static AttributionCampaign FromAndroidObject(AndroidJavaObject pCampaign) {
                return new AttributionCampaign(
                    pCampaign.Call<string>("getId"),
                    pCampaign.Call<string>("getName"),
                    pCampaign.Call<string>("getType")
                );
            }
#endif
#if UNITY_IOS || UNITY_WEBGL
            internal static AttributionCampaign CreateCampaign(string pId, string pName, string pType) {
                return new AttributionCampaign(pId, pName, pType);
            }
#endif
#if UNITY_EDITOR
            internal static AttributionCampaign CreateFakeCampaign(string pId, string pName, string pType) {
                return new AttributionCampaign(pId, pName, pType);
            }
#endif
    }
}