using System;

namespace JustTrack
{
    /// <summary>
    /// Defines units of measurement for events.
    /// </summary>
    public enum Unit
    {
        /// <summary>
        /// We want to count how many times something happened in total.
        /// </summary>
        Count,

        /// <summary>
        /// We want to measure how long something takes with millisecond precision.
        /// </summary>
        Milliseconds,

        /// <summary>
        /// We want to measure how long something takes with second precision.
        /// </summary>
        Seconds,
    }

    /// <summary>
    /// A time-based unit grouping the milliseconds,seconds units.
    /// </summary>
    public enum TimeUnitGroup
    {
        /// <summary>
        /// See Unit.Milliseconds
        /// </summary>
        Milliseconds,

        /// <summary>
        /// See Unit.Seconds
        /// </summary>
        Seconds,
    }

    /// <summary>
    /// Provides conversion utilities for time unit groups in the justtrack SDK.
    /// </summary>
    internal static class TimeUnitGroupConversions
    {
        /// <summary>
        /// Converts a TimeUnitGroup to its corresponding Unit.
        /// </summary>
        /// <param name="unit">The time unit group to convert.</param>
        /// <returns>The corresponding Unit value.</returns>
        internal static Unit ToUnit(TimeUnitGroup unit)
        {
            switch (unit)
            {
                case TimeUnitGroup.Milliseconds: return Unit.Milliseconds;
                case TimeUnitGroup.Seconds: return Unit.Seconds;
                default: throw new Exception($"Unexpected enum variant: {unit}");
            }
        }
    }
}
