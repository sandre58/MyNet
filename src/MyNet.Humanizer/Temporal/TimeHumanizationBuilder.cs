// -----------------------------------------------------------------------
// <copyright file="TimeHumanizationBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Humanizer.Formatting.Collections;
using MyNet.Utilities;
using MyNet.Utilities.Temporal.Decomposition;

namespace MyNet.Humanizer.Temporal;

/// <summary>
/// Fluent builder used to configure <see cref="TimeHumanizationOptions"/>.
/// </summary>
public sealed class TimeHumanizationBuilder
{
    private TimeHumanizationMode _mode = TimeHumanizationMode.Duration;
    private TimeUnit _minUnit = TimeUnit.Second;
    private TimeUnit _maxUnit = TimeUnit.Year;
    private int? _maxComponents = 2;
    private bool _includeZeroUnits;
    private TimeSpanDecompositionMode _decompositionMode = TimeSpanDecompositionMode.Hierarchical;
    private ITimeSpanQuantizer? _quantizer;
    private Tense? _tense;
    private ListFormattingOptions _listFormatting = ListFormattingOptionsPresets.CommaSeparated;

    /// <summary>
    /// Configures the builder from an existing preset or options instance.
    /// </summary>
    /// <param name="options">The source options.</param>
    /// <returns>The current builder.</returns>
    public TimeHumanizationBuilder From(TimeHumanizationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _mode = options.Mode;
        _minUnit = options.MinUnit;
        _maxUnit = options.MaxUnit;
        _maxComponents = options.MaxComponents;
        _includeZeroUnits = options.IncludeZeroUnits;
        _decompositionMode = options.DecompositionMode;
        _quantizer = options.Quantizer;
        _tense = options.Tense;
        _listFormatting = options.ListFormatting;

        return this;
    }

    /// <summary>
    /// Uses duration humanization.
    /// </summary>
    /// <returns>The current builder.</returns>
    public TimeHumanizationBuilder AsDuration()
    {
        _mode = TimeHumanizationMode.Duration;
        return this;
    }

    /// <summary>
    /// Uses relative-time humanization.
    /// </summary>
    /// <returns>The current builder.</returns>
    public TimeHumanizationBuilder AsRelative()
    {
        _mode = TimeHumanizationMode.Relative;
        return this;
    }

    /// <summary>
    /// Uses the specified tense for relative humanization.
    /// </summary>
    /// <param name="tense">The tense to use.</param>
    /// <returns>The current builder.</returns>
    public TimeHumanizationBuilder WithTense(Tense tense)
    {
        _tense = tense;
        return this;
    }

    /// <summary>
    /// Uses the specified minimum unit.
    /// </summary>
    /// <param name="unit">The minimum unit.</param>
    /// <returns>The current builder.</returns>
    public TimeHumanizationBuilder MinUnit(TimeUnit unit)
    {
        _minUnit = unit;
        return this;
    }

    /// <summary>
    /// Uses the specified maximum unit.
    /// </summary>
    /// <param name="unit">The maximum unit.</param>
    /// <returns>The current builder.</returns>
    public TimeHumanizationBuilder MaxUnit(TimeUnit unit)
    {
        _maxUnit = unit;
        return this;
    }

    /// <summary>
    /// Uses the specified unit range.
    /// </summary>
    /// <param name="minUnit">The minimum unit.</param>
    /// <param name="maxUnit">The maximum unit.</param>
    /// <returns>The current builder.</returns>
    public TimeHumanizationBuilder UseUnits(TimeUnit minUnit, TimeUnit maxUnit)
    {
        _minUnit = minUnit;
        _maxUnit = maxUnit;

        return this;
    }

    /// <summary>
    /// Limits the number of displayed components.
    /// </summary>
    /// <param name="count">The maximum number of components.</param>
    /// <returns>The current builder.</returns>
    public TimeHumanizationBuilder MaxComponents(int? count)
    {
        _maxComponents = count;
        return this;
    }

    /// <summary>
    /// Includes zero-value units.
    /// </summary>
    /// <returns>The current builder.</returns>
    public TimeHumanizationBuilder IncludeZeroUnits()
    {
        _includeZeroUnits = true;
        return this;
    }

    /// <summary>
    /// Excludes zero-value units.
    /// </summary>
    /// <returns>The current builder.</returns>
    public TimeHumanizationBuilder ExcludeZeroUnits()
    {
        _includeZeroUnits = false;
        return this;
    }

    /// <summary>
    /// Uses hierarchical decomposition.
    /// </summary>
    /// <returns>The current builder.</returns>
    public TimeHumanizationBuilder Hierarchical()
    {
        _decompositionMode = TimeSpanDecompositionMode.Hierarchical;
        return this;
    }

    /// <summary>
    /// Uses largest-unit-only decomposition.
    /// </summary>
    /// <returns>The current builder.</returns>
    public TimeHumanizationBuilder LargestUnitOnly()
    {
        _decompositionMode = TimeSpanDecompositionMode.LargestUnitOnly;
        return this;
    }

    /// <summary>
    /// Uses smallest-unit-only decomposition.
    /// </summary>
    /// <returns>The current builder.</returns>
    public TimeHumanizationBuilder SmallestUnitOnly()
    {
        _decompositionMode = TimeSpanDecompositionMode.SmallestUnitOnly;
        return this;
    }

    /// <summary>
    /// Uses fuzzy humanization.
    /// </summary>
    /// <returns>The current builder.</returns>
    public TimeHumanizationBuilder Fuzzy()
    {
        _quantizer = HumanFriendlyTimeSpanQuantizer.Default;
        return this;
    }

    /// <summary>
    /// Uses precise humanization.
    /// </summary>
    /// <returns>The current builder.</returns>
    public TimeHumanizationBuilder Precise()
    {
        _quantizer = null;
        return this;
    }

    /// <summary>
    /// Uses the specified quantizer.
    /// </summary>
    /// <param name="quantizer">The quantizer.</param>
    /// <returns>The current builder.</returns>
    public TimeHumanizationBuilder WithQuantizer(ITimeSpanQuantizer? quantizer)
    {
        _quantizer = quantizer;
        return this;
    }

    /// <summary>
    /// Uses the specified list formatting options.
    /// </summary>
    /// <param name="options">The list formatting options.</param>
    /// <returns>The current builder.</returns>
    public TimeHumanizationBuilder WithListFormatting(ListFormattingOptions options)
    {
        _listFormatting = options;
        return this;
    }

    /// <summary>
    /// Builds the configured <see cref="TimeHumanizationOptions"/>.
    /// </summary>
    /// <returns>The built options.</returns>
    public TimeHumanizationOptions Build() => new()
    {
        Mode = _mode,
        MinUnit = _minUnit,
        MaxUnit = _maxUnit,
        MaxComponents = _maxComponents,
        IncludeZeroUnits = _includeZeroUnits,
        DecompositionMode = _decompositionMode,
        Quantizer = _quantizer,
        Tense = _tense,
        ListFormatting = _listFormatting
    };
}
