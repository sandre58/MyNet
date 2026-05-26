// -----------------------------------------------------------------------
// <copyright file="ListFormatterExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyNet.Globalization.Facade;
using MyNet.Humanizer.Formatting.Collections;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Humanizer.Facade;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Provides convenient extension methods for list formatting using <see cref="IListFormatter"/>.
/// </summary>
public static class ListFormatterExtensions
{
    /// <summary>
    /// Gets or creates a <see cref="ListFormattingOptions"/> builder for a fluent configuration.
    /// </summary>
    /// <returns>A new <see cref="ListFormattingOptionsBuilder"/> instance.</returns>
    public static ListFormattingOptionsBuilder BuildOptions() => new();

    extension<T>(IEnumerable<T> source)
    {
        /// <summary>
        /// Formats a collection of items into a human-readable list using the default <see cref="IListFormatter"/>.
        /// Returns an empty string if the collection is empty or no formatter is available.
        /// </summary>
        /// <param name="conjunction">The conjunction to use (And, Or, None, or Ampersand). Defaults to And.</param>
        /// <param name="separator">The separator between items. Defaults to ", ".</param>
        /// <param name="culture">The culture to use when formatting the list.</param>
        /// <returns>A formatted string representation of the list.</returns>
        /// <example>
        /// <code>
        /// var items = new[] { "Apple", "Banana", "Cherry" };
        /// var formatted = items.Humanize(); // "Apple, Banana and Cherry"
        /// var withOr = items.Humanize(ListConjunction.Or); // "Apple, Banana or Cherry"
        /// </code>
        /// </example>
        public string Humanize(ListConjunction conjunction = ListConjunction.And, string separator = ", ", CultureInfo? culture = null)
            => source.Humanize(x => x?.ToString(), new() { Conjunction = conjunction, Separator = separator }, culture);

        /// <summary>
        /// Formats a collection of items into a human-readable list using a custom formatter for each item.
        /// </summary>
        /// <param name="formatter">The formatter function to apply to each item. Cannot be null.</param>
        /// <param name="conjunction">The conjunction to use (And, Or, None, or Ampersand). Defaults to And.</param>
        /// <param name="separator">The separator between items. Defaults to ", ".</param>
        /// <param name="culture">The culture to use when formatting the list.</param>
        /// <returns>A formatted string representation of the list.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="formatter"/> is null.</exception>
        public string Humanize(Func<T, string?> formatter, ListConjunction conjunction = ListConjunction.And, string separator = ", ", CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(formatter);
            return source.Humanize(formatter, new() { Conjunction = conjunction, Separator = separator }, culture);
        }

        /// <summary>
        /// Formats a collection of items into a human-readable list using the provided formatting options.
        /// Returns an empty string if the collection is empty or no formatter is available.
        /// </summary>
        /// <param name="options">The formatting options to use. If null, default options are applied.</param>
        /// <param name="culture">The culture to use when formatting the list.</param>
        /// <returns>A formatted string representation of the list.</returns>
        public string Humanize(ListFormattingOptions options, CultureInfo? culture = null)
            => source.Humanize(x => x?.ToString(), options, culture);

        /// <summary>
        /// Formats a collection of items into a human-readable list using a custom formatter and formatting options.
        /// Returns an empty string if the collection is empty or no formatter is available.
        /// </summary>
        /// <param name="formatter">The formatter function to apply to each item. Cannot be null.</param>
        /// <param name="options">The formatting options to use. If null, default options are applied.</param>
        /// <param name="culture">The culture to use when formatting the list.</param>
        /// <returns>A formatted string representation of the list.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="formatter"/> is null.</exception>
        public string Humanize(Func<T, string?> formatter, ListFormattingOptions options, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(formatter);

            var items = source.Select(formatter).ToList();
            return Localizer.ForCulture(culture).GetRequired<IListFormatter>().Format(items, options);
        }

