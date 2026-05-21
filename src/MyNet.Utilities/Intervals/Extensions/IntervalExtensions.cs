// -----------------------------------------------------------------------
// <copyright file="IntervalExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Utilities.Intervals;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class IntervalExtensions
{
    extension<T>(IInterval<T> left)
        where T : struct, IComparable<T>
    {
        /// <summary>
        /// Determines whether the current interval is strictly before the specified interval. An interval A is considered to be before another interval B if the end of A is less than the start of B, taking into account the inclusivity of the boundaries. If either interval does not have a defined end or start, respectively, the method returns false, as it cannot determine a clear ordering between the intervals.
        /// </summary>
        /// <param name="right">The interval to compare with the current interval.</param>
        /// <returns><c>true</c> if the current interval is strictly before the specified interval; otherwise, <c>false</c>.</returns>
        public bool IsBefore(IInterval<T> right)
        {
            if (!left.HasEnd || !right.HasStart)
                return false;

            var comparison =
                left.End!.Value.Value.CompareTo(right.Start!.Value.Value);

            return comparison < 0 || (comparison <= 0 && (!left.End.Value.IsInclusive || !right.Start.Value.IsInclusive));
        }

        /// <summary>
        /// Determines whether the current interval is strictly after the specified interval. An interval A is considered to be after another interval B if the start of A is greater than the end of B, taking into account the inclusivity of the boundaries. If either interval does not have a defined start or end, respectively, the method returns false, as it cannot determine a clear ordering between the intervals.
        /// </summary>
        /// <param name="right">The interval to compare with the current interval.</param>
        /// <returns><c>true</c> if the current interval is strictly after the specified interval; otherwise, <c>false</c>.</returns>
        public bool IsAfter(IInterval<T> right) => right.IsBefore(left);

        /// <summary>
        /// Determines whether the current interval touches the specified interval. Two intervals are considered to touch if they are adjacent to each other without overlapping, meaning that the end of one interval is exactly equal to the start of the other interval, and at least one of the boundaries is inclusive. If either interval does not have a defined end or start, respectively, the method returns false, as it cannot determine if they touch.
        /// </summary>
        /// <param name="right">The interval to compare with the current interval.</param>
        /// <returns><c>true</c> if the current interval touches the specified interval; otherwise, <c>false</c>.</returns>
        public bool Touches(IInterval<T> right)
        {
            var touchesOnRight = false;
            if (left.HasEnd && right.HasStart)
            {
                var comparison = left.End!.Value.Value.CompareTo(right.Start!.Value.Value);
                touchesOnRight = comparison == 0 && !(left.End.Value.IsInclusive && right.Start.Value.IsInclusive);
            }

            var touchesOnLeft = false;
            if (right.HasEnd && left.HasStart)
            {
                var comparison = right.End!.Value.Value.CompareTo(left.Start!.Value.Value);
                touchesOnLeft = comparison == 0 && !(right.End.Value.IsInclusive && left.Start.Value.IsInclusive);
            }

            return touchesOnRight || touchesOnLeft;
        }

        /// <summary>
        /// Determines whether the current interval overlaps with the specified interval. Two intervals are considered to overlap if they have any points in common, meaning that the start of one interval is less than the end of the other interval, and the end of one interval is greater than the start of the other interval. If either interval does not have a defined start or end, respectively, the method returns false, as it cannot determine if they overlap.
        /// </summary>
        /// <param name="right">The interval to compare with the current interval.</param>
        /// <returns><c>true</c> if the current interval overlaps with the specified interval; otherwise, <c>false</c>.</returns>
        public bool Overlaps(IInterval<T> right) => left.Intersects(right);

        /// <summary>
        /// Determines whether the current interval is entirely contained within the specified container interval. An interval A is considered to be inside another interval B if all points of A are also points of B, meaning that the start of A is greater than or equal to the start of B and the end of A is less than or equal to the end of B, taking into account the inclusivity of the boundaries. If either interval does not have a defined start or end, respectively, the method returns false, as it cannot determine if one interval is inside the other.
        /// </summary>
        /// <param name="container">The interval to check against.</param>
        /// <returns><c>true</c> if the current interval is inside the specified container interval; otherwise, <c>false</c>.</returns>
        public bool IsInside(IInterval<T> container) => container.Contains(left);

        /// <summary>
        /// Determines whether the current interval starts before the specified interval. An interval A is considered to start before another interval B if the start of A is less than the start of B, taking into account the inclusivity of the boundaries. If either interval does not have a defined start, respectively, the method returns false, as it cannot determine a clear ordering between the intervals.
        /// </summary>
        /// <param name="right">The interval to compare with the current interval.</param>
        /// <returns><c>true</c> if the current interval starts before the specified interval; otherwise, <c>false</c>.</returns>
        public bool StartsBefore(IInterval<T> right)
            => !left.HasStart ? right.HasStart : right.HasStart && left.Start!.Value.Value.CompareTo(right.Start!.Value.Value) < 0;

        /// <summary>
        /// Determines whether the current interval ends after the specified interval. An interval A is considered to end after another interval B if the end of A is greater than the end of B, taking into account the inclusivity of the boundaries. If either interval does not have a defined end, respectively, the method returns false, as it cannot determine a clear ordering between the intervals.
        /// </summary>
        /// <param name="right">The interval to compare with the current interval.</param>
        /// <returns><c>true</c> if the current interval ends after the specified interval; otherwise, <c>false</c>.</returns>
        public bool EndsAfter(IInterval<T> right)
            => !left.HasEnd ? right.HasEnd : right.HasEnd && left.End!.Value.Value.CompareTo(right.End!.Value.Value) > 0;

        /// <summary>
        /// Clamps a value to be within the current interval. If the value is less than the start of the interval, it returns the start value. If the value is greater than the end of the interval, it returns the end value. If the value is within the interval, it returns the value itself. This method ensures that a given value falls within the defined range of the interval, effectively "clamping" it to the nearest boundary if it falls outside.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <returns>The clamped value.</returns>
        public T Clamp(T value) =>
            left.HasStart && value.CompareTo(left.Start!.Value.Value) < 0
                ? left.Start.Value.Value
                : left.HasEnd && value.CompareTo(left.End!.Value.Value) > 0
                    ? left.End.Value.Value
                    : value;

        /// <summary>
        /// Determines whether the current interval is empty, meaning that it has no points in common. An interval is considered empty if its start and end boundaries are defined and are equal to each other, but at least one of the boundaries is exclusive. If either boundary is not defined, the method returns false, as it cannot determine if the interval is empty.
        /// </summary>
        /// <returns>True if the interval is empty; otherwise, false.</returns>
        public bool IsEmpty()
        {
            if (!left.HasStart || !left.HasEnd)
                return false;

            var comparison = left.Start!.Value.Value.CompareTo(left.End!.Value.Value);

            return comparison == 0 && (!left.Start.Value.IsInclusive || !left.End.Value.IsInclusive);
        }
    }
}
