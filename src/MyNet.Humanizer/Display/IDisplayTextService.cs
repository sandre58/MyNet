// -----------------------------------------------------------------------
// <copyright file="IDisplayTextService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Humanizer.Display;

/// <summary>
/// Service for retrieving display texts for values of various types, using registered display text strategies and culture-specific information.
/// </summary>
public interface IDisplayTextService
{
    /// <summary>
    /// Gets the display text for a given value of type T, using the appropriate display text strategy registered for that type and taking into account culture-specific information to determine the appropriate display text.
    /// </summary>
    /// <param name="value">The value for which to get the display text.</param>
    /// <param name="options">The options to use when generating the display text.</param>
    /// <param name="culture">The culture to use for determining the display text.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>The display text for the given value.</returns>
    string GetDisplayText<T>(T value, DisplayTextOptions options, CultureInfo? culture = null)
        where T : notnull;
}