        /// <summary>
        /// Formats a collection of items with an optional prefix and suffix.
        /// </summary>
        /// <param name="prefix">An optional prefix to prepend to the formatted list (e.g., "Items: ").</param>
        /// <param name="suffix">An optional suffix to append to the formatted list (e.g., ".").</param>
        /// <param name="options">The formatting options to use. If null, default options are applied.</param>
        /// <param name="culture">The culture to use when formatting the list.</param>
        /// <returns>The formatted list with optional prefix and suffix, or an empty string if the list is empty.</returns>
        /// <example>
        /// <code>
        /// var items = new[] { "Apple", "Banana" };
        /// var result = items.HumanizeWith("Items: ", "."); // "Items: Apple and Banana."
        /// </code>
        /// </example>
        public string HumanizeWith(string? prefix = null, string? suffix = null, ListFormattingOptions? options = null, CultureInfo? culture = null)
        {
            var formatted = source.Humanize(options ?? ListFormattingOptions.Default, culture);
            return string.IsNullOrEmpty(formatted) ? formatted : $"{prefix}{formatted}{suffix}";
        }

        /// <summary>
        /// Formats a collection of items with a custom formatter, optional prefix and suffix.
        /// </summary>
        /// <param name="formatter">The formatter function to apply to each item. Cannot be null.</param>
        /// <param name="prefix">An optional prefix to prepend to the formatted list.</param>
        /// <param name="suffix">An optional suffix to append to the formatted list.</param>
        /// <param name="options">The formatting options to use. If null, default options are applied.</param>
        /// <param name="culture">The culture to use when formatting the list.</param>
        /// <returns>The formatted list with optional prefix and suffix, or an empty string if the list is empty.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="formatter"/> is null.</exception>
        public string HumanizeWith(Func<T, string?> formatter, string? prefix = null, string? suffix = null, ListFormattingOptions? options = null, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(formatter);
            var formatted = source.Humanize(formatter, options ?? ListFormattingOptions.Default, culture);
            return string.IsNullOrEmpty(formatted) ? formatted : $"{prefix}{formatted}{suffix}";
        }

        /// <summary>
        /// Formats a collection of items with parentheses around the list.
        /// </summary>
        /// <param name="options">The formatting options to use. If null, default options are applied.</param>
        /// <param name="culture">The culture to use when formatting the list.</param>
        /// <returns>The formatted list in parentheses, or an empty string if the list is empty.</returns>
        /// <example>
        /// <code>
        /// var items = new[] { "Apple", "Banana" };
        /// var result = items.HumanizeInParentheses(); // "(Apple and Banana)"
        /// </code>
        /// </example>
        public string HumanizeInParentheses(ListFormattingOptions? options = null, CultureInfo? culture = null)
            => source.HumanizeWith("(", ")", options, culture);

        /// <summary>
        /// Formats a collection of items with brackets around the list.
        /// </summary>
        /// <param name="options">The formatting options to use. If null, default options are applied.</param>
        /// <param name="culture">The culture to use when formatting the list.</param>
        /// <returns>The formatted list in brackets, or an empty string if the list is empty.</returns>
        public string HumanizeInBrackets(ListFormattingOptions? options = null, CultureInfo? culture = null)
            => source.HumanizeWith("[", "]", options, culture);

        /// <summary>
        /// Formats a collection of items with braces around the list.
        /// </summary>
        /// <param name="options">The formatting options to use. If null, default options are applied.</param>
        /// <param name="culture">The culture to use when formatting the list.</param>
        /// <returns>The formatted list in braces, or an empty string if the list is empty.</returns>
        public string HumanizeInBraces(ListFormattingOptions? options = null, CultureInfo? culture = null)
            => source.HumanizeWith("{", "}", options, culture);

        /// <summary>
        /// Formats non-null items from a nullable collection.
        /// </summary>
        /// <param name="options">The formatting options to use. If null, default options are applied.</param>
        /// <param name="culture">The culture to use when formatting the list.</param>
        /// <returns>A formatted string of non-null items.</returns>
        public string HumanizeNonNull(ListFormattingOptions? options = null, CultureInfo? culture = null)
        {
            options ??= ListFormattingOptions.Default;

            // Create a new options with IgnoreNullOrWhiteSpace set to true
            var mergedOptions = new ListFormattingOptions
            {
                Conjunction = options.Conjunction,
                Separator = options.Separator,
                UseOxfordComma = options.UseOxfordComma,
                TrimItems = options.TrimItems,
                IgnoreNullOrWhiteSpace = true
            };

            return source.Humanize(mergedOptions, culture);
        }
    }
}
