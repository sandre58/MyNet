// -----------------------------------------------------------------------
// <copyright file="IClosedInterval.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Intervals;

/// <summary>
/// Represents a closed interval where both the start and end boundaries are defined and are inclusive. This interface extends <see cref="IBoundedInterval{T}"/> and provides specific guarantees that both boundaries are inclusive, meaning that the interval includes its start and end values. Implementations of this interface must ensure that the <see cref="IBoundedInterval{T}.Start"/> and <see cref="IBoundedInterval{T}.End"/> properties return boundaries that are marked as inclusive, making it suitable for scenarios where non-strict inequalities are required (e.g., [a, b] instead of (a, b]).
/// </summary>
/// <typeparam name="T">The type of the values in the interval.</typeparam>
public interface IClosedInterval<T> : IBoundedInterval<T>
    where T : struct, IComparable<T>;
