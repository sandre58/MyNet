// -----------------------------------------------------------------------
// <copyright file="Interval.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MyNet.Utilities;

namespace MyNet.Primitives.Intervals;

/// <summary>
/// Represents an interval of values of type T, where T is a struct that implements <see cref="IComparable{T}"/>. This class provides a concrete implementation of the <see cref="Interval{T, TSelf}"/> abstract class, allowing you to create intervals with specific start and end boundaries. The constructor takes nullable parameters for the start and end boundaries, which can be used to create unbounded intervals by passing null for either parameter. The class also includes methods for checking if a value is contained within the interval, if it intersects with another interval, and for merging intervals when possible. By using this class, you can easily work with ranges of values in a type-safe manner, ensuring that the intervals are valid and providing useful operations for manipulating them.
/// </summary>
/// <param name="start">The start boundary of the interval.</param>
/// <param name="end">The end boundary of the interval.</param>
/// <param name="allowWrappedBounds">Indicates whether wrapped bounds are allowed.</param>
/// <exception cref="ArgumentException">Thrown when the start boundary is greater than the end boundary or when the interval is empty.</exception>
public class Interval<T>(IntervalBoundary<T>? start, IntervalBoundary<T>? end, bool allowWrappedBounds = false)
    : Interval<T, Interval<T>>(start, end, allowWrappedBounds)
    where T : struct, IComparable<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Interval{T}"/> class with the specified start and end values. The constructor takes nullable parameters for the start and end values, which can be used to create unbounded intervals by passing null for either parameter. If both parameters have values, they are converted into <see cref="IntervalBoundary{T}"/> instances, which indicate that the boundaries are inclusive by default. This constructor provides a convenient way to create intervals using simple values without having to explicitly create <see cref="IntervalBoundary{T}"/> instances, while still allowing for the flexibility of unbounded intervals when needed.
    /// </summary>
    /// <param name="start">The start value of the interval.</param>
    /// <param name="to">The end value of the interval.</param>
    /// <param name="isInclusive">Indicates whether the boundaries are inclusive.</param>
    public Interval(T? start, T? to, bool isInclusive = true)
        : this(start.HasValue ? new IntervalBoundary<T>(start.Value, isInclusive) : null, to.HasValue ? new IntervalBoundary<T>(to.Value, isInclusive) : null)
    {
    }

    /// <inheritdoc/>
    protected override Interval<T> Create(IntervalBoundary<T>? start, IntervalBoundary<T>? end) => new(start, end);
}

