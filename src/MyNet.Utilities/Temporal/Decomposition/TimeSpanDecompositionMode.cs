// -----------------------------------------------------------------------
// <copyright file="TimeSpanDecompositionMode.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Temporal.Decomposition;

/// <summary>
/// Defines how a TimeSpan should be decomposed into its components (weeks, days, hours, etc.).
/// </summary>
public enum TimeSpanDecompositionMode
{
    /// <summary>
    /// Keeps a natural hierarchical decomposition using multiple units when needed.
    /// <example>
    /// 2 weeks, 4 days, 3 hours
    /// </example>
    /// </summary>
    Hierarchical,

    /// <summary>
    /// Collapses the entire duration into the largest possible unit only.
    /// <example>
    /// 2 weeks (instead of 14 days or more granular breakdown)
    /// </example>
    /// </summary>
    LargestUnitOnly,

    /// <summary>
    /// Collapses the entire duration into the smallest allowed unit only.
    /// <example>
    /// 1344 hours (instead of weeks/days/hours)
    /// </example>
    /// </summary>
    SmallestUnitOnly,

    /// <summary>
    /// Performs a flat decomposition without hierarchical semantics.
    /// Each unit is computed independently in a purely mathematical way.
    /// <example>
    /// 14 days, 0 hours, 0 minutes (no grouping logic applied)
    /// </example>
    /// </summary>
    Flat
}
