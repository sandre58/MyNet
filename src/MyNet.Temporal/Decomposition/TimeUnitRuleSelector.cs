// -----------------------------------------------------------------------
// <copyright file="TimeUnitRuleSelector.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Primitives;
using MyNet.Primitives.Temporal;

namespace MyNet.Temporal.Decomposition;

/// <summary>
/// Selects time units for decomposing a TimeSpan based on its magnitude and specified minimum and maximum units.
/// </summary>
/// <remarks>
/// Strict behavior: this selector chooses units only, without any fuzzy approximation.
/// </remarks>
public sealed class TimeUnitRuleSelector : ITimeUnitRuleEngine
{
    /// <summary>
    /// Gets a new instance of the <see cref="TimeUnitRuleSelector"/> class.
    /// </summary>
    public static TimeUnitRuleSelector Default { get; } = new();

    /// <summary>
    /// Selects the TimeUnits to use for a given TimeSpan, based on the provided minimum and maximum TimeUnits.
    /// </summary>
    /// <param name="span">The TimeSpan to decompose.</param>
    /// <param name="minUnit">The minimum TimeUnit to include in the decomposition.</param>
    /// <param name="maxUnit">The maximum TimeUnit to include in the decomposition.</param>
    /// <returns>A list of TimeUnits to use for the decomposition.</returns>
    /// <example>
    /// 63 minutes with min=Minute and max=Hour returns [Hour, Minute].
    /// </example>
    public IReadOnlyList<TimeUnit> Select(TimeSpan span, TimeUnit minUnit, TimeUnit maxUnit)
    {
        if (minUnit > maxUnit)
            throw new ArgumentException($"{nameof(minUnit)} cannot be greater than {nameof(maxUnit)}.");

        var absolute = span.Duration();
        var largest = minUnit;

        for (var unit = maxUnit; unit >= minUnit; unit--)
        {
            if (GetTotal(absolute, unit) >= 1d)
            {
                largest = unit;
                break;
            }
        }

        var result = new List<TimeUnit>();
        for (var unit = largest; unit >= minUnit; unit--)
        {
            if (largest >= TimeUnit.Month && unit == TimeUnit.Week)
                continue;

            result.Add(unit);
        }

        return result;
    }

    /// <summary>
    /// Calculates the total amount of the specified TimeUnit in the given TimeSpan.
    /// </summary>
    /// <param name="span">The TimeSpan to evaluate.</param>
    /// <param name="unit">The TimeUnit to calculate the total for.</param>
    /// <returns>The total amount of the specified TimeUnit in the given TimeSpan.</returns>
    private static double GetTotal(TimeSpan span, TimeUnit unit) => unit switch
    {
        TimeUnit.Year => span.TotalDays / DateTimeHelper.DaysPerYear,
        TimeUnit.Month => span.TotalDays / DateTimeHelper.DaysPerMonth,
        TimeUnit.Week => span.TotalDays / DateTimeHelper.DaysPerWeek,
        TimeUnit.Day => span.TotalDays,
        TimeUnit.Hour => span.TotalHours,
        TimeUnit.Minute => span.TotalMinutes,
        TimeUnit.Second => span.TotalSeconds,
        _ => span.TotalMilliseconds
    };
}
