// -----------------------------------------------------------------------
// <copyright file="ITimeHumanizer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.Globalization.Localization.Providers;
using MyNet.Humanizer.Formatting.Collections;
using MyNet.Primitives;
using MyNet.Temporal.Decomposition;

namespace MyNet.Humanizer.Temporal;

/// <summary>
/// Can humanize time spans and relative times.
/// </summary>
public interface ITimeHumanizer : ICultureScoped
{
    /// <summary>
    /// Humanizes a relative time, such as "2 days ago" or "in 3 hours".
    /// </summary>
    /// <param name="count">The number of time units.</param>
    /// <param name="unit">The unit of time.</param>
    /// <param name="tense">The tense of the time expression.</param>
    /// <returns>A human-readable string representing the relative time.</returns>
    string HumanizeRelativeTime(int count, TimeUnit unit, Tense tense);

    /// <summary>
    /// Humanizes a relative time represented as a collection of time unit values, such as "1 year, 2 months, and 3 days ago" or "in 1 year, 2 months, and 3 days".
    /// </summary>
    /// <param name="values">The time unit values.</param>
    /// <param name="tense">The tense of the time expression.</param>
    /// <returns>A human-readable string representing the relative time.</returns>
    string HumanizeRelativeTime(IReadOnlyList<TimeUnitValue> values, Tense tense);

    /// <summary>
    /// Humanizes a duration, such as "2 days" or "3 hours".
    /// </summary>
    /// <param name="count">The number of time units.</param>
    /// <param name="unit">The unit of time.</param>
    /// <returns>A human-readable string representing the duration.</returns>
    string HumanizeDuration(int count, TimeUnit unit);

    /// <summary>
    /// Humanizes a duration represented as a collection of time unit values, such as "1 year, 2 months, and 3 days".
    /// </summary>
    /// <param name="values">The time unit values.</param>
    /// <param name="listFormattingOptions">Optional list formatting options used to join multiple parts.</param>
    /// <returns>A human-readable string representing the duration.</returns>
    string HumanizeDuration(IReadOnlyList<TimeUnitValue> values, ListFormattingOptions? listFormattingOptions = null);
}
