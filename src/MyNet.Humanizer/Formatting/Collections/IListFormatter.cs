// -----------------------------------------------------------------------
// <copyright file="IListFormatter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.Globalization.Localization.Providers;

namespace MyNet.Humanizer.Formatting.Collections;

/// <summary>
/// Can format a list of strings into a human readable string.
/// </summary>
public interface IListFormatter : ICultureScoped
{
    /// <summary>
    /// Formats a list of strings into a human readable string.
    /// </summary>
    /// <param name="items">The list of strings to format.</param>
    /// <param name="options">The options to use when formatting the list.</param>
    /// <returns>A human readable string representing the list of items.</returns>
    string Format(IEnumerable<string?> items, ListFormattingOptions? options = null);
}
