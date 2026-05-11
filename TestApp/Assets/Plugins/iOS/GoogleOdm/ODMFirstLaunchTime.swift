import Foundation
import GoogleAdsOnDeviceConversion

@objc public class ODMFirstLaunchTime: NSObject {
    @objc public static func setup() {
        let firstLaunchKey = "io.justtrack.firstLaunchTime"
        let defaults = UserDefaults.standard

        if defaults.object(forKey: firstLaunchKey) == nil {
            let now = Date()
            defaults.set(now, forKey: firstLaunchKey)
            ConversionManager.sharedInstance.setFirstLaunchTime(now)
            print("[JustTrack] Set ODM first launch time: \(now)")
        } else if let storedDate = defaults.object(forKey: firstLaunchKey) as? Date {
            ConversionManager.sharedInstance.setFirstLaunchTime(storedDate)
            print("[JustTrack] Restored ODM first launch time: \(storedDate)")
        }
    }
}
