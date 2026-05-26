// -----------------------------------------------------------------------
// <copyright file="IOpenInterval.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Primitives.Intervals;

/// <summary>
/// Represents an open interval where both the start and end boundaries are defined and are exclusive. This interface extends <see cref="IBoundedInterval{T}"/> and provides specific guarantees that both boundaries are exclusive, meaning that the interval does not include its start and end values. Implementations of this interface must ensure that the <see cref="IBoundedInterval{T}.Start"/> and <see cref="IBoundedInterval{T}.End"/> properties return boundaries that are marked as exclusive, making it suitable for scenarios where strict inequalities are required (e.g., (a, b) instead of [a, b]).
/// </summary>
/// <typeparam name="T">The type of the values in the interval.</typeparam>
public interface IOpenInterval<T> : IBoundedInterval<T>
    where T : struct, IComparable<T>;
