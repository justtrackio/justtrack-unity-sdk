#if UNITY_ANDROID
using UnityEngine;
#endif

namespace JustTrack
{
    /// <summary>
    /// Represents the result of a validation operation in the justtrack SDK.
    /// </summary>
    public class ValidateResult
    {
        private ValidateResult(bool pValid, RetargetingParameters? pValidParameters, AttributionResponse pAttributionResponse)
        {
            this.Valid = pValid;
            this.ValidParameters = pValidParameters;
            this.AttributionResponse = pAttributionResponse;
        }

        /// <summary>
        /// Gets a value indicating whether the validation was successful.
        /// </summary>
        public bool Valid { get; private set; }

        /// <summary>
        /// Gets the valid retargeting parameters if validation was successful.
        /// </summary>
        public RetargetingParameters? ValidParameters { get; private set; }

        /// <summary>
        /// Gets the attribution response associated with the validation.
        /// </summary>
        public AttributionResponse AttributionResponse { get; private set; }

#if UNITY_ANDROID
            internal static ValidateResult FromAndroidObject(AndroidJavaObject pObject) {
                using var validParameters = pObject.Call<AndroidJavaObject>("validParameters");
                using var response = pObject.Call<AndroidJavaObject>("attributionResponse");

                return new ValidateResult(
                    pObject.Call<bool>("isValid"),
                    validParameters == null ? null : RetargetingParameters.FromAndroidObject(validParameters),
                    AttributionResponse.FromAndroidObject(response)
                );
            }
#endif
#if UNITY_IOS
            internal static ValidateResult CreateValidateResult(RetargetingParameters? pValidParameters, AttributionResponse pAttributionResponse) {
                return new ValidateResult(pValidParameters != null, pValidParameters, pAttributionResponse);
            }
#endif
#if UNITY_EDITOR
            internal static ValidateResult CreateFakeResult() {
                return new ValidateResult(false, null, AttributionResponse.CreateFakeResponse());
            }
#endif
    }
}
