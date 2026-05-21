// -----------------------------------------------------------------------
// <copyright file="IBoundedInterval.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Intervals;

/// <summary>
/// Represents a bounded interval where both the start and end boundaries are defined. This interface extends <see cref="IInterval{T}"/> and provides stronger guarantees about the presence of boundaries, allowing for more specific operations that require both a start and an end. Implementations of this interface must ensure that both the <see cref="Start"/> and <see cref="End"/> properties return non-null values, indicating that the interval is fully bounded on both sides.
/// </summary>
/// <typeparam name="T">The type of the values in the interval.</typeparam>
public interface IBoundedInterval<T> : IInterval<T>
    where T : struct, IComparable<T>
{
    /// <summary>
    /// Gets the start boundary of the interval, which is guaranteed to be non-null. The boundary includes information about whether it is inclusive or exclusive.
    /// </summary>
    new IntervalBoundary<T> Start { get; }

    /// <summary>
    /// Gets the end boundary of the interval, which is guaranteed to be non-null. The boundary includes information about whether it is inclusive or exclusive.
    /// </summary>
    new IntervalBoundary<T> End { get; }
}
