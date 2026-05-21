// -----------------------------------------------------------------------
// <copyright file="TimeSpanDecompositionPresets.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Temporal.Decomposition;

public static class TimeSpanDecompositionPresets
{
    /// <summary>
    /// Gets strict decomposition: precise, no noise, no zero units.
    /// Best for logs, debugging, and technical output.
    /// </summary>
    public static TimeSpanDecompositionOptions Strict { get; } = new()
    {
        MinUnit = TimeUnit.Millisecond,
        MaxUnit = TimeUnit.Year,
        Mode = TimeSpanDecompositionMode.Hierarchical,
        MaxComponents = null,
        IncludeZeroUnits = false,
        RuleEngine = TimeUnitRuleSelector.Default,
        Quantizer = null
    };

    /// <summary>
    /// Gets human-friendly decomposition optimized for UI display.
    /// Limits output size and removes empty units.
    /// </summary>
    public static TimeSpanDecompositionOptions Humanized { get; } = new()
    {
        MinUnit = TimeUnit.Second,
        MaxUnit = TimeUnit.Year,
        Mode = TimeSpanDecompositionMode.Hierarchical,
        MaxComponents = 2,
        IncludeZeroUnits = false,
        RuleEngine = TimeUnitRuleSelector.Default,
        Quantizer = null
    };

    /// <summary>
    /// Gets compact representation: best effort single or few units.
    /// Example: 2 weeks instead of 14 days 0 hours.
    /// </summary>
    public static TimeSpanDecompositionOptions Compact { get; } = new()
    {
        MinUnit = TimeUnit.Minute,
        MaxUnit = TimeUnit.Year,
        Mode = TimeSpanDecompositionMode.LargestUnitOnly,
        MaxComponents = 1,
        IncludeZeroUnits = false,
        RuleEngine = TimeUnitRuleSelector.Default,
        Quantizer = null
    };

    /// <summary>
    /// Gets full breakdown: keeps all units, including zeros.
    /// Useful for debugging or UI grids.
    /// </summary>
    public static TimeSpanDecompositionOptions Full { get; } = new()
    {
        MinUnit = TimeUnit.Millisecond,
        MaxUnit = TimeUnit.Year,
        Mode = TimeSpanDecompositionMode.Hierarchical,
        MaxComponents = null,
        IncludeZeroUnits = true,
        RuleEngine = TimeUnitRuleSelector.Default,
        Quantizer = null
    };

    /// <summary>
    /// Gets smallest unit representation: flattens into the smallest unit.
    /// Useful for comparisons, sorting, or storage.
    /// </summary>
    public static TimeSpanDecompositionOptions SmallestUnit { get; } = new()
    {
        MinUnit = TimeUnit.Millisecond,
        MaxUnit = TimeUnit.Year,
        Mode = TimeSpanDecompositionMode.SmallestUnitOnly,
        MaxComponents = 1,
        IncludeZeroUnits = false,
        RuleEngine = TimeUnitRuleSelector.Default,
        Quantizer = null
    };

    /// <summary>
    /// Gets largest unit only representation.
    /// Example: 2 weeks instead of detailed breakdown.
    /// </summary>
    public static TimeSpanDecompositionOptions LargestUnit { get; } = new()
    {
        MinUnit = TimeUnit.Millisecond,
        MaxUnit = TimeUnit.Year,
        Mode = TimeSpanDecompositionMode.LargestUnitOnly,
        MaxComponents = 1,
        IncludeZeroUnits = false,
        RuleEngine = TimeUnitRuleSelector.Default,
        Quantizer = null
    };
}
