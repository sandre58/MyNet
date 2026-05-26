// -----------------------------------------------------------------------
// <copyright file="ITimeSpanDecompositionBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.Primitives;

namespace MyNet.Temporal.Decomposition;

/// <summary>
/// Defines a builder for configuring and executing the decomposition of a TimeSpan into its constituent time units.
/// </summary>
public interface ITimeSpanDecompositionBuilder
{
    /// <summary>
    /// Configures the decomposition to produce a hierarchical structure of time units, where larger units are decomposed into smaller ones.
    /// </summary>
    /// <returns>Current builder.</returns>
    ITimeSpanDecompositionBuilder AsHierarchical();

    /// <summary>
    /// Configures the decomposition to produce a flat structure of time units, where all units are treated equally.
    /// </summary>
    /// <returns>Current builder.</returns>
    ITimeSpanDecompositionBuilder AsFlat();

    /// <summary>
    /// Configures the decomposition to use only the largest time unit.
    /// </summary>
    /// <returns>Current builder.</returns>
    ITimeSpanDecompositionBuilder UseLargestUnitOnly();

    /// <summary>
    /// Configures the decomposition to use only the smallest time unit.
    /// </summary>
    /// <returns>Current builder.</returns>
    ITimeSpanDecompositionBuilder UseSmallestUnitOnly();

    /// <summary>
    /// Configures the maximum number of components in the decomposition.
    /// </summary>
    /// <param name="count">The maximum number of components.</param>
    /// <returns>Current builder.</returns>
    ITimeSpanDecompositionBuilder MaxComponents(int count);

    /// <summary>
    /// Configures the minimum time unit to include in the decomposition. Units smaller than this will be excluded.
    /// </summary>
    /// <param name="minUnit">The minimum time unit to include.</param>
    /// <returns>Current builder.</returns>
    ITimeSpanDecompositionBuilder WithMinUnit(TimeUnit minUnit);

    /// <summary>
    /// Configures the maximum time unit to include in the decomposition. Units larger than this will be excluded.
    /// </summary>
    /// <param name="maxUnit">The maximum time unit to include.</param>
    /// <returns>Current builder.</returns>
    ITimeSpanDecompositionBuilder WithMaxUnit(TimeUnit maxUnit);

    /// <summary>
    /// Configures the decomposition to exclude zero-value units.
    /// </summary>
    /// <returns>Current builder.</returns>
    ITimeSpanDecompositionBuilder WithoutZeroUnits();

    /// <summary>
    /// Configures the decomposition to use a custom quantizer.
    /// </summary>
    /// <param name="quantizer">The quantizer to use.</param>
    /// <returns>Current builder.</returns>
    ITimeSpanDecompositionBuilder WithQuantizer(ITimeSpanQuantizer quantizer);

    /// <summary>
    /// Configures the decomposition to use custom unit rules.
    /// </summary>
    /// <param name="ruleEngine">The rule engine to use.</param>
    /// <returns>Current builder.</returns>
    ITimeSpanDecompositionBuilder WithUnitRules(ITimeUnitRuleEngine ruleEngine);

    /// <summary>
    /// Executes the decomposition and returns the resulting time unit values.
    /// </summary>
    /// <returns>A read-only list of time unit values.</returns>
    IReadOnlyList<TimeUnitValue> Decompose();
}
