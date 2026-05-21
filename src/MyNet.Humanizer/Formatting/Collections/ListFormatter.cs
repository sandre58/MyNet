// -----------------------------------------------------------------------
// <copyright file="ListFormatter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyNet.Globalization.Localization.Translation;
using MyNet.Humanizer.Resources;

namespace MyNet.Humanizer.Formatting.Collections;

/// <summary>
/// Formats lists of items into human readable strings. This class is sealed and cannot be inherited. It provides a convenient way to format lists of items using the default formatting logic defined in the base class <see cref="ListFormatterBase"/>. For more advanced scenarios, you can create your own custom list formatter by inheriting from <see cref="ListFormatterBase"/> and overriding the relevant methods to provide custom formatting logic.
/// </summary>
/// <param name="translationService">The translation service used to translate resource keys.</param>
/// <param name="culture">The culture used for humanization and formatting.</param>
public sealed class ListFormatter(ITranslationService translationService, CultureInfo culture)
    : ListFormatterBase(translationService, culture);

/// <summary>
/// Base class for list formatters. Provides common logic for formatting lists of items into human readable strings.
/// </summary>
public abstract class ListFormatterBase(ITranslationService translationService, CultureInfo supportedCulture) : IListFormatter
{
    /// <summary>
    /// Gets the supported culture for this humanizer.
    /// </summary>
    public CultureInfo Culture => supportedCulture;

    /// <summary>
    /// Gets the separator used to separate the last two items in a list when the conjunction is "and".
    /// </summary>
    protected virtual string AndSeparator { get; } = translationService.Translate(nameof(ListResources.AndSeparator), TranslationOptionsPresets.Default, nameof(ListResources), supportedCulture);

    /// <summary>
    /// Gets the separator used to separate the last two items in a list when the conjunction is "or".
    /// </summary>
    protected virtual string OrSeparator { get; } = translationService.Translate(nameof(ListResources.OrSeparator), TranslationOptionsPresets.Default, nameof(ListResources), supportedCulture);

    /// <summary>
    /// Formats a list of items into a human readable string.
    /// </summary>
    /// <param name="items">The list of items to format.</param>
    /// <param name="options">The formatting options to use.</param>
    /// <returns>A human readable string representation of the list.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the items parameter is null.</exception>
    public string Format(IEnumerable<string?> items, ListFormattingOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(items);

        options ??= ListFormattingOptions.Default;

        var values = items
            .Select(Normalize)
            .Where(x => !options.IgnoreNullOrWhiteSpace || !string.IsNullOrWhiteSpace(x))
            .ToArray();

        return values.Length switch
        {
            0 => string.Empty,
            1 => values[0],
            2 => FormatTwoItems(values, options),
            _ => FormatManyItems(values, options)
        };
    }

    /// <summary>
    /// Normalizes a string value by trimming it and replacing null with an empty string. This method can be overridden by derived classes to provide custom normalization logic.
    /// </summary>
    /// <param name="value">The string value to normalize.</param>
    /// <returns>The normalized string value.</returns>
    protected virtual string Normalize(string? value) => value?.Trim() ?? string.Empty;

    /// <summary>
    /// Formats a list of two items into a human readable string using the appropriate conjunction and separator based on the provided options.
    /// </summary>
    /// <param name="items">The array of two items to format.</param>
    /// <param name="options">The formatting options to use.</param>
    /// <returns>A human readable string representation of the two items.</returns>
    protected virtual string FormatTwoItems(string[] items, ListFormattingOptions options) => string.Concat(items[0], GetFinalSeparator(options), items[1]);

    /// <summary>
    /// Formats a list of three or more items into a human readable string by joining the items with the appropriate separators and conjunction based on the provided options.
    /// </summary>
    /// <param name="items">The array of items to format.</param>
    /// <param name="options">The formatting options to use.</param>
    /// <returns>A human readable string representation of the items.</returns>
    protected virtual string FormatManyItems(string[] items, ListFormattingOptions options)
    {
        var lastIndex = items.Length - 1;

        var start = string.Join(options.Separator, items.AsSpan(0, lastIndex).ToArray());

        var separator = options.UseOxfordComma && options.Conjunction != ListConjunction.None ? options.Separator.TrimEnd() : string.Empty;

        return string.Concat(start, separator, GetFinalSeparator(options), items[lastIndex]);
    }

    /// <summary>
    /// Gets the appropriate separator to use between the last two items in a list based on the conjunction specified in the formatting options. If the conjunction is "or", it returns the value of <see cref="OrSeparator"/>; otherwise, it returns the value of <see cref="AndSeparator"/>. This method can be overridden by derived classes to provide custom logic for determining the final separator.
    /// </summary>
    /// <param name="options">The formatting options to use.</param>
    /// <returns>The appropriate separator string.</returns>
    protected virtual string GetFinalSeparator(ListFormattingOptions options)
        => options.Conjunction switch
        {
            ListConjunction.Or => $" {OrSeparator.Trim()} ",
            ListConjunction.None => options.Separator,
            ListConjunction.Ampersand => " & ",
            _ => $" {AndSeparator.Trim()} "
        };
}
