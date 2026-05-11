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
                false, // pEnableDebugMode
                false, // pManualStart
                false, // pEnableConsoleLogging
                (response) =>
                {

                }, (error) =>
                {

                });
        
        Assert.IsTrue(JustTrackSDK.IsRunning());
    }
}
