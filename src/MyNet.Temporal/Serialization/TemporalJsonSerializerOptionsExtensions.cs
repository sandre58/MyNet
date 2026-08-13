// -----------------------------------------------------------------------
// <copyright file="TemporalJsonSerializerOptionsExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text.Json;

namespace MyNet.Temporal.Serialization;

/// <summary>
/// Registers System.Text.Json converters for core temporal value types.
/// </summary>
public static class TemporalJsonSerializerOptionsExtensions
{
    /// <summary>
    /// Adds converters for <see cref="Intervals.DateTimePeriod"/>, <see cref="Intervals.DateTimeOffsetPeriod"/>,
    /// and <see cref="CalendarAmount"/>.
    /// </summary>
    public static JsonSerializerOptions AddMyNetTemporalConverters(this JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Converters.Add(new DateTimePeriodJsonConverter());
        options.Converters.Add(new DateTimeOffsetPeriodJsonConverter());
        options.Converters.Add(new CalendarAmountJsonConverter());
        return options;
    }
}
