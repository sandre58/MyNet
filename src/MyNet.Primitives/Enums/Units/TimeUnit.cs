// -----------------------------------------------------------------------
// <copyright file="TimeUnit.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Primitives;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Specifies a time unit for conversion purposes. This enum can be used to represent various time intervals such as milliseconds, seconds, minutes, hours, days, weeks, months, and years.
/// </summary>
public enum TimeUnit
{
    Millisecond,
    Second,
    Minute,
    Hour,
    Day,
    Week,
    Month,
    Year
}
