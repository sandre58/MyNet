// -----------------------------------------------------------------------
// <copyright file="TimeSpanDecompositionBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.Utilities.Temporal.Decomposition;

/// <summary>
/// Builder for configuring and performing decomposition of a TimeSpan into its components based on specified options.
/// </summary>
/// <param name="decomposer">The decomposer to use for the decomposition.</param>
/// <param name="timeSpan">The TimeSpan to decompose.</param>
public sealed class TimeSpanDecompositionBuilder(ITimeSpanDecomposer decomposer, TimeSpan timeSpan) : ITimeSpanDecompositionBuilder
{
    private TimeSpanDecompositionMode _mode = TimeSpanDecompositionMode.Hierarchical;
    private int? _maxComponents;
    private bool _removeZeros;
    private ITimeSpanQuantizer? _quantizer;
    private ITimeUnitRuleEngine? _ruleEngine;
    private TimeUnit _minUnit = TimeUnit.Millisecond;
    private TimeUnit _maxUnit = TimeUnit.Year;

    /// <inheritdoc/>
    public ITimeSpanDecompositionBuilder AsHierarchical()
    {
        _mode = TimeSpanDecompositionMode.Hierarchical;
        return this;
    }

    /// <inheritdoc/>
    public ITimeSpanDecompositionBuilder AsFlat()
    {
        _mode = TimeSpanDecompositionMode.Flat;
        return this;
    }

    /// <inheritdoc/>
    public ITimeSpanDecompositionBuilder UseLargestUnitOnly()
    {
        _mode = TimeSpanDecompositionMode.LargestUnitOnly;
        return this;
    }

    /// <inheritdoc/>
    public ITimeSpanDecompositionBuilder UseSmallestUnitOnly()
    {
        _mode = TimeSpanDecompositionMode.SmallestUnitOnly;
        return this;
    }

    /// <inheritdoc/>
    public ITimeSpanDecompositionBuilder MaxComponents(int count)
    {
        _maxComponents = count;
        return this;
    }

    /// <inheritdoc/>
    public ITimeSpanDecompositionBuilder WithMinUnit(TimeUnit minUnit)
    {
        _minUnit = minUnit;
        return this;
    }

    /// <inheritdoc/>
    public ITimeSpanDecompositionBuilder WithMaxUnit(TimeUnit maxUnit)
    {
        _maxUnit = maxUnit;
        return this;
    }

    /// <inheritdoc/>
    public ITimeSpanDecompositionBuilder WithoutZeroUnits()
    {
        _removeZeros = true;
        return this;
    }

    /// <inheritdoc/>
    public ITimeSpanDecompositionBuilder WithQuantizer(ITimeSpanQuantizer quantizer)
    {
        _quantizer = quantizer;
        return this;
    }

    /// <inheritdoc/>
    public ITimeSpanDecompositionBuilder WithUnitRules(ITimeUnitRuleEngine ruleEngine)
    {
        _ruleEngine = ruleEngine;
        return this;
    }

    /// <inheritdoc/>
    public IReadOnlyList<TimeUnitValue> Decompose()
    {
        var options = new TimeSpanDecompositionOptions
        {
            RuleEngine = _ruleEngine ?? TimeUnitRuleSelector.Default,
            MinUnit = _minUnit,
            MaxUnit = _maxUnit,
            Quantizer = _quantizer,
            IncludeZeroUnits = !_removeZeros,
            MaxComponents = _maxComponents,
            Mode = _mode
        };

        return decomposer.Decompose(timeSpan, options);
    }
}
