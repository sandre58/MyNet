// -----------------------------------------------------------------------
// <copyright file="IDisplayTextStrategy.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Humanizer.Display;

/// <summary>
/// Provides display texts for values of type T, potentially taking into account culture-specific information.
/// </summary>
/// <typeparam name="T">The type of the value for which to provide a display text.</typeparam>
public interface IDisplayTextStrategy<in T> : IDisplayTextStrategy
{
    /// <summary>
    /// Gets the display text for a given value of type T, optionally using culture-specific information to determine the appropriate display text.
    /// </summary>
    /// <param name="value">The value for which to get the display text.</param>
    /// <param name="options">The options to use when generating the display text.</param>
    /// <param name="culture">The culture to use for determining the display text.</param>
    /// <returns>The display text for the given value.</returns>
    string GetDisplayText(T value, DisplayTextOptions options, CultureInfo culture);
}

/// <summary>
/// Provides a non-generic interface for display text strategies, allowing for retrieval of display texts for values of any type. This interface is used internally to enable polymorphic behavior when retrieving display texts without needing to know the specific type of the value at compile time.
/// </summary>
public interface IDisplayTextStrategy
{
    /// <summary>
    /// Gets the display text for a given value, using the provided options and culture information. This method is intended to be called by the generic <see cref="IDisplayTextStrategy{T}"/> implementations, which will cast the value to the appropriate type before calling this method. The implementation of this method should handle the logic for determining the display text based on the value, options, and culture, potentially using type-specific logic if necessary.
    /// </summary>
    /// <param name="value">The value for which to get the display text.</param>
    /// <param name="options">The options to use when generating the display text.</param>
    /// <param name="culture">The culture to use for determining the display text.</param>
    /// <returns>The display text for the given value.</returns>
    string GetDisplayText(object value, DisplayTextOptions options, CultureInfo culture);
}
