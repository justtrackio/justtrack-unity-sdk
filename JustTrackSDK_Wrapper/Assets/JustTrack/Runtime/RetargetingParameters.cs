using System.Collections.Generic;
#if UNITY_ANDROID
using UnityEngine;
#endif

namespace JustTrack
{
    /// <summary>
    /// Contains parameters used for retargeting in the justtrack SDK.
    /// </summary>
    public class RetargetingParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RetargetingParameters"/> class.
        /// </summary>
        /// <param name="pWasAlreadyInstalled">A value indicating whether the app was already installed.</param>
        /// <param name="pUri">The URI parameter.</param>
        /// <param name="pParameters">The parameters dictionary.</param>
        /// <param name="pPromotionParameter">The promotion parameter.</param>
        protected RetargetingParameters(bool pWasAlreadyInstalled, string? pUri, Dictionary<string, string> pParameters, string? pPromotionParameter)
        {
            this.WasAlreadyInstalled = pWasAlreadyInstalled;
            this.Uri = pUri;
            this.Parameters = pParameters;
            this.PromotionParameter = pPromotionParameter;
        }

        /// <summary>
        /// Gets a value indicating whether the app was already installed.
        /// </summary>
        public bool WasAlreadyInstalled { get; private set; }

        /// <summary>
        /// Gets the URI parameter.
        /// </summary>
        public string? Uri { get; private set; }

        /// <summary>
        /// Gets the parameters dictionary.
        /// </summary>
        public Dictionary<string, string> Parameters { get; private set; }

        /// <summary>
        /// Gets the promotion parameter.
        /// </summary>
        public string? PromotionParameter { get; private set; }

#if UNITY_ANDROID
            internal static RetargetingParameters FromAndroidObject(AndroidJavaObject pObject) {
                using var uri = pObject.Call<AndroidJavaObject>("getUri");

                var dictionary = new Dictionary<string, string>();

                using var parameters = pObject.Call<AndroidJavaObject>("getParameters");
                using var entrySet = parameters.Call<AndroidJavaObject>("entrySet");
                using var parametersIterator = entrySet.Call<AndroidJavaObject>("iterator");
                while (parametersIterator.Call<bool>("hasNext")) {
                    using var entry = parametersIterator.Call<AndroidJavaObject>("next");
                    dictionary[entry.Call<string>("getKey")] = entry.Call<string>("getValue");
                }

                return new RetargetingParameters(
                    pObject.Call<bool>("wasAlreadyInstalled"),
                    uri == null ? null : uri.Call<string>("toString"),
                    dictionary,
                    pObject.Call<string>("getPromotionParameter")
                );
            }
#endif
#if UNITY_IOS
            internal static RetargetingParameters CreateRetargetingParameters(bool pWasAlreadyInstalled, string? pUri, Dictionary<string, string> pParameters, string? pPromotionParameter) {
                return new RetargetingParameters(pWasAlreadyInstalled, pUri, pParameters, pPromotionParameter);
            }
#endif
    }
}
