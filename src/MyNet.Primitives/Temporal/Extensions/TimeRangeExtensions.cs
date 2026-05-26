// -----------------------------------------------------------------------
// <copyright file="TimeRangeExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Primitives.Intervals;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Primitives;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TimeRangeExtensions
{
    extension(TimeRange left)
    {
        /// <summary>
        /// Shifts the time range by a specified offset, effectively moving the entire range forward or backward in time by the given duration. The resulting time range will have its start and end times adjusted by adding the offset to both boundaries, allowing for easy manipulation of time intervals without changing their duration.
        /// </summary>
        /// <param name="offset">The time span to shift the time range by.</param>
        /// <returns>A new time range shifted by the specified offset.</returns>
        public TimeRange Shift(TimeSpan offset) => new(left.Start!.Value.Value.Add(offset), left.End!.Value.Value.Add(offset));

        /// <summary>
        /// Expands the time range by a specified duration, effectively increasing the length of the range by adding the duration to both the start and end times. The resulting time range will have its start time moved earlier by subtracting the duration from it, and its end time moved later by adding the duration to it, allowing for easy manipulation of time intervals when you want to extend their coverage without changing their central point in time.
        /// </summary>
        /// <param name="duration">The duration to expand the time range by.</param>
        /// <returns>A new time range expanded by the specified duration.</returns>
        public TimeRange Expand(TimeSpan duration) => new(left.Start!.Value.Value.Add(-duration), left.End!.Value.Value.Add(duration));

        /// <summary>
        /// Enumerates the time values within the time range at specified intervals defined by the step parameter. This method generates a sequence of TimeOnly values starting from the beginning of the time range and incrementing by the specified step until it reaches the end of the range. The enumeration will include all time values that fall within the range, allowing for easy iteration over specific time points at regular intervals.
        /// </summary>
        /// <param name="step">The time span to use as the interval between each enumerated time value.</param>
        /// <returns>An enumerable sequence of TimeOnly values within the time range.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the step is less than or equal to zero.</exception>
        public IEnumerable<TimeOnly> Enumerate(TimeSpan step)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(step, TimeSpan.Zero);

            var current = left.Start!.Value.Value;
            var elapsed = TimeSpan.Zero;

            yield return current;

            while (elapsed + step <= left.Duration)
            {
                current = current.Add(step);
                elapsed += step;
                yield return current;
            }
        }
    }
}
