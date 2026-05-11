namespace JustTrack
{
    /// <summary>
    /// You can use this event to track all details related to the progression of the user in the game.
    /// </summary>
    public class JtProgressionEvent : AppEvent
    {
        /// <summary>
        /// Represents progression actions that can be tracked.
        /// </summary>
        public enum Action
        {
            /// <summary>
            /// Start action
            /// </summary>
            START,

            /// <summary>
            /// Complete action
            /// </summary>
            COMPLETE,

            /// <summary>
            /// Fail action
            /// </summary>
            FAIL,
        }

        private static string EncodeAction(Action pJtAction)
        {
            switch (pJtAction)
            {
                case Action.START:
                    return "start";
                case Action.COMPLETE:
                    return "complete";
                case Action.FAIL:
                    return "fail";
                default:
                    return pJtAction.ToString().ToLowerInvariant();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JtProgressionEvent"/> class.
        /// </summary>
        /// <param name="pJtAction">The progression action.</param>
        /// <param name="pJtProgression1">Optional first progression level.</param>
        /// <param name="pJtProgression2">Optional second progression level.</param>
        /// <param name="pJtProgression3">Optional third progression level.</param>
        public JtProgressionEvent(Action pJtAction, string? pJtProgression1 = null, string? pJtProgression2 = null, string? pJtProgression3 = null)
            : this(EncodeAction(pJtAction), pJtProgression1, pJtProgression2, pJtProgression3)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JtProgressionEvent"/> class with duration.
        /// </summary>
        /// <param name="pJtAction">The progression action.</param>
        /// <param name="pDuration">The duration value.</param>
        /// <param name="pUnit">The time unit for the duration.</param>
        /// <param name="pJtProgression1">Optional first progression level.</param>
        /// <param name="pJtProgression2">Optional second progression level.</param>
        /// <param name="pJtProgression3">Optional third progression level.</param>
        public JtProgressionEvent(Action pJtAction, double pDuration, TimeUnitGroup pUnit, string? pJtProgression1 = null, string? pJtProgression2 = null, string? pJtProgression3 = null)
            : this(EncodeAction(pJtAction), pDuration, pUnit, pJtProgression1, pJtProgression2, pJtProgression3)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JtProgressionEvent"/> class.
        /// </summary>
        /// <param name="pJtAction">The progression action.</param>
        /// <param name="pJtProgression1">Optional first progression level.</param>
        /// <param name="pJtProgression2">Optional second progression level.</param>
        /// <param name="pJtProgression3">Optional third progression level.</param>
        public JtProgressionEvent(string pJtAction, string? pJtProgression1 = null, string? pJtProgression2 = null, string? pJtProgression3 = null)
            : base("jt_progression")
        {
            if (pJtAction != null)
            {
                this.AddDimension(Dimension.JT_ACTION, pJtAction);
            }

            if (pJtProgression1 != null)
            {
                this.AddDimension(Dimension.JT_PROGRESSION_1, pJtProgression1);
            }

            if (pJtProgression2 != null)
            {
                this.AddDimension(Dimension.JT_PROGRESSION_2, pJtProgression2);
            }

            if (pJtProgression3 != null)
            {
                this.AddDimension(Dimension.JT_PROGRESSION_3, pJtProgression3);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JtProgressionEvent"/> class with duration.
        /// </summary>
        /// <param name="pJtAction">The progression action.</param>
        /// <param name="pDuration">The duration value.</param>
        /// <param name="pUnit">The time unit for the duration.</param>
        /// <param name="pJtProgression1">Optional first progression level.</param>
        /// <param name="pJtProgression2">Optional second progression level.</param>
        /// <param name="pJtProgression3">Optional third progression level.</param>
        public JtProgressionEvent(string pJtAction, double pDuration, TimeUnitGroup pUnit, string? pJtProgression1 = null, string? pJtProgression2 = null, string? pJtProgression3 = null)
            : base("jt_progression")
        {
            if (pJtAction != null)
            {
                this.AddDimension(Dimension.JT_ACTION, pJtAction);
            }

            if (pJtProgression1 != null)
            {
                this.AddDimension(Dimension.JT_PROGRESSION_1, pJtProgression1);
            }

            if (pJtProgression2 != null)
            {
                this.AddDimension(Dimension.JT_PROGRESSION_2, pJtProgression2);
            }

            if (pJtProgression3 != null)
            {
                this.AddDimension(Dimension.JT_PROGRESSION_3, pJtProgression3);
            }

            this.SetValue(pDuration, TimeUnitGroupConversions.ToUnit(pUnit));
        }
    }

    /// <summary>
    /// You can use this event to capture details of items removed from the user's inventory. It could be either weapons or in-app currency.
    /// </summary>
    public class JtResourceEvent : AppEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JtResourceEvent"/> class.
        /// </summary>
        /// <param name="pJtAction">The resource action.</param>
        /// <param name="pJtItemType">Optional item type.</param>
        /// <param name="pJtItemName">Optional item name.</param>
        /// <param name="pJtItemId">Optional item ID.</param>
        public JtResourceEvent(string pJtAction, string? pJtItemType = null, string? pJtItemName = null, string? pJtItemId = null)
            : base("jt_resource")
        {
            if (pJtAction != null)
            {
                this.AddDimension(Dimension.JT_ACTION, pJtAction);
            }

            if (pJtItemType != null)
            {
                this.AddDimension(Dimension.JT_ITEM_TYPE, pJtItemType);
            }

            if (pJtItemName != null)
            {
                this.AddDimension(Dimension.JT_ITEM_NAME, pJtItemName);
            }

            if (pJtItemId != null)
            {
                this.AddDimension(Dimension.JT_ITEM_ID, pJtItemId);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JtResourceEvent"/> class with count.
        /// </summary>
        /// <param name="pJtAction">The resource action.</param>
        /// <param name="pCount">The count value.</param>
        /// <param name="pJtItemType">Optional item type.</param>
        /// <param name="pJtItemName">Optional item name.</param>
        /// <param name="pJtItemId">Optional item ID.</param>
        public JtResourceEvent(string pJtAction, double pCount, string? pJtItemType = null, string? pJtItemName = null, string? pJtItemId = null)
            : base("jt_resource")
        {
            if (pJtAction != null)
            {
                this.AddDimension(Dimension.JT_ACTION, pJtAction);
            }

            if (pJtItemType != null)
            {
                this.AddDimension(Dimension.JT_ITEM_TYPE, pJtItemType);
            }

            if (pJtItemName != null)
            {
                this.AddDimension(Dimension.JT_ITEM_NAME, pJtItemName);
            }

            if (pJtItemId != null)
            {
                this.AddDimension(Dimension.JT_ITEM_ID, pJtItemId);
            }

            this.SetValue(pCount, JustTrack.Unit.Count);
        }
    }

    /// <summary>
    /// Event that tracks and reports in-app purchase related details e.g. user selected some item from store, confirmed purchase, added to cart etc. In case of a successful purchase or subscription justtrack automatically tracks and reports the event so there is no need to generate that event.
    /// </summary>
    public class JtPurchaseEvent : AppEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JtPurchaseEvent"/> class.
        /// </summary>
        /// <param name="pJtAction">The purchase action.</param>
        /// <param name="pJtProductId">The product ID.</param>
        /// <param name="pJtProductType">The product type.</param>
        /// <param name="pJtToken">Optional token for the purchase.</param>
        public JtPurchaseEvent(string pJtAction, string? pJtProductId, string? pJtProductType, string? pJtToken = null)
            : base("jt_purchase")
        {
            if (pJtAction != null)
            {
                this.AddDimension(Dimension.JT_ACTION, pJtAction);
            }

            if (pJtProductId != null)
            {
                this.AddDimension(Dimension.JT_PRODUCT_ID, pJtProductId);
            }

            if (pJtToken != null)
            {
                this.AddDimension(Dimension.JT_TOKEN, pJtToken);
            }

            if (pJtProductType != null)
            {
                this.AddDimension(Dimension.JT_PRODUCT_TYPE, pJtProductType);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JtPurchaseEvent"/> class with count.
        /// </summary>
        /// <param name="pJtAction">The purchase action.</param>
        /// <param name="pJtProductId">The product ID.</param>
        /// <param name="pJtProductType">The product type.</param>
        /// <param name="pCount">The count value.</param>
        /// <param name="pJtToken">Optional token for the purchase.</param>
        public JtPurchaseEvent(string pJtAction, string? pJtProductId, string? pJtProductType, double pCount, string? pJtToken = null)
            : base("jt_purchase")
        {
            if (pJtAction != null)
            {
                this.AddDimension(Dimension.JT_ACTION, pJtAction);
            }

            if (pJtProductId != null)
            {
                this.AddDimension(Dimension.JT_PRODUCT_ID, pJtProductId);
            }

            if (pJtToken != null)
            {
                this.AddDimension(Dimension.JT_TOKEN, pJtToken);
            }

            if (pJtProductType != null)
            {
                this.AddDimension(Dimension.JT_PRODUCT_TYPE, pJtProductType);
            }

            this.SetValue(pCount, JustTrack.Unit.Count);
        }
    }

    /// <summary>
    /// You can use this event to capture details of an ad like load, click, show etc. Ad impressions are automatically tracked by justtrack SDK for integrated networks. For networks not integrated you can use forwardAdImpression().
    /// </summary>
    public class JtAdEvent : AppEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JtAdEvent"/> class.
        /// </summary>
        /// <param name="pJtAction">The ad action.</param>
        /// <param name="pJtAdBundleId">Optional ad bundle ID.</param>
        /// <param name="pJtAdInstanceName">Optional ad instance name.</param>
        /// <param name="pJtAdNetwork">Optional ad network.</param>
        /// <param name="pJtAdPlacement">Optional ad placement.</param>
        /// <param name="pJtAdSdk">Optional ad SDK.</param>
        /// <param name="pJtAdSegment">Optional ad segment.</param>
        /// <param name="pJtAdUnit">Optional ad unit.</param>
        /// <param name="pJtAdTestGroup">Optional ad test group.</param>
        public JtAdEvent(string pJtAction, string? pJtAdBundleId = null, string? pJtAdInstanceName = null, string? pJtAdNetwork = null, string? pJtAdPlacement = null, string? pJtAdSdk = null, string? pJtAdSegment = null, string? pJtAdUnit = null, string? pJtAdTestGroup = null)
            : base("jt_ad")
        {
            if (pJtAction != null)
            {
                this.AddDimension(Dimension.JT_ACTION, pJtAction);
            }

            if (pJtAdBundleId != null)
            {
                this.AddDimension(Dimension.JT_AD_BUNDLE_ID, pJtAdBundleId);
            }

            if (pJtAdInstanceName != null)
            {
                this.AddDimension(Dimension.JT_AD_INSTANCE_NAME, pJtAdInstanceName);
            }

            if (pJtAdNetwork != null)
            {
                this.AddDimension(Dimension.JT_AD_NETWORK, pJtAdNetwork);
            }

            if (pJtAdPlacement != null)
            {
                this.AddDimension(Dimension.JT_AD_PLACEMENT, pJtAdPlacement);
            }

            if (pJtAdSdk != null)
            {
                this.AddDimension(Dimension.JT_AD_SDK, pJtAdSdk);
            }

            if (pJtAdSegment != null)
            {
                this.AddDimension(Dimension.JT_AD_SEGMENT, pJtAdSegment);
            }

            if (pJtAdUnit != null)
            {
                this.AddDimension(Dimension.JT_AD_UNIT, pJtAdUnit);
            }

            if (pJtAdTestGroup != null)
            {
                this.AddDimension(Dimension.JT_AD_TEST_GROUP, pJtAdTestGroup);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JtAdEvent"/> class with duration.
        /// </summary>
        /// <param name="pJtAction">The ad action.</param>
        /// <param name="pDuration">The duration value.</param>
        /// <param name="pUnit">The time unit for the duration.</param>
        /// <param name="pJtAdBundleId">Optional ad bundle ID.</param>
        /// <param name="pJtAdInstanceName">Optional ad instance name.</param>
        /// <param name="pJtAdNetwork">Optional ad network.</param>
        /// <param name="pJtAdPlacement">Optional ad placement.</param>
        /// <param name="pJtAdSdk">Optional ad SDK.</param>
        /// <param name="pJtAdSegment">Optional ad segment.</param>
        /// <param name="pJtAdUnit">Optional ad unit.</param>
        /// <param name="pJtAdTestGroup">Optional ad test group.</param>
        public JtAdEvent(string pJtAction, double pDuration, TimeUnitGroup pUnit, string? pJtAdBundleId = null, string? pJtAdInstanceName = null, string? pJtAdNetwork = null, string? pJtAdPlacement = null, string? pJtAdSdk = null, string? pJtAdSegment = null, string? pJtAdUnit = null, string? pJtAdTestGroup = null)
            : base("jt_ad")
        {
            if (pJtAction != null)
            {
                this.AddDimension(Dimension.JT_ACTION, pJtAction);
            }

            if (pJtAdBundleId != null)
            {
                this.AddDimension(Dimension.JT_AD_BUNDLE_ID, pJtAdBundleId);
            }

            if (pJtAdInstanceName != null)
            {
                this.AddDimension(Dimension.JT_AD_INSTANCE_NAME, pJtAdInstanceName);
            }

            if (pJtAdNetwork != null)
            {
                this.AddDimension(Dimension.JT_AD_NETWORK, pJtAdNetwork);
            }

            if (pJtAdPlacement != null)
            {
                this.AddDimension(Dimension.JT_AD_PLACEMENT, pJtAdPlacement);
            }

            if (pJtAdSdk != null)
            {
                this.AddDimension(Dimension.JT_AD_SDK, pJtAdSdk);
            }

            if (pJtAdSegment != null)
            {
                this.AddDimension(Dimension.JT_AD_SEGMENT, pJtAdSegment);
            }

            if (pJtAdUnit != null)
            {
                this.AddDimension(Dimension.JT_AD_UNIT, pJtAdUnit);
            }

            if (pJtAdTestGroup != null)
            {
                this.AddDimension(Dimension.JT_AD_TEST_GROUP, pJtAdTestGroup);
            }

            this.SetValue(pDuration, TimeUnitGroupConversions.ToUnit(pUnit));
        }
    }

    /// <summary>
    /// To capture all details related to user login events.
    /// </summary>
    public class JtLoginEvent : AppEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JtLoginEvent"/> class.
        /// </summary>
        /// <param name="pJtAction">The login action.</param>
        /// <param name="pJtMethod">Optional login method.</param>
        public JtLoginEvent(string pJtAction, string? pJtMethod = null)
            : base("jt_login")
        {
            if (pJtAction != null)
            {
                this.AddDimension(Dimension.JT_ACTION, pJtAction);
            }

            if (pJtMethod != null)
            {
                this.AddDimension(Dimension.JT_METHOD, pJtMethod);
            }
        }
    }
}
