/**
 * justtrack WebGL Bridge
 *
 * This JavaScript library provides the bridge between Unity WebGL and the justtrack Web SDK.
 */

var JustTrackWebGLPlugin = {
    $JustTrackWebGL: {
        sdk: null,
        isInitialized: false,

        ptrToString: function(ptr) {
            return UTF8ToString(ptr);
        },

        sendUnityCallback: function(gameObjectName, methodName, data) {
            try {
                if (typeof SendMessage !== 'undefined') {
                    SendMessage(gameObjectName, methodName, data);
                } else {
                    console.error('[justtrack WebGL] SendMessage is not available');
                }
            } catch (e) {
                console.error('[justtrack WebGL] Error sending Unity callback:', e);
            }
        },

        createSuccessResponse: function(data) {
            return JSON.stringify({
                success: true,
                data: data,
                error: null
            });
        },

        createErrorResponse: function(error) {
            return JSON.stringify({
                success: false,
                data: null,
                error: error
            });
        },

        createEventCallbackEnvelope: function(callbackId, success, data, error) {
            return JSON.stringify({
                callbackId: callbackId,
                response: {
                    success: success,
                    data: data,
                    error: error
                }
            });
        },

        formatAttributionForUnity: function(attribution) {
            if (!attribution) {
                return null;
            }

            return JSON.stringify({
                userType: attribution.userType || '',
                type: attribution.type || '',
                campaignId: String(attribution.campaignId || 0),
                campaignName: attribution.campaignName || '',
                campaignType: attribution.campaignType || '',
                channelId: String(attribution.channelId || 0),
                channelName: attribution.channelName || '',
                channelIncent: attribution.channelIncent ? 'true' : 'false',
                partnerId: String(attribution.partnerId || 0),
                partnerName: attribution.partnerName || '',
                sourceId: attribution.sourceId || null,
                sourceBundleId: attribution.sourceBundleId || null,
                sourcePlacement: attribution.sourcePlacement || null,
                adsetId: attribution.adsetId || null,
                createdAt: attribution.createdAt ? new Date(attribution.createdAt).toISOString() : new Date().toISOString()
            });
        }
    },

    _justtrack_webgl_init: function(apiKeyPtr, bundleIdPtr, appVersionPtr, appCodePtr, userIdPtr, manualStart, isLoggingEnabled, gameObjectNamePtr, callbackMethodNamePtr) {
        var apiKey = JustTrackWebGL.ptrToString(apiKeyPtr);
        var bundleId = JustTrackWebGL.ptrToString(bundleIdPtr);
        var appVersion = JustTrackWebGL.ptrToString(appVersionPtr);
        var appCode = JustTrackWebGL.ptrToString(appCodePtr);
        var userId = JustTrackWebGL.ptrToString(userIdPtr);
        var gameObjectName = JustTrackWebGL.ptrToString(gameObjectNamePtr);
        var callbackMethodName = JustTrackWebGL.ptrToString(callbackMethodNamePtr);

        try {
            if (typeof window.justtrack === 'undefined') {
                throw new Error('justtrack Web SDK is not loaded. Please ensure the SDK script is included in your HTML template.');
            }

            var config = {
                apiToken: apiKey,
                bundleId: bundleId,
                appVersion: appVersion,
                appCode: appCode,
                manualStart: manualStart,
                isLoggingEnabled: isLoggingEnabled,
                wrapper: 'unity'
            };

            if (userId && userId.length > 0) {
                config.userId = userId;
            }

            window.justtrack.init(config);
            JustTrackWebGL.sdk = window.justtrack;
            JustTrackWebGL.isInitialized = true;

            var response = JustTrackWebGL.createSuccessResponse(null);
            JustTrackWebGL.sendUnityCallback(gameObjectName, callbackMethodName, response);

        } catch (e) {
            console.error('[justtrack WebGL] Initialization error:', e);
            var response = JustTrackWebGL.createErrorResponse(e.message || 'Initialization failed');
            JustTrackWebGL.sendUnityCallback(gameObjectName, callbackMethodName, response);
        }
    },

    _justtrack_webgl_start: function() {
        try {
            if (!JustTrackWebGL.isInitialized || !JustTrackWebGL.sdk) {
                throw new Error('SDK not initialized');
            }
            JustTrackWebGL.sdk.start();
        } catch (e) {
            console.error('[justtrack WebGL] Start error:', e);
        }
    },

    _justtrack_webgl_stop: function() {
        try {
            if (!JustTrackWebGL.isInitialized || !JustTrackWebGL.sdk) {
                throw new Error('SDK not initialized');
            }
            JustTrackWebGL.sdk.stop();
        } catch (e) {
            console.error('[justtrack WebGL] Stop error:', e);
        }
    },

    _justtrack_webgl_is_running: function() {
        try {
            if (!JustTrackWebGL.isInitialized || !JustTrackWebGL.sdk) {
                return false;
            }
            return JustTrackWebGL.sdk.isRunning() ? 1 : 0;
        } catch (e) {
            console.error('[justtrack WebGL] IsRunning error:', e);
            return 0;
        }
    },

    _justtrack_webgl_anonymize: function(gameObjectNamePtr, callbackMethodNamePtr) {
        var gameObjectName = JustTrackWebGL.ptrToString(gameObjectNamePtr);
        var callbackMethodName = JustTrackWebGL.ptrToString(callbackMethodNamePtr);

        try {
            if (!JustTrackWebGL.isInitialized || !JustTrackWebGL.sdk) {
                throw new Error('SDK not initialized');
            }

            JustTrackWebGL.sdk.anonymize()
                .then(function() {
                    var response = JustTrackWebGL.createSuccessResponse(null);
                    JustTrackWebGL.sendUnityCallback(gameObjectName, callbackMethodName, response);
                })
                .catch(function(error) {
                    console.error('[justtrack WebGL] Anonymize error:', error);
                    var response = JustTrackWebGL.createErrorResponse(error.message || 'Anonymize failed');
                    JustTrackWebGL.sendUnityCallback(gameObjectName, callbackMethodName, response);
                });

        } catch (e) {
            console.error('[justtrack WebGL] Anonymize error:', e);
            var response = JustTrackWebGL.createErrorResponse(e.message || 'Anonymize failed');
            JustTrackWebGL.sendUnityCallback(gameObjectName, callbackMethodName, response);
        }
    },

    _justtrack_webgl_track_event: function(eventNamePtr, dimensionsJsonPtr, valueJsonPtr, gameObjectNamePtr, callbackMethodNamePtr, callbackIdPtr) {
        var eventName = JustTrackWebGL.ptrToString(eventNamePtr);
        var dimensionsJson = JustTrackWebGL.ptrToString(dimensionsJsonPtr);
        var valueJson = JustTrackWebGL.ptrToString(valueJsonPtr);
        var gameObjectName = JustTrackWebGL.ptrToString(gameObjectNamePtr);
        var callbackMethodName = JustTrackWebGL.ptrToString(callbackMethodNamePtr);
        var callbackId = JustTrackWebGL.ptrToString(callbackIdPtr);

        try {
            if (!JustTrackWebGL.isInitialized || !JustTrackWebGL.sdk) {
                throw new Error('SDK not initialized');
            }

            var dimensions = null;
            if (dimensionsJson && dimensionsJson !== '{}') {
                try {
                    var dimensionsWrapper = JSON.parse(dimensionsJson);
                    dimensions = dimensionsWrapper.dimensions || null;
                } catch (e) {
                    console.warn('[justtrack WebGL] Failed to parse dimensions:', e);
                }
            }

            var value = null;
            if (valueJson && valueJson !== 'null') {
                try {
                    var valueObj = JSON.parse(valueJson);

                    if (valueObj.unit) {
                        value = new window.justtrack.Value(valueObj.value, valueObj.unit);
                    } else if (valueObj.currency) {
                        value = new window.justtrack.Money(valueObj.value, valueObj.currency);
                    }
                } catch (e) {
                    console.warn('[justtrack WebGL] Failed to parse value:', e);
                }
            }

            var trackResult = JustTrackWebGL.sdk.track(eventName, dimensions, value);

            if (callbackMethodName && callbackMethodName.length > 0 && callbackId && callbackId.length > 0) {
                trackResult.promise
                    .then(function() {
                        var callbackData = JustTrackWebGL.createEventCallbackEnvelope(callbackId, true, null, null);
                        JustTrackWebGL.sendUnityCallback(gameObjectName, callbackMethodName, callbackData);
                    })
                    .catch(function(error) {
                        console.error('[justtrack WebGL] Track event error:', error);
                        var callbackData = JustTrackWebGL.createEventCallbackEnvelope(
                            callbackId,
                            false,
                            null,
                            error.message || 'Track event failed'
                        );
                        JustTrackWebGL.sendUnityCallback(gameObjectName, callbackMethodName, callbackData);
                    });
            }

        } catch (e) {
            console.error('[justtrack WebGL] Track event error:', e);
            if (callbackMethodName && callbackMethodName.length > 0 && callbackId && callbackId.length > 0) {
                var callbackData = JustTrackWebGL.createEventCallbackEnvelope(
                    callbackId,
                    false,
                    null,
                    e.message || 'Track event failed'
                );
                JustTrackWebGL.sendUnityCallback(gameObjectName, callbackMethodName, callbackData);
            }
        }
    },

    _justtrack_webgl_get_attribution: function(gameObjectNamePtr, callbackMethodNamePtr) {
        var gameObjectName = JustTrackWebGL.ptrToString(gameObjectNamePtr);
        var callbackMethodName = JustTrackWebGL.ptrToString(callbackMethodNamePtr);

        try {
            if (!JustTrackWebGL.isInitialized || !JustTrackWebGL.sdk) {
                throw new Error('SDK not initialized');
            }

            JustTrackWebGL.sdk.getAttribution()
                .then(function(attribution) {
                    var attributionJson = JustTrackWebGL.formatAttributionForUnity(attribution);
                    var response = JustTrackWebGL.createSuccessResponse(attributionJson);
                    JustTrackWebGL.sendUnityCallback(gameObjectName, callbackMethodName, response);
                })
                .catch(function(error) {
                    console.error('[justtrack WebGL] GetAttribution error:', error);
                    var response = JustTrackWebGL.createErrorResponse(error.message || 'Failed to get attribution');
                    JustTrackWebGL.sendUnityCallback(gameObjectName, callbackMethodName, response);
                });

        } catch (e) {
            console.error('[justtrack WebGL] GetAttribution error:', e);
            var response = JustTrackWebGL.createErrorResponse(e.message || 'GetAttribution failed');
            JustTrackWebGL.sendUnityCallback(gameObjectName, callbackMethodName, response);
        }
    },

};
autoAddDeps(JustTrackWebGLPlugin, '$JustTrackWebGL');
mergeInto(LibraryManager.library, JustTrackWebGLPlugin);
