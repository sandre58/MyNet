// -----------------------------------------------------------------------
// <copyright file="IDateTimeOffsetInterval.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

using MyNet.Primitives.Intervals;

namespace MyNet.Temporal.Intervals;

/// <summary>
/// Contract for a temporal interval whose bounds are <see cref="DateTimeOffset"/> values (absolute instants with offset).
/// Comparison and containment use the underlying UTC instant, so equal instants with different offsets compare as equal.
/// </summary>
public interface IDateTimeOffsetInterval
{
    /// <summary>
    /// Gets the duration of the interval, or <see langword="null"/> when either bound is missing.
    /// </summary>
    TimeSpan? Duration { get; }
}
