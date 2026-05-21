// -----------------------------------------------------------------------
// <copyright file="ICultureService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;

namespace MyNet.Globalization.Culture;

/// <summary>
/// Defines a service for managing application culture settings, allowing retrieval of the current culture and changing it at runtime. This service also provides an event to notify subscribers when the culture changes.
/// Extends <see cref="ICultureContext"/> which provides read-only access to the current culture.
/// </summary>
public interface ICultureService : ICultureContext
{
    /// <summary>
    /// Occurs when the current culture changes.
    /// </summary>
    event EventHandler<CultureChangedEventArgs> CultureChanged;

    /// <summary>
    /// Changes the current culture.
    /// </summary>
    /// <param name="culture">Target culture.</param>
    void SetCulture(CultureInfo culture);

    /// <summary>
    /// Changes the current culture.
    /// </summary>
    /// <param name="cultureCode">Target culture code.</param>
    void SetCulture(string cultureCode);
}
