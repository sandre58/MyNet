// -----------------------------------------------------------------------
// <copyright file="TimeConverter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

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
        TimeUnit.Week => 7 * 24d * 60d * 60d * 1000d, // DaysPerWeek
        TimeUnit.Month => (365.2425 / 12) * 24d * 60d * 60d * 1000d, // Gregorian avg days/month (DateTimeHelper.DaysPerMonth)
        TimeUnit.Year => 365.2425 * 24d * 60d * 60d * 1000d, // Gregorian avg days/year (DateTimeHelper.DaysPerYear)
        _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unsupported time unit.")
    };
}
