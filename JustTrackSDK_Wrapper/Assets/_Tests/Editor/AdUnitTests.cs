using NUnit.Framework;

namespace JustTrack.Tests.Editor
{
    public class AdUnitTests
    {
        [Test] public void ToInternalString_Banner()              => Assert.AreEqual("banner",               AdUnitInternalConversation.ToInternalString(AdUnit.Banner));
        [Test] public void ToInternalString_Interstitial()        => Assert.AreEqual("interstitial",         AdUnitInternalConversation.ToInternalString(AdUnit.Interstitial));
        [Test] public void ToInternalString_Rewarded()            => Assert.AreEqual("rewarded",             AdUnitInternalConversation.ToInternalString(AdUnit.Rewarded));
        [Test] public void ToInternalString_RewardedInterstitial() => Assert.AreEqual("rewarded_interstitial", AdUnitInternalConversation.ToInternalString(AdUnit.RewardedInterstitial));
        [Test] public void ToInternalString_Native()              => Assert.AreEqual("native",               AdUnitInternalConversation.ToInternalString(AdUnit.Native));
        [Test] public void ToInternalString_AppOpen()             => Assert.AreEqual("app_open",             AdUnitInternalConversation.ToInternalString(AdUnit.AppOpen));
        [Test] public void ToInternalString_Unknown_ReturnsEmpty() => Assert.AreEqual(string.Empty,          AdUnitInternalConversation.ToInternalString((AdUnit)99));
    }
}
