import AppTrackingTransparency
import Foundation
import JustTrackSDK
import os
#if JUSTTRACK_UNITY_APPLOVIN
import JustTrackSDKAppLovinAdapter
#endif
#if JUSTTRACK_UNITY_FIREBASE
import JustTrackSDKFirebaseAdapter
#endif
#if JUSTTRACK_UNITY_IRONSOURCE
import JustTrackSDKIronSourceAdapter
#endif
#if JUSTTRACK_UNITY_UNITYADS
import JustTrackSDKUnityAdsAdapter
#endif
#if JUSTTRACK_UNITY_GOOGLE_ODM
import JustTrackSDKGoogleOdmAdapter
#endif

/// Protocol for sending messages back to Unity.
public protocol UnityMessageSender {
	/// Sends a message to a Unity GameObject.
	/// - Parameters:
	///   - withName: The name of the GameObject.
	///   - functionName: The function to call on the GameObject.
	///   - message: The message to send.
	func sendMessageToGO(withName: UnsafePointer<CChar>!, functionName: UnsafePointer<CChar>!, message: UnsafePointer<CChar>!)
}

#if JUSTTRACK_UNITY
extension UnityFramework: UnityMessageSender {}
#else
struct EmptyUnityMessageSender: UnityMessageSender {
	func sendMessageToGO(withName: UnsafePointer<CChar>!, functionName: UnsafePointer<CChar>!, message: UnsafePointer<CChar>!) {
		// do nothing
	}
}
#endif

/// Bridge class for Unity integration with the justtrack SDK.
@objc public class NativeBridge: NSObject {
	@objc public static let shared = NativeBridge()
	private var sdk: JustTrackSdk?
	private let sender: UnityMessageSender
	private var onStart: (() -> Void)?

	private override init() {
		#if JUSTTRACK_UNITY
			sender = UnityFramework.getInstance()
		#else
			sender = EmptyUnityMessageSender()
		#endif
		super.init()
	}

	@objc public func initSdk(
		apiToken: String,
		trackingId: String,
		trackingProvider: String,
		customUserId: String,
		inactivityTimeFrameHours: Int,
		reAttributionTimeFrameDays: Int,
		reFetchReAttributionDelaySeconds: Int,
		attributionRetryDelaySeconds: Int,
		automaticInAppPurchaseTracking: Bool,
		manualStart: Bool,
		enableConsoleLogging: Bool,
		customBundleId: String,
		customAppVersion: String,
		customAppCode: String,
        customServerUrl: String?
	) {
		DispatchQueue.main.async {
			do {
				var builder = JustTrackSdkBuilder(apiToken: apiToken)
				builder = try builder.set(trackingId: trackingId, trackingProvider: trackingProvider)
				if !customUserId.isEmpty {
					builder = try builder.set(userId: customUserId)
				}
				builder = builder.set(platformType: .unity)
					.set(inactivityTimeFrameHours: inactivityTimeFrameHours)
					.set(reAttributionTimeFrameDays: reAttributionTimeFrameDays)
					.set(reFetchReAttributionDelaySeconds: reFetchReAttributionDelaySeconds)
					.set(attributionRetryDelaySeconds: attributionRetryDelaySeconds)
					.set(automaticInAppPurchaseTracking: automaticInAppPurchaseTracking)
					.set(manualStart: manualStart)
					.set(isLoggingEnabled: enableConsoleLogging)

				if !customBundleId.isEmpty {
					builder = builder.set(bundleId: customBundleId)
				}
				if !customAppVersion.isEmpty || !customAppCode.isEmpty {
					builder = builder.set(applicationVersion: customAppVersion, versionCode: customAppCode)
				}
                if let customServerUrl = customServerUrl, !customServerUrl.isEmpty {
                    builder = try builder.set(serverUrl: customServerUrl)
                }

				self.sdk = try builder.build()
			} catch {
				self.sendMessage(receiver: "OnAttributionError", message: "Failed to initialize SDK: \(error.justTrackGetErrorDescription())")
				return
			}
			guard let sdk = self.sdk else { return }
			_ = sdk.register(attributionListener: { response in
				self.sendMessage(receiver: "OnAttributionListenerReceived", message: Self.encodeAttributionResponse(response: response))
			})
			_ = sdk.register(retargetingParametersListener: { parameters in
				self.sendMessage(
					receiver: "OnRetargetingParametersListenerReceived",
					message: Self.encodeRetargetingParameters(
						parameters: parameters,
						preliminaryId: nil
					)
				)
			})
			_ = sdk.register(preliminaryRetargetingParametersListener: { parameters in
				let preliminaryId = self.observePreliminaryParameters(parameters: parameters)
				self.sendMessage(
					receiver: "OnPreliminaryRetargetingParametersListenerReceived",
					message: Self.encodeRetargetingParameters(
						parameters: parameters,
						preliminaryId: preliminaryId
					)
				)
			})

			let onStartBlock = {
				sdk.attribution.observe(using: { result in
					switch result {
					case .failure(let error):
						self.sendMessage(receiver: "OnAttributionError", message: "Failed to attribute user: \(error.justTrackGetErrorDescription())")
					case .success(let response):
						self.sendMessage(receiver: "OnAttributionDone", message: Self.encodeAttributionResponse(response: response))
					}
				})

				sdk.getInstallInstanceId().observe { result in
					switch result {
					case let .failure(error):
						self.sendMessage(receiver: "OnAttributionError", message: "Failed to get install instance id: \(error.justTrackGetErrorDescription())")
					case let .success(installInstanceId):
						self.sendMessage(
							receiver: "OnSdkInitialized",
							message: "{\"installInstanceId\": \"\(installInstanceId)\"}"
						)
					}
				}
			}

			if manualStart {
				self.onStart = onStartBlock
			} else {
				onStartBlock()
			}
		}
	}

