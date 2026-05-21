// -----------------------------------------------------------------------
// <copyright file="ITimeSpanDecomposer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.Utilities.Temporal.Decomposition;

/// <summary>
/// Decomposes a <see cref="TimeSpan"/> into exact unit values using configured options.
/// </summary>
public interface ITimeSpanDecomposer
{
    /// <summary>
    /// Decomposes the provided span into ordered <see cref="TimeUnitValue"/> items.
    /// </summary>
    /// <param name="timeSpan">The source duration to decompose.</param>
    /// <param name="options">Selection, precision, and optional quantization settings.</param>
    /// <returns>The ordered decomposition result.</returns>
    /// <example>
    /// 1h03m with strict options can return: (1, Hour), (3, Minute).
    /// </example>
    IReadOnlyList<TimeUnitValue> Decompose(TimeSpan timeSpan, TimeSpanDecompositionOptions options);
}
