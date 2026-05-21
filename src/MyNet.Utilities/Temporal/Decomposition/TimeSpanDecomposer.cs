// -----------------------------------------------------------------------
// <copyright file="TimeSpanDecomposer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNet.Utilities.Temporal.Decomposition;

/// <summary>
/// Decomposes a <see cref="TimeSpan"/> into ordered unit values using strict arithmetic.
/// </summary>
public sealed class TimeSpanDecomposer : ITimeSpanDecomposer
{
    /// <summary>
    /// Gets the default decomposer instance.
    /// </summary>
    public static TimeSpanDecomposer Default { get; } = new();

    /// <inheritdoc />
    public IReadOnlyList<TimeUnitValue> Decompose(TimeSpan timeSpan, TimeSpanDecompositionOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.RuleEngine);

        var ticks = GetAbsoluteTicks(timeSpan);

        var units = options.RuleEngine
            .Select(timeSpan, options.MinUnit, options.MaxUnit)
            .Distinct()
            .OrderByDescending(GetUnitTicks)
            .ToList();

        if (units.Count == 0)
            return [new(0, options.MinUnit)];

        var result = options.Mode switch
        {
            TimeSpanDecompositionMode.Hierarchical => DecomposeHierarchical(ticks, units),
            TimeSpanDecompositionMode.Flat => DecomposeFlat(ticks, units),
            TimeSpanDecompositionMode.LargestUnitOnly => DecomposeLargestOnly(ticks, units),
            TimeSpanDecompositionMode.SmallestUnitOnly => DecomposeSmallestOnly(ticks, units),
            _ => DecomposeHierarchical(ticks, units)
        };

        result = ApplyPostProcessing(result, options);

