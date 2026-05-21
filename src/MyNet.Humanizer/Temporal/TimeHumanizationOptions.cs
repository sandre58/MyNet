// -----------------------------------------------------------------------
// <copyright file="TimeHumanizationOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Humanizer.Formatting.Collections;
using MyNet.Utilities;
using MyNet.Utilities.Temporal.Decomposition;

namespace MyNet.Humanizer.Temporal;

/// <summary>
/// Options for humanizing time intervals.
/// </summary>
public sealed class TimeHumanizationOptions
{
    /// <summary>
    /// Gets or initializes the mode of humanization to use. Current is <see cref="TimeHumanizationMode.Relative"/>.
    /// </summary>
    public TimeHumanizationMode Mode { get; init; } = TimeHumanizationMode.Duration;

    /// <summary>
    /// Gets or initializes the list formatting options to use when humanizing durations. This property is only relevant when the Mode is set to Duration. If null, default list formatting will be used.
    /// </summary>
    public Tense? Tense { get; init; }

    /// <summary>
    /// Gets the minimum time unit to consider in the decomposition.
    /// </summary>
    public TimeUnit MinUnit { get; init; } = TimeUnit.Second;

    /// <summary>
    /// Gets the maximum time unit to consider in the decomposition.
    /// </summary>
    public TimeUnit MaxUnit { get; init; } = TimeUnit.Year;

    /// <summary>
    /// Gets the mode for decomposing the TimeSpan into its component time units. This determines how the decomposition will be performed, such as whether to use a hierarchical approach (where larger units are prioritized) or a flat approach (where all units are treated equally). The default mode is Hierarchical, which means that the decomposition will prioritize larger time units first before breaking down into smaller units.
    /// </summary>
    public TimeSpanDecompositionMode DecompositionMode { get; init; } = TimeSpanDecompositionMode.Hierarchical;

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

    /// <summary>
    /// Gets the list formatting options to use when humanizing durations. This property is only relevant when the Mode is set to Duration. If null, default list formatting will be used.
    /// </summary>
    public ListFormattingOptions ListFormatting { get; init; } = ListFormattingOptionsPresets.CommaSeparated;
}
