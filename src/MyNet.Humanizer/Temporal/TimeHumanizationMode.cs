// -----------------------------------------------------------------------
// <copyright file="TimeHumanizationMode.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Humanizer.Temporal;

/// <summary>
/// Defines the mode of humanization for time intervals.
/// <see cref="Relative"/> produces output like "2 minutes ago" or "in 3 hours".
/// <see cref="Duration"/> produces output like "2 minutes" or "3 hours".
/// </summary>
public enum TimeHumanizationMode
{
    /// <summary>
    /// Produces humanized time intervals in a relative format, indicating whether the time is in the past or future (e.g., "2 minutes ago", "in 3 hours").
    /// </summary>
    Relative,

    /// <summary>
    /// Produces humanized time intervals in a duration format, without indicating whether the time is in the past or future (e.g., "2 minutes", "3 hours").
    /// </summary>
    Duration
}