	@objc public func start() {
		withSdk { [self] sdk in
			sdk.start()
			onStart?()
			onStart = nil
		}
	}

	@objc public func stop() {
		withSdk { sdk in
			sdk.stop()
		}
	}

	@objc public func anonymize() {
		withSdk { sdk in
			sdk.anonymize().observe { result in
				switch result {
				case let .failure(error):
					self.sendMessage(receiver: "OnAnonymizeError", message: "Failed to anonymize user: \(error.justTrackGetErrorDescription())")
				case .success:
					self.sendMessage(receiver: "OnAnonymizeDone")
				}
			}
		}
	}

	@objc public func isRunning() -> Bool {
		guard let sdk else {
			return false
		}

		return sdk.isRunning()
	}

	@objc public func getRetargetingParameters() {
		withSdk { [self] sdk in
			sdk.retargetingParameters.observe(using: { result in
				switch result {
				case .failure(let error):
					self.sendMessage(
						receiver: "OnGetRetargetingParametersError",
						message: "Failed to get retargeting parameters: \(error.justTrackGetErrorDescription())"
					)
				case .success(let parameters):
					guard let parameters = parameters else {
						self.sendMessage(receiver: "OnGetRetargetingParametersDone")
						return
					}
					self.sendMessage(
						receiver: "OnGetRetargetingParametersDone",
						message: Self.encodeRetargetingParameters(
							parameters: parameters,
							preliminaryId: nil
						)
					)
				}
			})
		}
	}

	@objc public func getPreliminaryRetargetingParameters() -> String {
		withSdkResult(fallback: "") { sdk in
			guard let parameters = sdk.preliminaryRetargetingParameters else {
				return ""
			}
			let preliminaryId = observePreliminaryParameters(parameters: parameters)
			return Self.encodeRetargetingParameters(parameters: parameters, preliminaryId: preliminaryId)
		}
	}

	@objc public func set(userId: String) {
		withSdk { sdk in
			_ = sdk.set(userId: userId)
		}
	}

	@objc public func set(automaticInAppPurchaseTracking: Bool) {
		withSdk { sdk in
			sdk.set(automaticInAppPurchaseTracking: automaticInAppPurchaseTracking)
		}
	}

	@objc public func set(firebaseAppInstanceId: String) {
		withSdk { sdk in
			_ = sdk.set(firebaseAppInstanceId: firebaseAppInstanceId)
		}
	}

