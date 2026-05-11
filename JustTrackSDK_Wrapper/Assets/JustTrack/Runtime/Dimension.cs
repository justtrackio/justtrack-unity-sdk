namespace JustTrack
{
    /// <summary>
    /// Defines the available dimensions for event tracking in the justtrack SDK.
    /// </summary>
    public enum Dimension
    {
        JT_ACTION,
        JT_AD_BUNDLE_ID,
        JT_AD_INSTANCE_NAME,
        JT_AD_NETWORK,
        JT_AD_PLACEMENT,
        JT_AD_SDK,
        JT_AD_SEGMENT,
        JT_AD_TEST_GROUP,
        JT_AD_UNIT,
        JT_CATEGORY,
        JT_CONTEXT,
        JT_DETAIL,
        JT_ITEM_ID,
        JT_ITEM_NAME,
        JT_ITEM_TYPE,
        JT_LOCATION,
        JT_METHOD,
        JT_PRODUCT_ID,
        JT_PRODUCT_TYPE,
        JT_PROGRESSION_1,
        JT_PROGRESSION_2,
        JT_PROGRESSION_3,
        JT_STATE,
        JT_TOKEN,
        JT_TRIGGER,
        JT_URL,
    }

    /// <summary>
    /// Utility class for converting dimension enums to their string representations.
    /// </summary>
    internal static class DimensionConversions
    {
        /// <summary>
        /// Converts a Dimension enum value to its corresponding string representation.
        /// </summary>
        /// <param name="dimension">The dimension to convert.</param>
        /// <returns>The string representation of the dimension.</returns>
        internal static string DimensionToString(Dimension dimension)
        {
            return dimension switch
            {
                Dimension.JT_ACTION => "jt_action",
                Dimension.JT_AD_BUNDLE_ID => "jt_ad_bundle_id",
                Dimension.JT_AD_INSTANCE_NAME => "jt_ad_instance_name",
                Dimension.JT_AD_NETWORK => "jt_ad_network",
                Dimension.JT_AD_PLACEMENT => "jt_ad_placement",
                Dimension.JT_AD_SDK => "jt_ad_sdk",
                Dimension.JT_AD_SEGMENT => "jt_ad_segment",
                Dimension.JT_AD_TEST_GROUP => "jt_ad_test_group",
                Dimension.JT_AD_UNIT => "jt_ad_unit",
                Dimension.JT_CATEGORY => "jt_category",
                Dimension.JT_CONTEXT => "jt_context",
                Dimension.JT_DETAIL => "jt_detail",
                Dimension.JT_ITEM_ID => "jt_item_id",
                Dimension.JT_ITEM_NAME => "jt_item_name",
                Dimension.JT_ITEM_TYPE => "jt_item_type",
                Dimension.JT_LOCATION => "jt_location",
                Dimension.JT_METHOD => "jt_method",
                Dimension.JT_PRODUCT_ID => "jt_product_id",
                Dimension.JT_PRODUCT_TYPE => "jt_product_type",
                Dimension.JT_PROGRESSION_1 => "jt_progression_1",
                Dimension.JT_PROGRESSION_2 => "jt_progression_2",
                Dimension.JT_PROGRESSION_3 => "jt_progression_3",
                Dimension.JT_STATE => "jt_state",
                Dimension.JT_TOKEN => "jt_token",
                Dimension.JT_TRIGGER => "jt_trigger",
                Dimension.JT_URL => "jt_url",
                _ => string.Empty,
            };
        }
    }
}
