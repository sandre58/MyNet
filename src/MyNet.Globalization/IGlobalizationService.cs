// -----------------------------------------------------------------------
// <copyright file="IGlobalizationService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Globalization.Culture;
using MyNet.Globalization.DateTime;

namespace MyNet.Globalization;

/// <summary>
/// Defines a service that provides globalization-related functionalities, including culture and time zone management.
/// </summary>
public interface IGlobalizationService : ICultureService, ITimeZoneService
{
    /// <summary>
    /// Temporarily changes the current culture for the scope of the returned <see cref="IDisposable"/>. When the returned object is disposed, the culture reverts to its previous value.
    /// </summary>
    /// <param name="culture">The culture to set temporarily.</param>
    /// <returns>An <see cref="IDisposable"/> that, when disposed, restores the previous culture.</returns>
    IDisposable UseCulture(CultureInfo culture);

    /// <summary>
    /// Temporarily changes the current time zone for the scope of the returned <see cref="IDisposable"/>. When the returned object is disposed, the time zone reverts to its previous value.
    /// </summary>
    /// <param name="timeZone">The time zone to set temporarily.</param>
    /// <returns>An <see cref="IDisposable"/> that, when disposed, restores the previous time zone.</returns>
    IDisposable UseTimeZone(TimeZoneInfo timeZone);
}
