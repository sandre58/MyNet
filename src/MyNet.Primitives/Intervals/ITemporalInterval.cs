// -----------------------------------------------------------------------
// <copyright file="ITemporalInterval.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Primitives.Intervals;

public interface ITemporalInterval
{
    /// <summary>
    /// Gets the duration of the interval as a TimeSpan. If either the start or end boundary is not defined, this property returns null, indicating that the duration cannot be calculated due to the unbounded nature of the interval on one or both sides. If both boundaries are defined, it calculates the duration by subtracting the start value from the end value, providing a TimeSpan that represents the length of time covered by the interval.
    /// </summary>
    TimeSpan Duration { get; }
}
