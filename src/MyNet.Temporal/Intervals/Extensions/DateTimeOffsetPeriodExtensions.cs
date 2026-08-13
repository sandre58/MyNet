// -----------------------------------------------------------------------
// <copyright file="DateTimeOffsetPeriodExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Primitives;
using MyNet.Temporal.Intervals;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Temporal;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Fluent helpers for <see cref="DateTimeOffsetPeriod"/>. Status helpers require an explicit <c>now</c>;
/// they do not call <see cref="DateTimeOffset.Now"/>.
/// </summary>
public static class DateTimeOffsetPeriodExtensions
{
    extension(DateTimeOffsetPeriod left)
    {
        /// <summary>
        /// Returns whether <paramref name="now"/> lies inside the period.
        /// </summary>
        public bool IsCurrent(DateTimeOffset now) => left.Contains(now);

        /// <summary>
        /// Returns whether the period is entirely before <paramref name="now"/>.
        /// </summary>
        public bool IsPast(DateTimeOffset now) => left.IsPast(now);

        /// <summary>
        /// Returns whether the period is entirely after <paramref name="now"/>.
        /// </summary>
        public bool IsFuture(DateTimeOffset now) => left.IsFuture(now);

        /// <summary>
        /// Enumerates hour ticks from start (inclusive) while strictly before end.
        /// </summary>
        public IEnumerable<DateTimeOffset> EnumerateHours()
        {
            for (var current = left.Start!.Value.Value;
                 current < left.End!.Value.Value;
                 current = current.AddHours(1))
            {
                yield return current;
            }
        }

        /// <summary>
        /// Enumerates calendar-day midnights (in the start offset) from the start date while strictly before end.
        /// </summary>
        public IEnumerable<DateTimeOffset> EnumerateDays()
        {
            var start = left.Start!.Value.Value;
            for (var current = new DateTimeOffset(start.Year, start.Month, start.Day, 0, 0, 0, start.Offset);
                 current < left.End!.Value.Value;
                 current = current.AddDays(1))
            {
                yield return current;
            }
        }

        /// <summary>
        /// Fixed-duration gap between two non-overlapping periods; <see cref="TimeSpan.Zero"/> if they overlap.
        /// For the gap as an interval, use the generic interval <c>Gap</c> method.
        /// </summary>
        public TimeSpan TimeGap(DateTimeOffsetPeriod right) =>
            left.Overlaps(right)
                ? TimeSpan.Zero
                : left.End!.Value.Value < right.Start!.Value.Value
                    ? right.Start.Value.Value - left.End.Value.Value
                    : left.Start!.Value.Value - right.End!.Value.Value;

        /// <summary>
        /// Gets the start time-of-day component.
        /// </summary>
        public TimeSpan StartTimeOfDay => left.Start!.Value.Value.TimeOfDay;

        /// <summary>
        /// End time-of-day, collapsing sub-minute intervals to the start time when below <paramref name="minimumDuration"/>.
        /// </summary>
        public TimeSpan EndTimeOfDay(TimeSpan minimumDuration = default)
        {
            var minimum = minimumDuration == TimeSpan.Zero ? TimeSpan.FromMinutes(1) : minimumDuration;
            var start = left.StartTimeOfDay;
            var end = left.End!.Value.Value.TimeOfDay;

            return end > start && end - start < minimum ? start : end;
        }

        /// <summary>
        /// Gets a value indicating whether the exclusive end falls on a later calendar day than the start (using each bound's local date).
        /// </summary>
        public bool SpansMidnight => left.End!.Value.Value.Date > left.Start!.Value.Value.Date;
    }
}
