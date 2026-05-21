// -----------------------------------------------------------------------
// <copyright file="TimeLocalizationOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Humanizer.Temporal;

/// <summary>
/// Options for configuring time localization resource keys.
/// </summary>
public sealed class TimeLocalizationOptions
{
    /// <summary>
    /// Gets resource key for "now".
    /// </summary>
    public string NowKey { get; init; } = "RelativeDateNow";

    /// <summary>
    /// Gets resource key for "never".
    /// </summary>
    public string NeverKey { get; init; } = "RelativeDateNever";

    /// <summary>
    /// Gets resource key for zero duration.
    /// </summary>
    public string ZeroKey { get; init; } = "RelativeDateZero";

    /// <summary>
    /// Gets resource key for tomorrow.
    /// </summary>
    public string TomorrowKey { get; init; } = "RelativeDateTomorrow";

    /// <summary>
    /// Gets resource key for yesterday.
    /// </summary>
    public string YesterdayKey { get; init; } = "RelativeDateYesterday";

    /// <summary>
    /// Gets format for relative date resource keys.
    /// Example:
    /// RelativeDateFutureHour
    /// RelativeDatePastMinute.
    /// </summary>
    public string RelativeDateKeyFormat { get; init; } = "RelativeDate{0}{1}";

    /// <summary>
    /// Gets format for duration resource keys.
    /// Example:
    /// DurationHour
    /// DurationMinute.
    /// </summary>
    public string DurationKeyFormat { get; init; } = "Duration{0}";
}
