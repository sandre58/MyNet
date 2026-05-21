// -----------------------------------------------------------------------
// <copyright file="TimeHumanizationPresets.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Utilities;
using MyNet.Utilities.Temporal.Decomposition;

namespace MyNet.Humanizer.Temporal;

/// <summary>
/// Provides preset configurations for <see cref="TimeSpanDecompositionOptions"/>.
/// </summary>
public static class TimeHumanizationPresets
{
    /// <summary>
    /// Gets the default humanization options, which is the same as <see cref="Duration"/>.
    /// </summary>
    public static TimeHumanizationOptions Default { get; } = new()
    {
        Mode = TimeHumanizationMode.Duration,
        MaxComponents = 2,
        MinUnit = TimeUnit.Second,
        MaxUnit = TimeUnit.Year,
        DecompositionMode = TimeSpanDecompositionMode.Hierarchical
    };

    /// <summary>
    /// Gets human-readable duration with up to 2 components.
    /// Example:
    /// 1 minute 59 seconds.
    /// </summary>
    public static TimeHumanizationOptions Duration { get; } = new()
    {
        Mode = TimeHumanizationMode.Duration,
        MaxComponents = 2,
        MinUnit = TimeUnit.Second,
        MaxUnit = TimeUnit.Year,
        DecompositionMode = TimeSpanDecompositionMode.Hierarchical
    };

    /// <summary>
    /// Gets human-readable relative time with a single component.
    /// Example:
    /// 1 minute ago.
    /// </summary>
    public static TimeHumanizationOptions Relative { get; } = new()
    {
        Mode = TimeHumanizationMode.Relative,
        MaxComponents = 1,
        Quantizer = HumanFriendlyTimeSpanQuantizer.Default
    };

    /// <summary>
    /// Gets exact human-readable decomposition.
    /// Example:
    /// 1 minute 59 seconds.
    /// </summary>
    public static TimeHumanizationOptions Precise { get; } = new()
    {
        Mode = TimeHumanizationMode.Duration,
        MaxComponents = null,
        IncludeZeroUnits = false
    };

    /// <summary>
    /// Gets human-friendly approximation.
    /// Example:
    /// 59 seconds -> 1 minute.
    /// </summary>
    public static TimeHumanizationOptions Fuzzy { get; } = new()
    {
        Mode = TimeHumanizationMode.Relative,
        MaxComponents = 1,
        Quantizer = HumanFriendlyTimeSpanQuantizer.Default
    };

    /// <summary>
    /// Gets compact UI-oriented representation.
    /// Example:
    /// 2h.
    /// </summary>
    public static TimeHumanizationOptions Compact { get; } = new()
    {
        Mode = TimeHumanizationMode.Duration,
        MaxComponents = 1
    };
}
