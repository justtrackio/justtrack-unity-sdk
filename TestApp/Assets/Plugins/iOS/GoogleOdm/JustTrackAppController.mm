#import "UnityAppController.h"
#import <UnityFramework/UnityFramework-Swift.h>

@interface JustTrackAppController : UnityAppController
@end

IMPL_APP_CONTROLLER_SUBCLASS(JustTrackAppController)

@implementation JustTrackAppController

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary<UIApplicationLaunchOptionsKey, id> *)launchOptions {
    [ODMFirstLaunchTime setup];
    return [super application:application didFinishLaunchingWithOptions:launchOptions];
}

@end