        return result.Count == 0 ? [new(0, options.MinUnit)] : result;
    }

    // --------------------------------------------------
    // Core decomposition strategies
    // --------------------------------------------------

    /// <summary>
    /// Decomposes the given ticks into unit values for all specified units using a hierarchical subtraction approach. Each unit's value is calculated based on the remaining ticks after accounting for larger units, ensuring that the resulting unit values reflect a proper hierarchical decomposition of the timespan. This strategy is suitable for scenarios where you want to represent the timespan in a way that respects the natural hierarchy of time units (e.g., 1 hour and 30 minutes instead of 90 minutes).
    /// </summary>
    /// <param name="ticks">The number of ticks to decompose.</param>
    /// <param name="units">The list of time units to consider.</param>
    /// <returns>A list of TimeUnitValue representing the timespan in each specified unit.</returns>
    private static List<TimeUnitValue> DecomposeHierarchical(long ticks, List<TimeUnit> units)
    {
        var remaining = ticks;
        var result = new List<TimeUnitValue>(units.Count);

        foreach (var unit in units)
        {
            var unitTicks = GetUnitTicks(unit);
            if (unitTicks <= 0)
                continue;

            var value = (int)(remaining / unitTicks);
            result.Add(new(value, unit));

            remaining -= value * unitTicks;
            if (remaining <= 0)
                break;
        }

        return result;
    }

    /// <summary>
    /// Decomposes the given ticks into unit values for all specified units without performing hierarchical subtraction. Each unit's value is calculated independently based on the total ticks, which may result in overlapping values across units. This strategy is useful for scenarios where you want to represent the timespan in multiple units simultaneously without enforcing a strict hierarchy.
    /// </summary>
    /// <param name="ticks">The number of ticks to decompose.</param>
    /// <param name="units">The list of time units to consider.</param>
    /// <returns>A list of TimeUnitValue representing the timespan in each specified unit.</returns>
    private static List<TimeUnitValue> DecomposeFlat(long ticks, List<TimeUnit> units)
    {
        var result = new List<TimeUnitValue>(units.Count);
        result.AddRange(from unit in units let unitTicks = GetUnitTicks(unit) where unitTicks > 0 let value = (int)(ticks / unitTicks) select new TimeUnitValue(value, unit));

        return result;
    }

    /// <summary>
    /// Decomposes the given ticks into a single unit value based on the largest unit in the provided list. This strategy effectively treats the entire timespan as a quantity of the largest unit, without performing hierarchical decomposition.
    /// </summary>
    /// <param name="ticks">The number of ticks to decompose.</param>
    /// <param name="units">The list of time units to consider.</param>
    /// <returns>A list containing a single TimeUnitValue representing the entire timespan in the largest unit.</returns>
    private static List<TimeUnitValue> DecomposeLargestOnly(long ticks, List<TimeUnit> units)
    {
        var unit = units[0];
        var value = (int)(ticks / GetUnitTicks(unit));

        return [new(value, unit)];
    }

    /// <summary>
    /// Decomposes the given ticks into a single unit value based on the smallest unit in the provided list. This strategy effectively treats the entire timespan as a quantity of the smallest unit, without performing hierarchical decomposition.
    /// </summary>
    /// <param name="ticks">The number of ticks to decompose.</param>
    /// <param name="units">The list of time units to consider.</param>
    /// <returns>A list containing a single TimeUnitValue representing the entire timespan in the smallest unit.</returns>
    private static List<TimeUnitValue> DecomposeSmallestOnly(long ticks, List<TimeUnit> units)
    {
        var unit = units[^1];
        var value = (int)(ticks / GetUnitTicks(unit));

        return [new(value, unit)];
    }

    /// <summary>
    /// Applies post-processing steps such as quantization, zero-unit filtering, and component limiting to the decomposed unit values based on the provided options.
    /// </summary>
    /// <param name="input">The list of decomposed time unit values.</param>
    /// <param name="options">The decomposition options to apply.</param>
    /// <returns>The post-processed list of time unit values.</returns>
    private static List<TimeUnitValue> ApplyPostProcessing(List<TimeUnitValue> input, TimeSpanDecompositionOptions options)
    {
        var result = input;

        if (options.Quantizer is not null)
            result = [.. options.Quantizer.Quantize(result)];

        if (!options.IncludeZeroUnits)
            result = [.. result.Where(x => x.Value != 0)];

        if (options.MaxComponents is > 0)
            result = [.. result.Take(options.MaxComponents.Value)];

        return result;
    }

    // --------------------------------------------------
    // Helpers
    // --------------------------------------------------

    /// <summary>
    /// Gets the absolute value of ticks, treating long.MinValue as long.MaxValue to avoid overflow issues.
    /// </summary>
    /// <param name="timeSpan">The TimeSpan to get the absolute ticks from.</param>
    /// <returns>The absolute value of ticks.</returns>
    private static long GetAbsoluteTicks(TimeSpan timeSpan) => timeSpan.Ticks == long.MinValue ? long.MaxValue : Math.Abs(timeSpan.Ticks);

    /// <summary>
    /// Gets the number of ticks corresponding to a given time unit, using fixed average values for variable-length units.
    /// </summary>
    /// <param name="unit">The time unit.</param>
    /// <returns>The number of ticks for the specified time unit.</returns>
    private static long GetUnitTicks(TimeUnit unit)
        => unit switch
        {
            TimeUnit.Year => (long)(DateTimeHelper.DaysPerYear * TimeSpan.TicksPerDay),
            TimeUnit.Month => (long)(DateTimeHelper.DaysPerMonth * TimeSpan.TicksPerDay),
            TimeUnit.Week => TimeSpan.TicksPerDay * DateTimeHelper.DaysPerWeek,
            TimeUnit.Day => TimeSpan.TicksPerDay,
            TimeUnit.Hour => TimeSpan.TicksPerHour,
            TimeUnit.Minute => TimeSpan.TicksPerMinute,
            TimeUnit.Second => TimeSpan.TicksPerSecond,
            TimeUnit.Millisecond => TimeSpan.TicksPerMillisecond,
            _ => 1L
        };
}
