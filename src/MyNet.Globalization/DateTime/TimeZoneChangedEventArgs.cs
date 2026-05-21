// -----------------------------------------------------------------------
// <copyright file="TimeZoneChangedEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Globalization.DateTime;

/// <summary>
/// Event arguments for time zone change events, containing the old and new time zones.
/// </summary>
/// <param name="oldTimeZone">The previous time zone.</param>
/// <param name="newTimeZone">The new time zone.</param>
public sealed class TimeZoneChangedEventArgs(TimeZoneInfo oldTimeZone, TimeZoneInfo newTimeZone) : EventArgs
{
    /// <summary>
    /// Gets the previous time zone.
    /// </summary>
    public TimeZoneInfo OldTimeZone { get; } = oldTimeZone;

    /// <summary>
    /// Gets the new time zone.
    /// </summary>
    public TimeZoneInfo NewTimeZone { get; } = newTimeZone;
}
