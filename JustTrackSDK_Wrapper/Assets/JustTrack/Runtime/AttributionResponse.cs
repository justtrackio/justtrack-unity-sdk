using System;
#if UNITY_ANDROID
using UnityEngine;
#endif

namespace JustTrack
{
    /// <summary>
    /// A representation of the attribution returned by the server.
    /// </summary>
    public class AttributionResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributionResponse"/> class.
        /// </summary>
        /// <param name="pCampaign">The attribution campaign.</param>
        /// <param name="pUserType">The user type.</param>
        /// <param name="pType">The attribution type.</param>
        /// <param name="pChannel">The attribution channel.</param>
        /// <param name="pPartner">The attribution partner.</param>
        /// <param name="pSourceId">The source ID.</param>
        /// <param name="pSourceBundleId">The source bundle ID.</param>
        /// <param name="pSourcePlacement">The source placement.</param>
        /// <param name="pAdsetId">The ad set ID.</param>
        /// <param name="pCreatedAt">The creation date and time.</param>
        private AttributionResponse(AttributionCampaign pCampaign, string pUserType, string pType, AttributionChannel pChannel, AttributionPartner pPartner, string? pSourceId, string? pSourceBundleId, string? pSourcePlacement, string? pAdsetId, DateTime pCreatedAt)
        {
            this.Campaign = pCampaign;
            this.UserType = pUserType;
            this.Type = pType;
            this.Channel = pChannel;
            this.Partner = pPartner;
            this.SourceId = pSourceId;
            this.SourceBundleId = pSourceBundleId;
            this.SourcePlacement = pSourcePlacement;
            this.AdsetId = pAdsetId;
            this.CreatedAt = pCreatedAt;
        }

        /// <summary>
        /// Gets the attribution campaign.
        /// </summary>
        public AttributionCampaign Campaign { get; private set; }

        /// <summary>
        /// Gets the user type for this attribution.
        /// </summary>
        public string UserType { get; private set; }

        /// <summary>
        /// Gets the attribution type.
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Gets the attribution channel.
        /// </summary>
        public AttributionChannel Channel { get; private set; }

        /// <summary>
        /// Gets the attribution partner.
        /// </summary>
        public AttributionPartner Partner { get; private set; }

        /// <summary>
        /// Gets the source ID for this attribution.
        /// </summary>
        public string? SourceId { get; private set; }

        /// <summary>
        /// Gets the source bundle ID for this attribution.
        /// </summary>
        public string? SourceBundleId { get; private set; }

        /// <summary>
        /// Gets the source placement for this attribution.
        /// </summary>
        public string? SourcePlacement { get; private set; }

        /// <summary>
        /// Gets the ad set ID for this attribution.
        /// </summary>
        public string? AdsetId { get; private set; }

        /// <summary>
        /// Gets the creation date and time of this attribution.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

#if UNITY_ANDROID
            internal static AttributionResponse FromAndroidObject(AndroidJavaObject pResponseObject) {
                using var campaign = pResponseObject.Call<AndroidJavaObject>("getCampaign");
                using var channel = pResponseObject.Call<AndroidJavaObject>("getChannel");
                using var partner = pResponseObject.Call<AndroidJavaObject>("getPartner");
                using var createdAt = pResponseObject.Call<AndroidJavaObject>("getCreatedAt");

                return new AttributionResponse(
                    AttributionCampaign.FromAndroidObject(campaign),
                    pResponseObject.Call<string>("getUserType"),
                    pResponseObject.Call<string>("getType"),
                    AttributionChannel.FromAndroidObject(channel),
                    AttributionPartner.FromAndroidObject(partner),
                    pResponseObject.Call<string?>("getSourceId"),
                    pResponseObject.Call<string?>("getSourceBundleId"),
                    pResponseObject.Call<string?>("getSourcePlacement"),
                    pResponseObject.Call<string?>("getAdsetId"),
                    new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + new TimeSpan(createdAt.Call<long>("getTime") * 10000)
                );
            }
#endif
#if UNITY_IOS || UNITY_WEBGL
            internal static AttributionResponse CreateResponse(string userType, string type, int campaignId, string campaignName, string campaignType, int channelId, string channelName, bool channelIncent, int partnerId, string partnerName, string? sourceId, string? sourceBundleId, string? sourcePlacement, string? adsetId, DateTime createdAt) {
                return new AttributionResponse(
                    AttributionCampaign.CreateCampaign(campaignId, campaignName, campaignType),
                    userType,
                    type,
                    AttributionChannel.CreateChannel(channelId, channelName, channelIncent),
                    AttributionPartner.CreatePartner(partnerId, partnerName),
                    sourceId,
                    sourceBundleId,
                    sourcePlacement,
                    adsetId,
                    createdAt
                );
            }
#endif
#if UNITY_EDITOR
            internal static AttributionResponse CreateFakeResponse() {
                return new AttributionResponse(
                    AttributionCampaign.CreateFakeCampaign(1, "fake campaign", "acquisition"),
                    "acquisition",
                    "fake",
                    AttributionChannel.CreateFakeChannel(1, "Direct", false),
                    AttributionPartner.CreateFakePartner(1, "Organic"),
                    "fake source id",
                    "fake source bundle id",
                    "fake source placemenet",
                    "fake adset id",
                    DateTime.UtcNow
                );
            }
#endif
    }
}
