// -----------------------------------------------------------------------
// <copyright file="TimeConverter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Primitives.Temporal;

namespace MyNet.Primitives.Conversion;

/// <summary>
/// Provides conversion between time units using shared calendar constants for week, month and year.
/// </summary>
public sealed class TimeConverter : IUnitConverter<TimeUnit>
{
    public double Convert(double value, TimeUnit from, TimeUnit to)
    {
        var fromFactor = GetMillisecondsFactor(from);
        var toFactor = GetMillisecondsFactor(to);

        return value * fromFactor / toFactor;
    }

    private static double GetMillisecondsFactor(TimeUnit unit) => unit switch
    {
        TimeUnit.Millisecond => 1d,
        TimeUnit.Second => 1000d,
        TimeUnit.Minute => 60d * 1000d,
        TimeUnit.Hour => 60d * 60d * 1000d,
        TimeUnit.Day => 24d * 60d * 60d * 1000d,
        TimeUnit.Week => DateTimeHelper.DaysPerWeek * 24d * 60d * 60d * 1000d,
        TimeUnit.Month => DateTimeHelper.DaysPerMonth * 24d * 60d * 60d * 1000d,
        TimeUnit.Year => DateTimeHelper.DaysPerYear * 24d * 60d * 60d * 1000d,
        _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unsupported time unit.")
    };
}
