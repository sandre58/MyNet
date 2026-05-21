// -----------------------------------------------------------------------
// <copyright file="GlobalizationEvents.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Globalization.Culture;
using MyNet.Globalization.DateTime;

namespace MyNet.Globalization.Events;

/// <summary>
/// Implements the <see cref="IGlobalizationEvents"/> interface and provides events for culture and time zone changes.
/// </summary>
public sealed class GlobalizationEvents : IGlobalizationEvents
{
    private readonly ICultureService _cultureService;
    private readonly ITimeZoneService _timeZoneService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalizationEvents"/> class with the specified culture and time zone services.
    /// </summary>
    /// <param name="cultureService">The culture service to use for culture-related events.</param>
    /// <param name="timeZoneService">The time zone service to use for time zone-related events.</param>
    public GlobalizationEvents(ICultureService cultureService, ITimeZoneService timeZoneService)
    {
        _cultureService = cultureService;
        _timeZoneService = timeZoneService;

        _cultureService.CultureChanged += (s, e) => CultureChanged?.Invoke(s, e);
        _timeZoneService.TimeZoneChanged += (s, e) => TimeZoneChanged?.Invoke(s, e);
    }

    /// <summary>
    /// Occurs when the culture changes. Subscribers can handle this event to respond to culture changes in the application.
    /// </summary>
    public event EventHandler<CultureChangedEventArgs>? CultureChanged;

    /// <summary>
    /// Occurs when the time zone changes. Subscribers can handle this event to respond to time zone changes in the application.
    /// </summary>
    public event EventHandler<TimeZoneChangedEventArgs>? TimeZoneChanged;
}
