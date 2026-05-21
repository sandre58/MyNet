// -----------------------------------------------------------------------
// <copyright file="TimeSpanDecompositionOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Temporal.Decomposition;

/// <summary>
/// Options for decomposing a TimeSpan into its component time units, allowing customization of precision, unit inclusion, and range of units considered.
/// </summary>
/// <example>
/// Strict mode:
/// new TimeSpanDecompositionOptions { }.
/// Fuzzy mode:
/// new TimeSpanDecompositionOptions { Quantizer = HumanFriendlyTimeSpanQuantizer.Current }.
/// </example>
public sealed class TimeSpanDecompositionOptions
{
    /// <summary>
    /// Gets the default options for decomposing a TimeSpan, which includes a precision of 1, does not count empty units, and considers time units from seconds to years.
    /// </summary>
    public static TimeSpanDecompositionOptions Default { get; } = TimeSpanDecompositionPresets.Strict;

    /// <summary>
    /// Gets the minimum time unit to consider in the decomposition.
    /// </summary>
    public TimeUnit MinUnit { get; init; } = TimeUnit.Millisecond;

    /// <summary>
    /// Gets the maximum time unit to consider in the decomposition.
    /// </summary>
    public TimeUnit MaxUnit { get; init; } = TimeUnit.Year;

    /// <summary>
    /// Gets the mode for decomposing the TimeSpan into its component time units. This determines how the decomposition will be performed, such as whether to use a hierarchical approach (where larger units are prioritized) or a flat approach (where all units are treated equally). The default mode is Hierarchical, which means that the decomposition will prioritize larger time units first before breaking down into smaller units.
    /// </summary>
    public TimeSpanDecompositionMode Mode { get; init; } = TimeSpanDecompositionMode.Hierarchical;

    /// <summary>
    /// Gets the precision for the decomposition, which determines how many time units to include in the result. For example, a precision of 2 would include only the two largest time units in the decomposition. If null, all non-zero time units will be included.
    /// </summary>
    public int? MaxComponents { get; init; }

    /// <summary>
    /// Gets a value indicating whether to include time units with zero values in the decomposition result. If true, all time units within the specified range (from MinUnit to MaxUnit) will be included in the result, even if their value is zero. If false, only time units with non-zero values will be included in the decomposition result. The default value is false, which means that by default, only non-zero time units will be included in the decomposition.
    /// </summary>
    public bool IncludeZeroUnits { get; init; }

    /// <summary>
    /// Gets the rule engine to use for decomposing the TimeSpan into its component time units. This allows for customization of how the decomposition is performed, such as defining specific rules for how to handle certain time units or how to round values. If not specified, a default rule engine will be used.
    /// </summary>
    public ITimeUnitRuleEngine RuleEngine { get; init; } = TimeUnitRuleSelector.Default;

    /// <summary>
    /// Gets the optional quantizer to apply after exact decomposition.
    /// </summary>
    public ITimeSpanQuantizer? Quantizer { get; init; }
}
