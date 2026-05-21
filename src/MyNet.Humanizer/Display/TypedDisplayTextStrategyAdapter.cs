// -----------------------------------------------------------------------
// <copyright file="TypedDisplayTextStrategyAdapter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Humanizer.Display;

/// <summary>
/// Adapter that allows an untyped <see cref="IDisplayTextStrategy"/> to be used as a typed <see cref="IDisplayTextStrategy{T}"/>. This is useful for scenarios where a strategy is registered without a specific target type but needs to be applied to a specific type at runtime.
/// </summary>
/// <param name="inner">The inner untyped display text strategy.</param>
/// <typeparam name="T">The target type for the typed display text strategy.</typeparam>
public sealed class TypedDisplayTextStrategyAdapter<T>(IDisplayTextStrategy inner) : IDisplayTextStrategy<T>
    where T : notnull
{
    /// <summary>
    /// Gets the display text for the given value using the inner untyped strategy. The value is passed as an object to the inner strategy, and the result is returned as a string. This method allows the typed strategy to leverage the logic of the untyped strategy while providing type safety at the interface level.
    /// </summary>
    /// <param name="value">The value for which to get the display text.</param>
    /// <param name="options">The display text options.</param>
    /// <param name="culture">The culture to use for formatting.</param>
    /// <returns>The display text for the given value.</returns>
    public string GetDisplayText(T value, DisplayTextOptions options, CultureInfo culture) => inner.GetDisplayText(value, options, culture);

    /// <summary>
    /// Gets the display text for the given value using the inner untyped strategy. This method is part of the IDisplayTextStrategy interface and allows the adapter to be used in contexts where an untyped strategy is expected. The value is passed as an object to the inner strategy, and the result is returned as a string.
    /// </summary>
    /// <param name="value">The value for which to get the display text.</param>
    /// <param name="options">The display text options.</param>
    /// <param name="culture">The culture to use for formatting.</param>
    /// <returns>The display text for the given value.</returns>
    public string GetDisplayText(object value, DisplayTextOptions options, CultureInfo culture) => inner.GetDisplayText(value, options, culture);
}
