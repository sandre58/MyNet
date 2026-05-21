// -----------------------------------------------------------------------
// <copyright file="ITimeSpanQuantizer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.Utilities.Temporal.Decomposition;

/// <summary>
/// Applies optional fuzzy rounding to already decomposed unit values.
/// </summary>
public interface ITimeSpanQuantizer
{
    /// <summary>
    /// Quantizes decomposed values using human-friendly thresholds.
    /// </summary>
    /// <param name="values">Exact values returned by a decomposer.</param>
    /// <returns>Rounded values suitable for human display.</returns>
    /// <example>
    /// (59, Second) can become (1, Minute).
    /// </example>
    IReadOnlyList<TimeUnitValue> Quantize(IReadOnlyList<TimeUnitValue> values);
}
