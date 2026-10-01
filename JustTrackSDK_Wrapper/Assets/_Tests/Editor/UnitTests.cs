using System;
using NUnit.Framework;

namespace JustTrack.Tests.Editor
{
    public class UnitTests
    {
        // Unit enum — defined values exist
        [Test] public void Unit_Count_IsDefined()       => Assert.IsTrue(Enum.IsDefined(typeof(Unit), Unit.Count));
        [Test] public void Unit_Milliseconds_IsDefined() => Assert.IsTrue(Enum.IsDefined(typeof(Unit), Unit.Milliseconds));
        [Test] public void Unit_Seconds_IsDefined()      => Assert.IsTrue(Enum.IsDefined(typeof(Unit), Unit.Seconds));

        // TimeUnitGroup → Unit conversions
        [Test] public void ToUnit_Milliseconds_ReturnsMilliseconds() =>
            Assert.AreEqual(Unit.Milliseconds, TimeUnitGroupConversions.ToUnit(TimeUnitGroup.Milliseconds));

        [Test] public void ToUnit_Seconds_ReturnsSeconds() =>
            Assert.AreEqual(Unit.Seconds, TimeUnitGroupConversions.ToUnit(TimeUnitGroup.Seconds));

        [Test] public void ToUnit_InvalidVariant_Throws() =>
            Assert.Throws<Exception>(() => TimeUnitGroupConversions.ToUnit((TimeUnitGroup)99));
    }
}