	@objc public func publishEvent(
		name: String,
		dimensions: String,
		value: Double,
		unit: String?,
		currency: String?,
		requestId: String?
	) {
		withSdk { [self] sdk in
			if #available(iOS 10.0, *) {
				guard let dimensionData = dimensions.data(using: .utf8) else {
					sendError(error: "Failed to create data from json string")
					return
				}
				do {
					let decodedDimensions = try Self.decode(dimensions: dimensionData)

					let unitResult: JustTrackSDK.Unit?
					if let unit {
						guard let parsedUnit = JustTrackSDK.Unit(rawValue: unit) else {
							sendError(error: "Unknown unit type: \(unit)")
							return
						}
						unitResult = parsedUnit
					} else {
						unitResult = nil
					}

					sdk.publish(
						event: AppEvent(
							name: name,
							dimensions: decodedDimensions,
							value: value,
							unit: unitResult,
							currency: currency
						)
					).observe(on: .main) { [weak self] publishingResult in
						guard let requestId else { return }
						switch publishingResult {
						case let .failure(error):
							self?.sendMessage(receiver: "OnPublishEventError", message: "\(requestId)|\(error.localizedDescription)")
						case .success:
							self?.sendMessage(receiver: "OnPublishEventDone", message: requestId)
						}
					}
				} catch {
					sendError(error: "Failed to parse json string")
				}
			} else {
				sendError(error: "Not available on this platform")
			}
		}
	}

	@objc public func forwardAdImpression(
		adUnit: String?,
		adSdkName: String?,
		adNetwork: String?,
		placement: String?,
		testGroup: String?,
		segmentName: String?,
		instanceName: String?,
		bundleId: String?,
		revenue: NSNumber?,
		currency: NSString?
	) {
		guard let sdk = sdk, let adSdkName = adSdkName else {
			self.sendMessage(receiver: "OnForwardAdImpressionError", message: "Failed to get adSdkName")
			return
		}
		guard let adUnit = adUnit else {
			self.sendMessage(receiver: "OnForwardAdImpressionError", message: "Failed to get adUnit")
			return
		}

		var impression = AdImpression(unit: adUnit, sdkName: adSdkName)
			.set(network: adNetwork)
			.set(placement: placement)
			.set(testGroup: testGroup)
			.set(segmentName: segmentName)
			.set(instanceName: instanceName)
			.set(bundleId: bundleId)
		if let revenue {
			impression = impression.set(revenue: Money(value: revenue.doubleValue, currency: String(currency ?? "USD")))
		}

		sdk.forward(adImpression: impression).observe { forwardResult in
			switch forwardResult {
			case let .failure(error):
				self.sendMessage(receiver: "OnForwardAdImpressionError", message: "Failed to forward ad impression: \(error.justTrackGetErrorDescription())")
			case .success:
				self.sendMessage(receiver: "OnForwardAdImpressionDone")
			}
		}
	}

	@objc public func integrateWithAppLovin(customUserId: String?) {
#if JUSTTRACK_UNITY_APPLOVIN
		integrate(with: JusttrackAppLovinAdapter(customUserId: customUserId)).observe(on: .main) { [self] integrationResult in
			switch integrationResult {
			case let .failure(error):
				sendMessage(receiver: "OnIntegrateAppLovinError", message: error.localizedDescription)
			case .success:
				sendMessage(receiver: "OnIntegrateAppLovinDone")
			}
		}
#else
		sendMessage(receiver: "OnIntegrateAppLovinError", message: noAdapterMessage(for: "AppLovin"))
#endif
	}

	@objc public func integrateWithFirebase() {
#if JUSTTRACK_UNITY_FIREBASE
		integrate(with: JusttrackFirebaseAdapter()).observe(on: .main) { [self] integrationResult in
			switch integrationResult {
			case let .failure(error):
				sendMessage(receiver: "OnIntegrateFirebaseError", message: error.localizedDescription)
			case .success:
				sendMessage(receiver: "OnIntegrateFirebaseDone")
			}
		}
#else
		sendMessage(receiver: "OnIntegrateFirebaseError", message: noAdapterMessage(for: "Firebase"))
#endif
	}

	@objc public func integrateWithIronSource(customUserId: String?) {
#if JUSTTRACK_UNITY_IRONSOURCE
		integrate(with: JusttrackIronSourceAdapter(customUserId: customUserId)).observe(on: .main) { [self] integrationResult in
			switch integrationResult {
			case let .failure(error):
				sendMessage(receiver: "OnIntegrateIronSourceError", message: error.localizedDescription)
			case .success:
				sendMessage(receiver: "OnIntegrateIronSourceDone")
			}
		}
#else
		sendMessage(receiver: "OnIntegrateIronSourceError", message: noAdapterMessage(for: "IronSource"))
#endif
	}

	@objc public func integrateWithUnityAds() {
#if JUSTTRACK_UNITY_UNITYADS
		integrate(with: JusttrackUnityAdsAdapter()).observe(on: .main) { [self] integrationResult in
			switch integrationResult {
			case let .failure(error):
				sendMessage(receiver: "OnIntegrateUnityAdsError", message: error.localizedDescription)
			case .success:
				sendMessage(receiver: "OnIntegrateUnityAdsDone")
			}
		}
#else
		sendMessage(receiver: "OnIntegrateUnityAdsError", message: noAdapterMessage(for: "UnityAds"))
#endif
	}

	@objc public func integrateWithGoogleOdm() {
#if JUSTTRACK_UNITY_GOOGLE_ODM
		integrate(with: JusttrackGoogleOdmAdapter()).observe(on: .main) { [self] integrationResult in
			switch integrationResult {
			case let .failure(error):
				sendMessage(receiver: "OnIntegrateGoogleOdmError", message: error.localizedDescription)
			case .success:
				sendMessage(receiver: "OnIntegrateGoogleOdmDone")
			}
		}
#else
		sendMessage(receiver: "OnIntegrateGoogleOdmError", message: noAdapterMessage(for: "GoogleOdm"))
#endif
	}

	/// Gets the advertiser ID (IDFA) information.
	@objc public func getAdvertiserIdInfo() {
		withSdk { [self] sdk in
			sdk.getAdvertiserIdInfo().observe { result in
				switch result {
				case let .failure(error):
					self.sendMessage(receiver: "OnGetAdvertiserIdInfoError", message: "Failed to get advertiser id: \(error.justTrackGetErrorDescription())")

				case let .success(info):
					self.sendMessage(receiver: "OnGetAdvertiserIdInfo", message: info.advertiserId ?? "")
				}
			}
		}
	}

	@objc public func getTestGroupId() -> Int {
		return sdk?.testGroupId ?? -1
	}

	@objc public func requestTrackingAuthorization() {
		JustTrack.requestTrackingAuthorization { granted in
			self.sendMessage(receiver: "OnTrackingAuthorization", message: granted ? "authorized" : "denied")
		}
	}

	@objc public func forwardTransactionId(_ transactionId: String, productId: String, quantity: Int) {
		withSdk { sdk in
			if #available(iOS 15.0, *) {
				_ = sdk.forward(transactionId: transactionId, productId: productId, quantity: quantity)
			} else {
				os_log("JustTrackSdk: %s", type: .info, "NativeBridge: `forwardTransactionId(_:productId:quantity:)` is not supported")
			}
		}
	}

	@objc public func getTrackingAuthorizationStatus() -> Int {
		if #available(iOS 14, *) {
			switch ATTrackingManager.trackingAuthorizationStatus {
			case .notDetermined:
				return 0
			case .restricted:
				return 1
			case .denied:
				return 2
			case .authorized:
				return 3
			@unknown default:
				return 4
			}
		}

		return 4
	}

	/// Sets an experiment variant for A/B testing.
	/// - Parameters:
	///   - experiment: The experiment name.
	///   - variant: The variant name.
	///   - tags: JSON string with tags array.
	///   - happenedAt: ISO 8601 date string for when the variant was set.
	@objc public func setExperimentVariant(experiment: String, variant: String, tags: String, happenedAt: String) {
		withSdk { [self] sdk in
			var tagsArray: [String] = []
			if !tags.isEmpty {
				if let tagsData = tags.data(using: .utf8) {
					do {
						if let jsonObject = try JSONSerialization.jsonObject(with: tagsData, options: []) as? [String: Any],
							let items = jsonObject["items"] as? [String]
						{
							tagsArray = items
						}
					} catch {
						self.sendMessage(receiver: "OnSetExperimentVariantError", message: "Failed to parse tags JSON: \(error.localizedDescription)")
						return
					}
				}
			}

			var happenedAtDate: Date?
			if !happenedAt.isEmpty {
				let formatter = ISO8601DateFormatter()
				happenedAtDate = formatter.date(from: happenedAt)
				if happenedAtDate == nil {
					self.sendMessage(receiver: "OnSetExperimentVariantError", message: "Failed to parse happenedAt date string: \(happenedAt)")
					return
				}
			}

			sdk.setExperimentVariant(
				experiment: experiment,
				variant: variant,
				tags: tagsArray,
				happenedAt: happenedAtDate
			).observe { result in
				switch result {
				case let .failure(error):
					self.sendMessage(receiver: "OnSetExperimentVariantError", message: "Failed to set experiment variant: \(error.justTrackGetErrorDescription())")
				case .success:
					self.sendMessage(receiver: "OnSetExperimentVariantDone")
				}
			}
		}
	}

	/// Fetches remote config values from the server.
	@objc public func fetchRemoteConfig() {
		withSdk { [self] sdk in
			sdk.remoteConfig.fetch() { error in
                if let error {
                    self.sendMessage(receiver: "OnFetchRemoteConfigError", message: "Failed to fetch remote config: \(error.justTrackGetErrorDescription())")
                } else {
                    self.sendMessage(receiver: "OnFetchRemoteConfigDone")
                }
			}
		}
	}

	/// Activates experiment assignments by confirming enrollment with the server.
	/// - Parameter experiments: Array of experiment IDs to activate.
	@objc public func activateRemoteConfig(experiments: [String]) {
		withSdk { [self] sdk in
			sdk.remoteConfig.activate(experimentIds: experiments) { error in
				if let error = error {
					self.sendMessage(receiver: "OnActivateRemoteConfigError", message: "Failed to activate remote config: \(error.justTrackGetErrorDescription())")
					return
				}
				self.sendMessage(receiver: "OnActivateRemoteConfigDone")
			}
		}
	}

	/// Fetches and then activates all pending experiments.
	@objc public func fetchAndActivateRemoteConfig() {
		withSdk { [self] sdk in
			sdk.remoteConfig.fetchAndActivate { error in
				if let error = error {
					self.sendMessage(receiver: "OnFetchAndActivateRemoteConfigError", message: "Failed to fetch and activate remote config: \(error.justTrackGetErrorDescription())")
					return
				}
				self.sendMessage(receiver: "OnFetchAndActivateRemoteConfigDone")
			}
		}
	}

	/// Sets remote config settings.
	/// - Parameter minimumFetchIntervalInSeconds: Minimum fetch interval in seconds.
	@objc public func setRemoteConfigSettings(minimumFetchIntervalInSeconds: Int64) {
		withSdk { sdk in
			let settings = JusttrackRemoteConfigSettings(minFetchIntervalInSec: TimeInterval(minimumFetchIntervalInSeconds))
			sdk.remoteConfig.setConfig(settings)
		}
	}

	/// Gets all current experiment assignments.
	@objc public func getAllAssignments() -> String? {
		withSdkResult(fallback: nil) { sdk in
			let assignments = sdk.remoteConfig.allAssignments
			struct AssignmentDto: Encodable {
				let ConfigKey: String
				let ConfigValue: String
				let ExperimentId: String
				let IsPending: Bool
			}
			var items: [AssignmentDto] = []
			items.reserveCapacity(assignments.count)
			for assignment in assignments {
				items.append(AssignmentDto(
					ConfigKey: assignment.configKey,
					ConfigValue: assignment.configValue,
					ExperimentId: assignment.experimentId,
					IsPending: assignment.isPending
				))
			}
			let dto: [String: [AssignmentDto]] = ["items": items]
			return Self.encodeDto(dto: dto)
		}
	}

	@objc public func getRemoteConfigString(configKey: String) -> String? {
		withSdkResult(fallback: nil) { sdk in
			return sdk.remoteConfig.getString(configKey: configKey)
		}
	}

	// MARK: - Private Helpers

	private static func encodeAttributionResponse(response: AttributionResponse) -> String {
		let dto: [String: String?] = [
			"userType": response.userType,
			"redownload": String(response.isRedownload),
			"type": response.type,
			"campaignId": String(response.campaign.id),
			"campaignName": response.campaign.name,
			"campaignType": response.campaign.type,
			"channelId": String(response.channel.id),
			"channelName": response.channel.name,
			"channelIncent": String(response.channel.isIncent),
			"partnerId": String(response.partner.id),
			"partnerName": response.partner.name,
			"sourceId": response.sourceId,
			"sourceBundleId": response.sourceBundleId,
			"sourcePlacement": response.sourcePlacement,
			"adsetId": response.adsetId,
			"createdAt": formatDateSeconds(response.createdAt),
		]

		return Self.encodeDto(dto: dto)
	}

	private static func encodeDto<T>(dto: T) -> String where T: Encodable {
		let dtoJson = try! JSONEncoder().encode(dto)  // swiftlint:disable:this force_try
		return String.init(decoding: dtoJson, as: UTF8.self)
	}

	private static func encodeRetargetingParameters(parameters: RetargetingParameters, preliminaryId: String?) -> String {
		var parametersList: [[String: String]] = []
		for parameter in parameters.parameters {
			parametersList.append(["parameter": parameter.key, "value": parameter.value])
		}
		let parametersJson = try! JSONEncoder().encode(["parameters": parametersList])  // swiftlint:disable:this force_try
		var dto = [
			"wasAlreadyInstalled": parameters.wasAlreadyInstalled ? "true" : "false",
			"url": parameters.url?.absoluteString,
			"parameters": String.init(decoding: parametersJson, as: UTF8.self),
			"promoCode": parameters.promotionParameter,
		]
		if let preliminaryId {
			dto["preliminaryId"] = preliminaryId
		}
		return Self.encodeDto(dto: dto)
	}

	private func observePreliminaryParameters(parameters: PreliminaryRetargetingParameters) -> String {
		let preliminaryId = UUID().uuidString.lowercased()  // swiftlint:disable:this no_uuid
		parameters.validate().observe(using: { result in
			switch result {
			case .failure(let error):
				self.sendMessage(
					receiver: "OnValidatePreliminaryRetargetingParametersError",
					message: Self.encodeDto(dto: [
						"preliminaryId": preliminaryId,
						"error": error.justTrackGetErrorDescription(),
					])
				)
			case .success(let parameters):
				var dto = [
					"preliminaryId": preliminaryId,
					"response": Self.encodeAttributionResponse(response: parameters.attributionResponse),
				]
				if let parameters = parameters.validParameters {
					dto["parameters"] = Self.encodeRetargetingParameters(parameters: parameters, preliminaryId: nil)
				}
				self.sendMessage(receiver: "OnValidatePreliminaryRetargetingParametersDone", message: Self.encodeDto(dto: dto))
			}
		})
		return preliminaryId
	}

	private static func decode(dimensions: Data) throws -> [String: String] {
		guard let json = try JSONSerialization.jsonObject(with: dimensions, options: []) as? [String: Any] else {
			throw DecodingError.dataCorrupted(.init(codingPath: [], debugDescription: "Failed to decode JSON as object"))
		}
		var result: [String: String] = [:]
		for (k, v) in json {
			guard let v = v as? String? else {
				throw DecodingError.typeMismatch(
					String.self,
					.init(
						codingPath: [],
						debugDescription: "Field '\(k)' should be a string, got \(type(of: v))"
					)
				)
			}

			if let v, v != "" {
				result[k] = v
			}
		}

		return result
	}

	private func integrate(with adapter: JusttrackAdapter) -> Future<Void> {
		let promise = FutureImpl<Void>()

		withSdk { sdk in
			sdk.integrate(with: adapter).observe { integrationResult in
				switch integrationResult {
				case let .failure(error):
					promise.reject(error)
				case .success:
					promise.resolve(())
				}
			}
		}

		return promise.toFuture()
	}

	private func sendMessage(receiver: String, message: String = "") {
		sender.sendMessageToGO(withName: "JustTrackSDKNativeBridgeUnity", functionName: receiver, message: message)
	}

	private func withSdk(_ callback: @escaping (JustTrackSdk) -> Void) {
		DispatchQueue.main.async {
			self.withSdkResult(fallback: Void(), callback)
		}
	}

	private func withSdkResult<T>(fallback: T, _ callback: (JustTrackSdk) -> T) -> T {
		guard let sdk = sdk else {
			sendError(error: "Not initialized")
			return fallback
		}

		return callback(sdk)
	}

	private func sendError(error: String) {
		sendMessage(receiver: "OnHandleError", message: error)
	}

	private func noAdapterMessage(for adapter: String) -> String {
		"justtrackSDK \(adapter) for iOS is not imported. Please select the 'Add \(adapter) Integration Adapter' option on the justtrackSDK settings panel in your Unity Editor and rebuild the project."
	}
}

private let enUs = Locale.init(identifier: "en_US")
private let utc = TimeZone(abbreviation: "UTC")
private let dateFormatSeconds = "yyyy-MM-dd'T'HH:mm:ss'Z'"
private let dateFormatMilliseconds = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'"

private func formatDateSeconds(_ date: Date) -> String {
	let formatter = DateFormatter()
	formatter.dateFormat = dateFormatSeconds
	formatter.timeZone = utc
	formatter.locale = enUs

	return formatter.string(from: date)
}
