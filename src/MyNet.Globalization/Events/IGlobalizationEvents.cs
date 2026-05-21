// -----------------------------------------------------------------------
// <copyright file="IGlobalizationEvents.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Globalization.Culture;
using MyNet.Globalization.DateTime;

namespace MyNet.Globalization.Events;

/// <summary>
/// Interface for globalization-related events, such as culture and time zone changes.
/// </summary>
public interface IGlobalizationEvents
{
    /// <summary>
    /// Event triggered when the application's culture changes, allowing subscribers to respond to culture updates.
    /// </summary>
    event EventHandler<CultureChangedEventArgs> CultureChanged;

    /// <summary>
    /// Event triggered when the application's time zone changes, allowing subscribers to respond to time zone updates.
    /// </summary>
    event EventHandler<TimeZoneChangedEventArgs> TimeZoneChanged;
}