/// <summary>
/// Represents an interval of values of type T, where T is a struct that implements <see cref="IComparable{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the values in the interval.</typeparam>
/// <typeparam name="TSelf">The type of the interval.</typeparam>
public abstract class Interval<T, TSelf> : IInterval<T>
    where T : struct, IComparable<T>
    where TSelf : Interval<T, TSelf>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Interval{T, TSelf}"/> class with the specified start and end boundaries. The start and end boundaries are represented by <see cref="IntervalBoundary{T}"/> structs, which indicate the value of the boundary and whether it is inclusive or exclusive. If both boundaries are specified, the constructor checks that the start boundary is less than or equal to the end boundary, and that if they are equal, both boundaries must be inclusive. If these conditions are not met, an <see cref="ArgumentException"/> is thrown.
    /// </summary>
    /// <param name="start">The start boundary of the interval.</param>
    /// <param name="end">The end boundary of the interval.</param>
    /// <param name="allowWrappedBounds">Indicates whether wrapped bounds are allowed.</param>
    /// <exception cref="ArgumentException">Thrown when the start boundary is greater than the end boundary or when the interval is empty.</exception>
    protected Interval(IntervalBoundary<T>? start, IntervalBoundary<T>? end, bool allowWrappedBounds = false)
    {
        if (start.HasValue && end.HasValue)
        {
            var comparison = start.Value.Value.CompareTo(end.Value.Value);

            switch (comparison)
            {
                case > 0 when !allowWrappedBounds:
                    throw new ArgumentException("Start must be before end.");
                case 0 when !start.Value.IsInclusive || !end.Value.IsInclusive:
                    throw new ArgumentException("Empty interval.");
            }
        }

        Start = start;
        End = end;
    }

    /// <summary>
    /// Gets the start boundary of the interval. The start boundary is represented by an <see cref="IntervalBoundary{T}"/> struct, which indicates the value of the boundary and whether it is inclusive or exclusive. If the start boundary is not specified, it is considered to be unbounded on the lower end, meaning that it includes all values less than or equal to the end boundary (if specified) or all values if the end boundary is also unbounded.
    /// </summary>
    public IntervalBoundary<T>? Start { get; }

    /// <summary>
    /// Gets the end boundary of the interval. The end boundary is represented by an <see cref="IntervalBoundary{T}"/> struct, which indicates the value of the boundary and whether it is inclusive or exclusive. If the end boundary is not specified, it is considered to be unbounded on the upper end, meaning that it includes all values greater than or equal to the start boundary (if specified) or all values if the start boundary is also unbounded).
    /// </summary>
    public IntervalBoundary<T>? End { get; }

    /// <summary>
    /// Gets a value indicating whether the interval has a start boundary. An interval is considered to have a start boundary if the Start property has a value, meaning that it is not null. If the Start property is null, the interval is considered to be unbounded on the lower end, and therefore does not have a start boundary.
    /// </summary>
    public bool HasStart => Start.HasValue;

    /// <summary>
    /// Gets a value indicating whether the interval has an end boundary. An interval is considered to have an end boundary if the End property has a value, meaning that it is not null. If the End property is null, the interval is considered to be unbounded on the upper end, and therefore does not have an end boundary.
    /// </summary>
    public bool HasEnd => End.HasValue;

    /// <summary>
    /// Gets a value indicating whether the interval is unbounded on the lower end. An interval is considered unbounded on the lower end if it does not have a start boundary.
    /// </summary>
    public bool IsUnboundedStart => !HasStart;

    /// <summary>
    /// Gets a value indicating whether the interval is unbounded on the upper end. An interval is considered unbounded on the upper end if it does not have an end boundary.
    /// </summary>
    public bool IsUnboundedEnd => !HasEnd;

    /// <summary>
    /// Gets a value indicating whether the interval represents a single point. An interval is considered a point if it has both a start and an end boundary, and the values of these boundaries are equal.
    /// </summary>
    public bool IsPoint => HasStart && HasEnd && Start!.Value.Value.CompareTo(End!.Value.Value) == 0;

    /// <summary>
    /// Merges a collection of intervals by combining any overlapping or adjacent intervals into a single interval. The method first orders the intervals, then iterates through them, merging intervals that can be merged using the TryMerge method. The result is a new collection of intervals where no two intervals overlap or touch each other, providing a simplified representation of the covered ranges. This is particularly useful for scenarios where you want to consolidate multiple intervals into a more concise form, such as when dealing with time ranges, numeric ranges, or any other comparable types that can be represented as intervals.
    /// </summary>
    /// <param name="intervals">The collection of intervals to merge.</param>
    /// <returns>A collection of merged intervals.</returns>
    public static IReadOnlyCollection<TSelf> MergeOverlapping(IEnumerable<TSelf> intervals)
    {
        var ordered = intervals.Order().ToList();

        if (ordered.Count == 0)
            return [];

        var result = new List<TSelf>();

        var current = ordered[0];

        for (var i = 1; i < ordered.Count; i++)
        {
            var next = ordered[i];

            if (current.TryMerge(next, out var merged))
            {
                current = merged;
                continue;
            }

            result.Add(current);
            current = next;
        }

        result.Add(current);

        return result;
    }

    /// <summary>
    /// Determines whether the current interval is adjacent to another interval. Two intervals are considered adjacent if they share a common boundary, meaning that the end of one interval coincides with the start of the other interval. This method checks for adjacency by comparing the end boundary of the current interval with the start boundary of the other interval, and vice versa. If either comparison indicates that the boundaries are equal, the intervals are considered adjacent, and the method returns true; otherwise, it returns false.
    /// </summary>
    /// <param name="right">The interval to check for adjacency.</param>
    /// <returns><c>true</c> if the intervals are adjacent; otherwise, <c>false</c>.</returns>
    public bool IsAdjacentTo(TSelf right) => End?.Value.CompareNullableTo(right.Start?.Value) == 0 || right.End?.Value.CompareNullableTo(Start?.Value) == 0;

    /// <summary>
    /// Creates a new instance of the interval with the specified start and end boundaries. This method is abstract and must be implemented by derived classes to return an instance of the specific interval type they represent. The start and end parameters are nullable, allowing for the creation of unbounded intervals by passing null for either parameter. The implementation of this method should ensure that the created interval adheres to the constraints defined in the constructor, such as ensuring that the start boundary is not greater than the end boundary and that empty intervals are not created.
    /// </summary>
    /// <param name="start">The start boundary of the interval.</param>
    /// <param name="end">The end boundary of the interval.</param>
    /// <returns>A new instance of the interval with the specified boundaries.</returns>
    protected abstract TSelf Create(IntervalBoundary<T>? start, IntervalBoundary<T>? end);

    /// <summary>
    /// Determines whether the interval contains a specified value.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns><c>true</c> if the interval contains the specified value; otherwise, <c>false</c>.</returns>
    public virtual bool Contains(T value)
    {
        var afterStart =
            !HasStart ||
            value.CompareTo(Start!.Value.Value) > 0 ||
            (value.CompareTo(Start.Value.Value) == 0 && Start.Value.IsInclusive);

        var beforeEnd =
            !HasEnd ||
            value.CompareTo(End!.Value.Value) < 0 ||
            (value.CompareTo(End.Value.Value) == 0 && End.Value.IsInclusive);

        return afterStart && beforeEnd;
    }

    /// <summary>
    /// Determines whether the current interval contains another interval. An interval A is considered to contain another interval B if every value that is contained in B is also contained in A. This means that the start boundary of A must be less than or equal to the start boundary of B, and the end boundary of A must be greater than or equal to the end boundary of B. If either interval is unbounded on one end, the comparison for that end is considered to be satisfied.
    /// </summary>
    /// <param name="other">The interval to check for containment.</param>
    /// <returns><c>true</c> if the current interval contains the specified interval; otherwise, <c>false</c>.</returns>
    public virtual bool Contains(IInterval<T> other) => ContainsStart(other) && ContainsEnd(other);

    /// <summary>
    /// Determines whether the current interval intersects with another interval. Two intervals are considered to intersect if they have at least one value in common. This means that the start boundary of one interval must be less than or equal to the end boundary of the other interval, and the end boundary of one interval must be greater than or equal to the start boundary of the other interval. If either interval is unbounded on one end, the comparison for that end is considered to be satisfied.
    /// </summary>
    /// <param name="other">The interval to check for intersection.</param>
    /// <returns><c>true</c> if the current interval intersects with the specified interval; otherwise, <c>false</c>.</returns>
    public virtual bool Intersects(IInterval<T> other)
    {
        if (End.HasValue && other.Start.HasValue)
        {
            var comparison = End.Value.Value.CompareTo(other.Start.Value.Value);

            switch (comparison)
            {
                case < 0:
                case 0 when !End.Value.IsInclusive || !other.Start.Value.IsInclusive:
                    return false;
            }
        }

        if (other.End.HasValue && Start.HasValue)
        {
            var comparison = other.End.Value.Value.CompareTo(Start.Value.Value);

            switch (comparison)
            {
                case < 0:
                case 0 when !other.End.Value.IsInclusive || !Start.Value.IsInclusive:
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Determines whether the current interval can be merged with the specified interval. Two intervals can be merged if they either overlap or touch each other, meaning that they have points in common or are adjacent without any gap between them. If either interval does not have a defined start or end, respectively, the method returns false, as it cannot determine if they can be merged.
    /// </summary>
    /// <param name="right">The interval to compare with the current interval.</param>
    /// <returns><c>true</c> if the current interval can be merged with the specified interval; otherwise, <c>false</c>.</returns>
    public bool CanMerge(TSelf right) => Intersects(right) || this.Touches(right);

    /// <summary>
    /// Attempts to merge the current interval with the specified interval. If the intervals can be merged (i.e., they overlap or touch), it creates a new interval that encompasses both intervals and returns it through the out parameter. If the intervals cannot be merged, it returns false and sets the out parameter to null. The merged interval is created by taking the minimum start boundary and the maximum end boundary of the two intervals, ensuring that it covers all points contained in either of the original intervals.
    /// </summary>
    /// <param name="right">The interval to merge with the current interval.</param>
    /// <param name="merged">When this method returns, contains the merged interval if the merge was successful; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the intervals were successfully merged; otherwise, <c>false</c>.</returns>
    public bool TryMerge(TSelf right, [NotNullWhen(true)] out TSelf? merged)
    {
        if (!CanMerge(right))
        {
            merged = null;
            return false;
        }

        var start = MinStart((TSelf)this, right);
        var end = MaxEnd((TSelf)this, right);

        merged = Create(start, end);

        return true;
    }

    /// <summary>
    /// Calculates the intersection of the current interval with the specified interval. If the intervals intersect, it creates a new interval that represents the overlapping portion of the two intervals and returns it. If the intervals do not intersect, it returns null. The intersection interval is created by taking the maximum start boundary and the minimum end boundary of the two intervals, ensuring that it covers only the points that are contained in both of the original intervals.
    /// </summary>
    /// <param name="right">The interval to intersect with the current interval.</param>
    /// <returns>The intersection interval if the intervals intersect; otherwise, <c>null</c>.</returns>
    public TSelf? Intersection(TSelf right)
    {
        if (!Intersects(right))
            return null;

        var start = MaxStart((TSelf)this, right);
        var end = MinEnd((TSelf)this, right);

        return Create(start, end);
    }

    /// <summary>
    /// Calculates the union of the current interval with the specified interval. If the intervals can be merged (i.e., they overlap or touch), it returns a collection containing a single interval that encompasses both intervals. If the intervals cannot be merged, it returns a collection containing the two original intervals in sorted order. The union operation effectively combines the ranges covered by both intervals, providing a simplified representation of the total coverage when possible.
    /// </summary>
    /// <param name="right">The interval to intersect with the current interval.</param>
    /// <returns>The union interval if the intervals union; otherwise, <c>null</c>.</returns>
    public IReadOnlyCollection<TSelf> Union(TSelf right)
        => TryMerge(right, out var merged) ? [merged] : new List<TSelf> { (TSelf)this, right }.Order().ToArray();

    /// <summary>
    /// Calculates the gap between the current interval and the specified interval. If the intervals do not intersect or touch each other, it creates a new interval that represents the gap between them and returns it. If the intervals intersect or touch, it returns null, indicating that there is no gap between them. The gap interval is created by taking the end boundary of the earlier interval and the start boundary of the later interval, ensuring that it covers only the points that are not contained in either of the original intervals but lie between them.
    /// </summary>
    /// <param name="right">The interval to intersect with the current interval.</param>
    /// <returns>The gap interval if the intervals do not intersect or touch; otherwise, <c>null</c>.</returns>
    public TSelf? Gap(TSelf right)
    {
        if (Intersects(right) || this.Touches(right))
            return null;

        var ordered = CompareTo(right) <= 0 ? ((TSelf)this, right) : (right, (TSelf)this);

        return Create(new IntervalBoundary<T>(ordered.Item1.End!.Value.Value, !ordered.Item1.End.Value.IsInclusive),
            new IntervalBoundary<T>(ordered.Item2.Start!.Value.Value, !ordered.Item2.Start.Value.IsInclusive));
    }

    /// <summary>
    /// Expands the current interval to include the specified value. If the value is outside the current interval, it creates a new interval that encompasses both the original interval and the value. The method takes an optional parameter to specify whether the new boundaries should be inclusive or exclusive. If the value is already within the current interval, it returns the original interval without modification. This method provides a way to dynamically adjust the boundaries of an interval to ensure that it includes a specific value.
    /// </summary>
    /// <param name="value">The value to include in the interval.</param>
    /// <param name="inclusive">Specifies whether the new boundaries should be inclusive or exclusive.</param>
    /// <returns>A new interval that includes the specified value.</returns>
    public TSelf ExpandTo(T value, bool inclusive = true)
    {
        var start = Start;
        var end = End;

        if (!HasStart || value.CompareTo(Start!.Value.Value) < 0)
        {
            start = new IntervalBoundary<T>(value, inclusive);
        }

        if (!HasEnd || value.CompareTo(End!.Value.Value) > 0)
        {
            end = new IntervalBoundary<T>(value, inclusive);
        }

        return Create(start, end);
    }

    /// <summary>
    /// Expands the current interval to include another interval. If the other interval is outside the current interval, it creates a new interval that encompasses both the original interval and the other interval. The method takes an optional parameter to specify whether the new boundaries should be inclusive or exclusive. If the other interval is already within the current interval, it returns the original interval without modification. This method provides a way to dynamically adjust the boundaries of an interval to ensure that it includes another specified interval.
    /// </summary>
    /// <param name="other">The interval to include in the current interval.</param>
    /// <returns>A new interval that includes the specified interval.</returns>
    public IInterval<T> ExpandTo(TSelf other) => Create(MinStart((TSelf)this, other), MaxEnd((TSelf)this, other));

    /// <summary>
    /// Determines whether the current interval contains another interval. An interval A is considered to contain another interval B if every value that is contained in B is also contained in A. This means that the start boundary of A must be less than or equal to the start boundary of B, and the end boundary of A must be greater than or equal to the end boundary of B. If either interval is unbounded on one end, the comparison for that end is considered to be satisfied.
    /// </summary>
    /// <param name="other">The interval to check for containment.</param>
    /// <returns><c>true</c> if the current interval contains the specified interval; otherwise, <c>false</c>.</returns>
    private bool ContainsStart(IInterval<T> other)
    {
        if (!HasStart)
            return true;

        if (!other.HasStart)
            return false;

        var comparison =
            Start!.Value.Value.CompareTo(other.Start!.Value.Value);

        return comparison switch
        {
            < 0 => true,
            > 0 => false,
            _ => Start.Value.IsInclusive || !other.Start.Value.IsInclusive
        };
    }

    /// <summary>
    /// Determines whether the current interval contains another interval. An interval A is considered to contain another interval B if every value that is contained in B is also contained in A. This means that the start boundary of A must be less than or equal to the start boundary of B, and the end boundary of A must be greater than or equal to the end boundary of B. If either interval is unbounded on one end, the comparison for that end is considered to be satisfied.
    /// </summary>
    /// <param name="other">The interval to check for containment.</param>
    /// <returns><c>true</c> if the current interval contains the specified interval; otherwise, <c>false</c>.</returns>
    private bool ContainsEnd(IInterval<T> other)
    {
        if (!HasEnd)
            return true;

        if (!other.HasEnd)
            return false;

        var comparison =
            End!.Value.Value.CompareTo(other.End!.Value.Value);

        return comparison switch
        {
            > 0 => true,
            < 0 => false,
            _ => End.Value.IsInclusive || !other.End.Value.IsInclusive
        };
    }

    /// <summary>
    /// Determines whether two intervals are equal by comparing their boundaries. Two intervals are considered equal if they have the same start and end boundaries, meaning that their Start and End properties are equal. This operator overload provides a convenient way to compare intervals for equality using the == operator, allowing for more readable and intuitive code when working with intervals. By implementing this operator, you can easily check if two intervals represent the same range of values without having to call the Equals method explicitly.
    /// </summary>
    /// <param name="left">The left interval to compare.</param>
    /// <param name="right">The right interval to compare.</param>
    /// <returns><c>true</c> if the left interval is equal to the right interval; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Interval<T, TSelf> left, IInterval<T> right) => left.Equals(right);

    /// <summary>
    /// Determines whether two intervals are not equal by comparing their boundaries. Two intervals are considered not equal if they do not have the same start and end boundaries, meaning that either their Start properties or their End properties are not equal. This operator overload provides a convenient way to compare intervals for inequality using the != operator, allowing for more readable and intuitive code when working with intervals. By implementing this operator, you can easily check if two intervals represent different ranges of values without having to call the Equals method explicitly and negating its result.
    /// </summary>
    /// <param name="left">The left interval to compare.</param>
    /// <param name="right">The right interval to compare.</param>
    /// <returns><c>true</c> if the left interval is not equal to the right interval; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Interval<T, TSelf> left, IInterval<T> right) => !left.Equals(right);

    /// <summary>
    /// Determines whether the current interval is greater than another interval. An interval A is considered to be greater than another interval B if it starts after B, meaning that the start value of A is greater than the start value of B, or if the start values are equal and the end value of A is greater than the end value of B. If either interval is unbounded on one end, the comparison for that end is considered to be satisfied.
    /// </summary>
    /// <param name="left">The left interval to compare.</param>
    /// <param name="right">The right interval to compare.</param>
    /// <returns><c>true</c> if the left interval is greater than the right interval; otherwise, <c>false</c>.</returns>
    public static bool operator >(Interval<T, TSelf> left, IInterval<T> right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Determines whether the current interval is less than another interval. An interval A is considered to be less than another interval B if it starts before B, meaning that the start value of A is less than the start value of B, or if the start values are equal and the end value of A is less than the end value of B. If either interval is unbounded on one end, the comparison for that end is considered to be satisfied.
    /// </summary>
    /// <param name="left">The left interval to compare.</param>
    /// <param name="right">The right interval to compare.</param>
    /// <returns><c>true</c> if the left interval is less than the right interval; otherwise, <c>false</c>.</returns>
    public static bool operator <(Interval<T, TSelf> left, IInterval<T> right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Determines whether the current interval is greater than or equal to another interval. An interval A is considered to be greater than or equal to another interval B if it starts after B, meaning that the start value of A is greater than the start value of B, or if the start values are equal and the end value of A is greater than or equal to the end value of B. If either interval is unbounded on one end, the comparison for that end is considered to be satisfied.
    /// </summary>
    /// <param name="left">The left interval to compare.</param>
    /// <param name="right">The right interval to compare.</param>
    /// <returns><c>true</c> if the left interval is greater than or equal to the right interval; otherwise, <c>false</c>.</returns>
    public static bool operator >=(Interval<T, TSelf> left, IInterval<T> right) => left.CompareTo(right) >= 0;

    /// <summary>
    /// Determines whether the current interval is less than or equal to another interval. An interval A is considered to be less than or equal to another interval B if it starts before B, meaning that the start value of A is less than the start value of B, or if the start values are equal and the end value of A is less than or equal to the end value of B. If either interval is unbounded on one end, the comparison for that end is considered to be satisfied.
    /// </summary>
    /// <param name="left">The left interval to compare.</param>
    /// <param name="right">The right interval to compare.</param>
    /// <returns><c>true</c> if the left interval is less than or equal to the right interval; otherwise, <c>false</c>.</returns>
    public static bool operator <=(Interval<T, TSelf> left, IInterval<T> right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Compares the current interval with another interval and returns an integer that indicates whether the current interval is less than, equal to, or greater than the other interval. The comparison is based on the start values of the intervals, and if the start values are equal, the end values are compared. An interval is considered to be less than another interval if its start value is less than the start value of the other interval, or if the start values are equal and its end value is less than the end value of the other interval. An interval is considered to be greater than another interval if its start value is greater than the start value of the other interval, or if the start values are equal and its end value is greater than the end value of the other interval. An interval is considered to be equal to another interval if both its start and end values are equal to those of the other interval.
    /// </summary>
    /// <param name="other">The interval to compare with the current interval.</param>
    /// <returns>An integer that indicates whether the current interval is less than, equal to, or greater than the other interval.</returns>
    public int CompareTo(IInterval<T>? other)
    {
        if (other is null)
            return 1;

        switch (HasStart)
        {
            case false when other.HasStart:
                return -1;
            case true when !other.HasStart:
                return 1;
            case true:
                {
                    var startComparison = Start!.Value.Value.CompareTo(other.Start!.Value.Value);
                    if (startComparison != 0)
                        return startComparison;

                    if (Start.Value.IsInclusive != other.Start.Value.IsInclusive)
                        return Start.Value.IsInclusive ? -1 : 1;
                    break;
                }
        }

        switch (HasEnd)
        {
            case false when other.HasEnd:
                return 1;
            case true when !other.HasEnd:
                return -1;
            case false:
                return 0;
            default:
                {
                    var endComparison = End!.Value.Value.CompareTo(other.End!.Value.Value);
                    return endComparison != 0 ? endComparison : End.Value.IsInclusive == other.End.Value.IsInclusive ? 0 : End.Value.IsInclusive ? 1 : -1;
                }
        }
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current interval. Two intervals are considered equal if they have the same start and end boundaries, meaning that their Start and End properties are equal. This method overrides the default implementation to provide value-based equality semantics, allowing intervals to be compared based on their boundaries rather than their references. By implementing this method, you can ensure that intervals behave as expected when used in collections, comparisons, or any scenario where equality checks are performed.
    /// </summary>
    /// <param name="obj">The object to compare with the current interval.</param>
    /// <returns><c>true</c> if the specified object is equal to the current interval; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) => obj is IInterval<T> interval && Start.Equals(interval.Start) && End.Equals(interval.End);

    /// <summary>
    /// Returns a hash code for the current interval. The hash code is computed based on the Start and End properties of the interval, which represent its boundaries. By combining the hash codes of these properties, this method provides a way to generate a unique hash code for each distinct interval, allowing intervals to be used effectively in hash-based collections such as dictionaries or hash sets. This implementation ensures that intervals with the same boundaries will have the same hash code, while intervals with different boundaries will likely have different hash codes.
    /// </summary>
    /// <returns>A hash code for the current interval.</returns>
    public override int GetHashCode() => HashCode.Combine(Start, End);

    /// <summary>
    /// Returns a string that represents the current interval. The string is formatted as "Start - End", where Start and End are the string representations of the start and end values of the interval, respectively.
    /// </summary>
    /// <returns>A string that represents the current interval.</returns>
    public override string ToString()
    {
        var startBracket = Start?.IsInclusive == true ? "[" : "]";
        var endBracket = End?.IsInclusive == true ? "]" : "[";

        var start = HasStart ? Start!.Value.Value.ToString() : "-∞";
        var end = HasEnd ? End!.Value.Value.ToString() : "+∞";

        return $"{startBracket}{start}; {end}{endBracket}";
    }

    /// <summary>
    /// Determines the minimum start boundary between two intervals. If one of the intervals does not have a defined start boundary, it returns the start boundary of the other interval. If both intervals have defined start boundaries, it compares their values and returns the one that is smaller. In the case where both start boundaries are equal, it returns the one that is inclusive, if either is inclusive; otherwise, it returns either of them since they are effectively the same in terms of ordering. This method is used to calculate the starting point of a merged interval when combining two intervals.
    /// </summary>
    /// <param name="left">The first interval to compare.</param>
    /// <param name="right">The second interval to compare.</param>
    /// <returns>The minimum start boundary between the two intervals.</returns>
    private static IntervalBoundary<T>? MinStart(TSelf left, TSelf right)
    {
        if (!left.HasStart)
            return left.Start;

        if (!right.HasStart)
            return right.Start;

        var comparison =
            left.Start!.Value.Value.CompareTo(
                right.Start!.Value.Value);

        return comparison switch
        {
            < 0 => left.Start,
            > 0 => right.Start,
            _ => left.Start.Value.IsInclusive ? left.Start : right.Start
        };
    }

    /// <summary>
    /// Determines the maximum start boundary between two intervals. If one of the intervals does not have a defined start boundary, it returns the start boundary of the other interval. If both intervals have defined start boundaries, it compares their values and returns the one that is larger. In the case where both start boundaries are equal, it returns the one that is exclusive, if either is exclusive; otherwise, it returns either of them since they are effectively the same in terms of ordering. This method is used to calculate the starting point of an intersection interval when combining two intervals.
    /// </summary>
    /// <param name="left">The first interval to compare.</param>
    /// <param name="right">The second interval to compare.</param>
    /// <returns>The maximum start boundary between the two intervals.</returns>
    private static IntervalBoundary<T>? MaxStart(TSelf left, TSelf right)
    {
        if (!left.HasStart)
            return right.Start;

        if (!right.HasStart)
            return left.Start;

        var comparison =
            left.Start!.Value.Value.CompareTo(
                right.Start!.Value.Value);

        return comparison switch
        {
            > 0 => left.Start,
            < 0 => right.Start,
            _ => !left.Start.Value.IsInclusive ? left.Start : right.Start
        };
    }

    /// <summary>
    /// Determines the minimum end boundary between two intervals. If one of the intervals does not have a defined end boundary, it returns the end boundary of the other interval. If both intervals have defined end boundaries, it compares their values and returns the one that is smaller. In the case where both end boundaries are equal, it returns the one that is exclusive, if either is exclusive; otherwise, it returns either of them since they are effectively the same in terms of ordering. This method is used to calculate the ending point of an intersection interval when combining two intervals.
    /// </summary>
    /// <param name="left">The first interval to compare.</param>
    /// <param name="right">The second interval to compare.</param>
    /// <returns>The minimum end boundary between the two intervals.</returns>
    private static IntervalBoundary<T>? MinEnd(TSelf left, TSelf right)
    {
        if (!left.HasEnd)
            return right.End;

        if (!right.HasEnd)
            return left.End;

        var comparison =
            left.End!.Value.Value.CompareTo(
                right.End!.Value.Value);

        return comparison switch
        {
            < 0 => left.End,
            > 0 => right.End,
            _ => !left.End.Value.IsInclusive ? left.End : right.End
        };
    }

    /// <summary>
    /// Determines the maximum end boundary between two intervals. If one of the intervals does not have a defined end boundary, it returns the end boundary of the other interval. If both intervals have defined end boundaries, it compares their values and returns the one that is larger. In the case where both end boundaries are equal, it returns the one that is inclusive, if either is inclusive; otherwise, it returns either of them since they are effectively the same in terms of ordering. This method is used to calculate the ending point of a merged interval when combining two intervals.
    /// </summary>
    /// <param name="left">The first interval to compare.</param>
    /// <param name="right">The second interval to compare.</param>
    /// <returns>The maximum end boundary between the two intervals.</returns>
    private static IntervalBoundary<T>? MaxEnd(TSelf left, TSelf right)
    {
        if (!left.HasEnd)
            return left.End;

        if (!right.HasEnd)
            return right.End;

        var comparison = left.End!.Value.Value.CompareTo(right.End!.Value.Value);

        return comparison switch
        {
            > 0 => left.End,
            < 0 => right.End,
            _ => left.End.Value.IsInclusive ? left.End : right.End
        };
    }
}

/// <summary>
/// Represents a boundary of an interval, which can be either inclusive or exclusive. An inclusive boundary includes the value it represents in the interval, while an exclusive boundary does not include the value it represents in the interval. The <see cref="IntervalBoundary{T}"/> struct is immutable and can be used to define the boundaries of intervals in a flexible way, allowing for both open and closed intervals to be represented using the same structure.
/// </summary>
/// <param name="Value">The value of the boundary.</param>
/// <param name="IsInclusive">Indicates whether the boundary is inclusive.</param>
/// <typeparam name="T">The type of the value.</typeparam>
public readonly record struct IntervalBoundary<T>(T Value, bool IsInclusive = true)
    where T : struct, IComparable<T>;
