// -----------------------------------------------------------------------
// <copyright file="TimeSpanUnitSemantics.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Temporal.Decomposition;

/// <summary>
/// Defines how hierarchical units should be interpreted during decomposition.
/// </summary>
public enum TimeSpanUnitSemantics
{
    /// <summary>
    /// Uses fixed average durations for calendar units (month/year).
    /// <example>
    /// 1 month = ~30.44 days
    /// </example>
    /// </summary>
    FixedAverage,

    /// <summary>
    /// Uses strict calendar-based logic (requires a reference date).
    /// <example>
    /// January → February = 1 calendar month (28–31 days depending on year)
    /// </example>
    /// </summary>
    CalendarAware
}
