// -----------------------------------------------------------------------
// <copyright file="DateRangeExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Primitives.Intervals;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Primitives;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class DateRangeExtensions
{
    extension(DateRange left)
    {
        /// <summary>
        /// Determines whether this date range is adjacent to another date range.
        /// </summary>
        /// <param name="right">The other date range to check adjacency with.</param>
        /// <returns><c>true</c> if the date ranges are adjacent; otherwise, <c>false</c>.</returns>
        public bool IsAdjacentTo(DateRange right) => left.End!.Value.Value.AddDays(1) == right.Start!.Value.Value || right.End!.Value.Value.AddDays(1) == left.Start!.Value.Value;

        /// <summary>
        /// Splits the date range into multiple date ranges, each representing a single month. The resulting date ranges will cover the entire original date range without overlapping, and each will start on the first day of the month and end on the last day of the month (or the end of the original date range if it ends before the last day of the month).
        /// </summary>
        /// <returns>An enumerable of date ranges, each representing a single month.</returns>
        public IEnumerable<DateRange> SplitByMonth()
        {
            var current = new DateOnly(left.Start!.Value.Value.Year, left.Start.Value.Value.Month, 1);

            while (current <= left.End!.Value.Value)
            {
                var monthStart = current < left.Start.Value.Value ? left.Start.Value.Value : current;

                var monthEnd =
                    current.AddMonths(1).AddDays(-1);

                if (monthEnd > left.End.Value.Value)
                {
                    monthEnd = left.End.Value.Value;
                }

                yield return new(monthStart, monthEnd);

                current = current.AddMonths(1);
            }
        }

        /// <summary>
        /// Splits the date range into multiple date ranges, each representing a single week. The resulting date ranges will cover the entire original date range without overlapping, and each will start on the specified first day of the week and end on the last day of the week (or the end of the original date range if it ends before the last day of the week).
        /// </summary>
        /// <param name="firstDayOfWeek">The first day of the week to use when splitting the date range.</param>
        /// <returns>An enumerable of date ranges, each representing a single week.</returns>
        public IEnumerable<DateRange> SplitByWeek(DayOfWeek firstDayOfWeek = DayOfWeek.Monday)
        {
            var current = left.Start!.Value.Value;

            while (current <= left.End!.Value.Value)
            {
                var offset = ((int)current.DayOfWeek - (int)firstDayOfWeek + 7) % 7;

                var weekStart = current.AddDays(-offset);

                if (weekStart < left.Start.Value.Value)
                {
                    weekStart = left.Start.Value.Value;
                }

                var weekEnd = weekStart.AddDays(6);

                if (weekEnd > left.End.Value.Value)
                {
                    weekEnd = left.End.Value.Value;
                }

                yield return new(weekStart, weekEnd);

                current = weekEnd.AddDays(1);
            }
        }

        /// <summary>
        /// Expands the date range by a specified number of days on both sides. The resulting date range will start the specified number of days before the original start date and end the specified number of days after the original end date.
        /// </summary>
        /// <param name="days">The number of days to expand the date range by on each side.</param>
        /// <returns>A new date range expanded by the specified number of days on both sides.</returns>
        public DateRange Expand(int days)
            => new(left.Start!.Value.Value.AddDays(-days), left.End!.Value.Value.AddDays(days));
    }
}
