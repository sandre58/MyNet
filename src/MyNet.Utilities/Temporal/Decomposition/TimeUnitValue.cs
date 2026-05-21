// -----------------------------------------------------------------------
// <copyright file="TimeUnitValue.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Temporal.Decomposition;

/// <summary>
/// Represents a value of a specific time unit, used in time span decomposition to break down a time span into its constituent parts.
/// </summary>
/// <param name="Value">The value of the time unit.</param>
/// <param name="Unit">The time unit.</param>
/// <example>
/// new TimeUnitValue(3, TimeUnit.Minute) represents "3 minutes".
/// </example>
public readonly record struct TimeUnitValue(int Value, TimeUnit Unit);
