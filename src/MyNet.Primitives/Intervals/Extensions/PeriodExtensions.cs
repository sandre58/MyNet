// -----------------------------------------------------------------------
// <copyright file="PeriodExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Primitives.Intervals;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Primitives;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class PeriodExtensions
{
    extension(Period left)
    {
        /// <summary>
        /// Determines whether the current period is current, meaning that the current date and time falls within the period. This method checks if the current date and time is greater than or equal to the start of the period and less than the end of the period, taking into account whether the boundaries are inclusive or exclusive. If the current date and time satisfies these conditions, it returns true; otherwise, it returns false.
        /// </summary>
        /// <returns>True if the current date and time falls within the period; otherwise, false.</returns>
        public bool IsCurrent() => left.Contains(DateTime.Now);

        /// <summary>
        /// Determines whether the current period is in the past, meaning that the end of the period is before the current date and time. This method checks if the end of the period is less than or equal to the current date and time. If it is, the period is considered to be in the past.
        /// </summary>
        /// <returns>True if the period is in the past; otherwise, false.</returns>
        public bool IsPast() => left.IsPast(DateTime.Now);

        /// <summary>
        /// Determines whether the current period is in the future, meaning that the start of the period is after the current date and time. This method checks if the start of the period is greater than the current date and time. If it is, the period is considered to be in the future.
        /// </summary>
        /// <returns>True if the period is in the future; otherwise, false.</returns>
        public bool IsFuture() => left.IsFuture(DateTime.Now);

        /// <summary>
        /// Shifts the current period by a specified offset. This method creates a new period with the start and end times adjusted by the given offset.
        /// </summary>
        /// <param name="offset">The time span to shift the period by.</param>
        /// <returns>A new period shifted by the specified offset.</returns>
        public Period Shift(TimeSpan offset) => new(left.Start!.Value.Value + offset, left.End!.Value.Value + offset);

        /// <summary>
        /// Extends the current period by a specified duration. This method creates a new period with the end time extended by the given duration.
        /// </summary>
        /// <param name="duration">The duration to extend the period by.</param>
        /// <returns>A new period extended by the specified duration.</returns>
        public Period Extend(TimeSpan duration) => new(left.Start!.Value.Value, left.End!.Value.Value + duration);

        /// <summary>
        /// Enumerates the hours within the current period. This method generates a sequence of DateTime values representing each hour within the period.
        /// </summary>
        /// <returns>An enumerable of DateTime values for each hour within the period.</returns>
        public IEnumerable<DateTime> EnumerateHours()
        {
            for (var current = left.Start!.Value.Value;
                 current < left.End!.Value.Value;
                 current = current.AddHours(1))
            {
                yield return current;
            }
        }

        /// <summary>
        /// Enumerates the days within the current period. This method generates a sequence of DateTime values representing each day within the period.
        /// </summary>
        /// <returns>An enumerable of DateTime values for each day within the period.</returns>
        public IEnumerable<DateTime> EnumerateDays()
        {
            for (var current = left.Start!.Value.Value.Date;
                 current < left.End!.Value.Value;
                 current = current.AddDays(1))
            {
                yield return current;
            }
        }

        /// <summary>
        /// Calculates the gap between the current period and another period. This method returns the time span between the end of the current period and the start of the other period, or vice versa, if the periods do not overlap.
        /// </summary>
        /// <param name="right">The other period to calculate the gap with.</param>
        /// <returns>The time span representing the gap between the two periods, or TimeSpan.Zero if they overlap.</returns>
        public TimeSpan Gap(Period right) =>
            left.Overlaps(right)
                ? TimeSpan.Zero
                : left.End!.Value.Value < right.Start!.Value.Value
                    ? right.Start.Value.Value - left.End.Value.Value
                    : left.Start!.Value.Value - right.End!.Value.Value;
    }
}
