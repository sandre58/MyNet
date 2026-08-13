// -----------------------------------------------------------------------
// <copyright file="IDateTimeInterval.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

using MyNet.Primitives.Intervals;

namespace MyNet.Temporal.Intervals;

/// <summary>
/// Contract for a temporal interval whose bounds are <see cref="DateTime"/> values (civil / UI time).
/// <see cref="DateTime.Kind"/> is preserved as provided by the caller; this library does not normalize Kind.
/// Unspecified Kind is treated as opaque civil time owned by the consumer.
/// </summary>
public interface IDateTimeInterval
{
    /// <summary>
    /// Gets the duration of the interval, or <see langword="null"/> when either bound is missing.
    /// </summary>
    TimeSpan? Duration { get; }
}
