#include "JustTrackSDKUnity.h"
// MARK: Other bridging headers
// here we would insert the bridging headers from other unity packages (like NiceVibrations)
// MARK: Swift bridging header
#import <UnityFramework/UnityFramework-Swift.h>

static char *_justtrack_sdk_rp_alloc_string(NSString *s) {
    if (s == NULL) {
        return NULL;
    }

    return strdup([s UTF8String]);
}

extern "C" {
    void _justtrack_sdk_rp_free_string(char *str) {
        free(str);
    }

    void _justtrack_sdk_rp_init(const char *apiToken, const char *trackingId, const char *trackingProvider, const char *customUserId, int inactivityTimeFrameHours, int reAttributionTimeFrameDays, int reFetchAttributionDelaySeconds, int attributionRetryDelaySeconds, int automaticInAppPurchaseTracking, int manualStart, int enableConsoleLogging, const char *customBundleId, const char *customAppVersion, const char *customAppCode, const char *customServerUrl) {
        NSString *nsApiToken = [NSString stringWithUTF8String:apiToken];
        NSString *nsTrackingId = [NSString stringWithUTF8String:trackingId];
        NSString *nsTrackingProvider = [NSString stringWithUTF8String:trackingProvider];
        NSString *nsCustomUserId = [NSString stringWithUTF8String:customUserId];
        NSString *nsCustomBundleId = [NSString stringWithUTF8String:customBundleId];
        NSString *nsCustomAppVersion = [NSString stringWithUTF8String:customAppVersion];
        NSString *nsCustomAppCode = [NSString stringWithUTF8String:customAppCode];
        NSString *nsCustomServerUrl = [NSString stringWithUTF8String:customServerUrl];

        [[NativeBridge shared] initSdkWithApiToken: nsApiToken
                                        trackingId: nsTrackingId
                                  trackingProvider: nsTrackingProvider
                                      customUserId: nsCustomUserId
                          inactivityTimeFrameHours: (NSInteger) inactivityTimeFrameHours
                        reAttributionTimeFrameDays: (NSInteger) reAttributionTimeFrameDays
			  reFetchReAttributionDelaySeconds: (NSInteger) reFetchAttributionDelaySeconds
				  attributionRetryDelaySeconds: (NSInteger) attributionRetryDelaySeconds
				automaticInAppPurchaseTracking: automaticInAppPurchaseTracking
								   manualStart: manualStart
						  enableConsoleLogging: enableConsoleLogging
							    customBundleId: nsCustomBundleId
							  customAppVersion: nsCustomAppVersion
							     customAppCode: nsCustomAppCode
							   customServerUrl: nsCustomServerUrl];
	}

    void _justtrack_sdk_rp_start() {
        [[NativeBridge shared] start];
    }

    void _justtrack_sdk_rp_stop() {
        [[NativeBridge shared] stop];
    }

    void _justtrack_sdk_rp_anonymize() {
        [[NativeBridge shared] anonymize];
    }

    bool _justtrack_sdk_rp_is_running() {
        return [[NativeBridge shared] isRunning];
    }

    void _justtrack_sdk_rp_get_retargeting_parameters() {
        [[NativeBridge shared] getRetargetingParameters];
    }

    char *_justtrack_sdk_rp_get_preliminary_retargeting_parameters() {
        NSString *s = [[NativeBridge shared] getPreliminaryRetargetingParameters];

        return _justtrack_sdk_rp_alloc_string(s);
    }

    void _justtrack_sdk_rp_set_user_id(const char *userId) {
        NSString *nsUserId = [NSString stringWithUTF8String:userId];

        [[NativeBridge shared] setWithUserId: nsUserId];
    }

    void _justtrack_sdk_rp_set_automatic_in_app_purchase_tracking(bool automaticInAppPurchaseTracking) {
        [[NativeBridge shared] setWithAutomaticInAppPurchaseTracking: automaticInAppPurchaseTracking];
    }

    void _justtrack_sdk_rp_set_firebase_app_instance_id(const char *firebaseAppInstanceId) {
        NSString *nsFirebaseAppInstanceId = [NSString stringWithUTF8String:firebaseAppInstanceId];

        [[NativeBridge shared] setWithFirebaseAppInstanceId:nsFirebaseAppInstanceId];
    }

    void _justtrack_sdk_rp_publish_event(const char *name, const char *dimensions, double value, const char *unit, const char *currency, const char *requestId) {
        NSString *nsName = [NSString stringWithUTF8String:name];
        NSString *nsDimensions = [NSString stringWithUTF8String:dimensions];
		NSString *nsUnit = unit == NULL ? nil : [NSString stringWithUTF8String:unit];
        NSString *nsCurrency = currency == NULL ? nil : [NSString stringWithUTF8String:currency];
        NSString *nsRequestId = requestId == NULL ? nil : [NSString stringWithUTF8String:requestId];

        [[NativeBridge shared] publishEventWithName: nsName
                                         dimensions: nsDimensions
                                              value: value
											   unit: nsUnit
                                           currency: nsCurrency
                                          requestId: nsRequestId];
    }

    void _justtrack_sdk_rp_integrate_with_app_lovin(const char *customUserId) {
        NSString *nsCustomUserId = customUserId == NULL ? NULL : [NSString stringWithUTF8String:customUserId];

		[[NativeBridge shared] integrateWithAppLovinWithCustomUserId:nsCustomUserId];
    }

	void _justtrack_sdk_rp_integrate_with_firebase() {
		[[NativeBridge shared] integrateWithFirebase];
	}

	void _justtrack_sdk_rp_integrate_with_iron_source(const char *customUserId) {
		NSString *nsCustomUserId = customUserId == NULL ? NULL : [NSString stringWithUTF8String:customUserId];

		[[NativeBridge shared] integrateWithIronSourceWithCustomUserId:nsCustomUserId];
	}

    void _justtrack_sdk_rp_integrate_with_unity_ads() {
        [[NativeBridge shared] integrateWithUnityAds];
    }

    void _justtrack_sdk_rp_integrate_with_google_odm() {
        [[NativeBridge shared] integrateWithGoogleOdm];
    }

    void _justtrack_sdk_rp_forward_ad_impression(const char *adUnit, const char *adSdkName, const char *adNetwork, const char *placement, const char *testGroup, const char *segmentName, const char *instanceName, const char *bundleId, double revenue, const char *currency) {
        NSString *nsAdUnit = adUnit != NULL ? [NSString stringWithUTF8String:adUnit] : NULL;
        NSString *nsAdSdkName = adSdkName != NULL ? [NSString stringWithUTF8String:adSdkName] : NULL;
        NSString *nsAdNetwork = adNetwork != NULL ? [NSString stringWithUTF8String:adNetwork] : NULL;
        NSString *nsPlacement = placement != NULL ? [NSString stringWithUTF8String:placement] : NULL;
        NSString *nsTestGroup = testGroup != NULL ? [NSString stringWithUTF8String:testGroup] : NULL;
        NSString *nsSegmentName = segmentName != NULL ? [NSString stringWithUTF8String:segmentName] : NULL;
        NSString *nsInstanceName = instanceName != NULL ? [NSString stringWithUTF8String:instanceName] : NULL;
        NSString *nsBundleId = bundleId != NULL ? [NSString stringWithUTF8String:bundleId] : NULL;
        NSNumber *nsRevenue = [NSNumber numberWithDouble:revenue];
        NSString *nsCurrency = currency != NULL ? [NSString stringWithUTF8String:currency] : NULL;

		[[NativeBridge shared] forwardAdImpressionWithAdUnit: nsAdUnit
												   adSdkName: nsAdSdkName
												   adNetwork: nsAdNetwork
												   placement: nsPlacement
												   testGroup: nsTestGroup
												 segmentName: nsSegmentName
												instanceName: nsInstanceName
													bundleId: nsBundleId
													 revenue: nsRevenue
													currency: nsCurrency];
    }

    void _justtrack_sdk_rp_get_advertiser_id_info() {
        return [[NativeBridge shared] getAdvertiserIdInfo];
    }

	int _justtrack_sdk_rp_get_test_group_id() {
		return (int) [[NativeBridge shared] getTestGroupId];
	}

    void _justtrack_sdk_rp_request_tracking_authorization() {
        return [[NativeBridge shared] requestTrackingAuthorization];
    }

    void _justtrack_sdk_rp_forward_transaction_id(const char *transactionId, const char *productId, int quantity) {
        NSString *nsTransactionId = [NSString stringWithUTF8String:transactionId];
        NSString *nsProductId = [NSString stringWithUTF8String:productId];

        [[NativeBridge shared] forwardTransactionId:nsTransactionId productId:nsProductId quantity:(NSInteger)quantity];
    }

    int _justtrack_sdk_rp_get_tracking_authorization_status() {
        return (int) [[NativeBridge shared] getTrackingAuthorizationStatus];
    }

	void _justtrack_sdk_rp_set_experiment_variant(const char *experiment, const char *variant, const char *tags, const char *happenedAt) {
		NSString *nsExperiment = [NSString stringWithUTF8String:experiment];
		NSString *nsVariant = [NSString stringWithUTF8String:variant];
		NSString *nsTags = [NSString stringWithUTF8String:tags];
		NSString *nsHappenedAt = [NSString stringWithUTF8String:happenedAt];

		[[NativeBridge shared] setExperimentVariantWithExperiment:nsExperiment variant:nsVariant tags:nsTags happenedAt:nsHappenedAt];
	}

	void _justtrack_sdk_rp_fetch_remote_config() {
		[[NativeBridge shared] fetchRemoteConfig];
	}

	void _justtrack_sdk_rp_activate_remote_config(const char *experimentsJson) {
		NSArray<NSString *> *items = @[];
		if (experimentsJson != NULL) {
			NSString *nsExperimentsJson = [NSString stringWithUTF8String:experimentsJson];
			if (nsExperimentsJson.length > 0) {
				NSData *jsonData = [nsExperimentsJson dataUsingEncoding:NSUTF8StringEncoding];
				if (jsonData != nil) {
					NSError *error = nil;
					NSDictionary *jsonObject = [NSJSONSerialization JSONObjectWithData:jsonData options:0 error:&error];
					if (error == nil && [jsonObject isKindOfClass:[NSDictionary class]]) {
						NSArray *rawItems = jsonObject[@"items"];
						if ([rawItems isKindOfClass:[NSArray class]]) {
							items = rawItems;
						}
					}
				}
			}
		}

		[[NativeBridge shared] activateRemoteConfigWithExperiments:items];
	}

	void _justtrack_sdk_rp_fetch_and_activate_remote_config() {
		[[NativeBridge shared] fetchAndActivateRemoteConfig];
	}

	void _justtrack_sdk_rp_set_remote_config_settings(long long minimumFetchIntervalInSeconds) {
		[[NativeBridge shared] setRemoteConfigSettingsWithMinimumFetchIntervalInSeconds:minimumFetchIntervalInSeconds];
	}

	char *_justtrack_sdk_rp_get_all_assignments() {
		NSString *s = [[NativeBridge shared] getAllAssignments];

		return s == nil ? NULL : _justtrack_sdk_rp_alloc_string(s);
	}

	char *_justtrack_sdk_rp_get_remote_config_string(const char *configKey) {
		NSString *nsConfigKey = [NSString stringWithUTF8String:configKey];
		NSString *s = [[NativeBridge shared] getRemoteConfigStringWithConfigKey:nsConfigKey];

		return s == nil ? NULL : _justtrack_sdk_rp_alloc_string(s);
	}
}
