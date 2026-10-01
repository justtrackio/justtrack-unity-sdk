using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using JustTrack;

/*
 * This is a sample test, to set some starting point. 
 */
public class PublishEventTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void PublishEventTestsSimplePasses()
    {
        JustTrackSDK.Init(
                "test-token",
                "",
                "",
                null,
                false, // pAutomaticInAppPurchaseTracking
                false, // pManualStart
                false, // pEnableConsoleLogging
                false, // pEnableConnectionTracking
                null,  // pServerUrl
                null,  // pBundleId
                null); // pApplicationVersion
        
        Assert.IsTrue(JustTrackSDK.IsRunning());
    }
}
