#if UNITY_ANDROID
using UnityEngine;
#endif

namespace JustTrack
{
    /// <summary>
    /// A representation of a partner.
    /// </summary>
    public class AttributionPartner
    {
        private AttributionPartner(int pId, string pName)
        {
            this.Id = pId;
            this.Name = pName;
        }

        /// <summary>
        /// Gets the unique identifier of the attribution partner.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the name of the attribution partner.
        /// </summary>
        public string Name { get; private set; }

#if UNITY_ANDROID
            internal static AttributionPartner FromAndroidObject(AndroidJavaObject pPartner) {
                return new AttributionPartner(
                    pPartner.Call<int>("getId"),
                    pPartner.Call<string>("getName")
                );
            }
#endif
#if UNITY_IOS || UNITY_WEBGL
            internal static AttributionPartner CreatePartner(int pId, string pName) {
                return new AttributionPartner(pId, pName);
            }
#endif
#if UNITY_EDITOR
            internal static AttributionPartner CreateFakePartner(int pId, string pName) {
                return new AttributionPartner(pId, pName);
            }
#endif
    }
}
