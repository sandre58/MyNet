// -----------------------------------------------------------------------
// <copyright file="TemplateTransform.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MyNet.Text.Templating;

/// <summary>
/// Current implementation of <see cref="ITemplateTransform"/> that supports named placeholders with optional format specifiers.
/// <example>
/// {count}
/// {count:N2}
/// {price:C}
/// {date:yyyy-MM-dd}
/// </example>
/// </summary>
public sealed partial class TemplateTransform(TextTemplateOptions options) : ITemplateTransform
{
    private static readonly ConcurrentDictionary<string, Func<object?, CultureInfo, string?, string>> CachedFormatters = new();

    /// <inheritdoc />
    public string Apply(string input, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(culture);

        var rendered = PlaceholderRegex().Replace(input, match =>
        {
            var argumentName = match.Groups["name"].Value;
            var format = match.Groups["format"].Success ? match.Groups["format"].Value : null;

            return !TryResolveArgument(argumentName, options, out var value) ? match.Value : FormatValue(value, culture, format);
        });

        return ApplyQuantityRendering(rendered, options, culture);
    }

    /// <summary>
    /// Tries to resolve an argument value by name from the provided translation options.
    /// </summary>
    /// <param name="argumentName">The name of the argument to resolve.</param>
    /// <param name="options">The translation options containing the arguments.</param>
    /// <param name="value">The resolved value, if found.</param>
    /// <returns>True if the argument was successfully resolved; otherwise, false.</returns>
    private static bool TryResolveArgument(string argumentName, TextTemplateOptions options, out object? value)
    {
        // Built-in count support
        if (argumentName.Equals(nameof(TextTemplateOptions.Quantity), StringComparison.OrdinalIgnoreCase))
        {
            value = options.Quantity;
            return options.Quantity.HasValue;
        }

        // Custom arguments
        if (options.Arguments.TryGetValue(argumentName, out value))
        {
            return true;
        }

        value = null;
        return false;
    }

    /// <summary>
    /// Formats a value using the provided culture and optional format string.
    /// </summary>
    private static string FormatValue(object? value, CultureInfo culture, string? format)
    {
        if (value is null)
        {
            return string.Empty;
        }

        var formatter = CachedFormatters.GetOrAdd(
            value.GetType().FullName + "|" + format,
            static _ => static (v, c, f) => v is null
                ? string.Empty
                : v switch
                {
                    IFormattable formattable => formattable.ToString(f, c),
                    _ => v.ToString() ?? string.Empty
                });

        return formatter(value, culture, format);
    }

    /// <summary>
    /// Applies quantity rendering to the rendered translation string based on the provided options and culture. This method checks if a quantity value is specified in the options and, if so, formats it according to the specified rendering mode (prefix or suffix) and separator. If the rendered string already contains a placeholder for quantity, it will not apply additional rendering to avoid duplication.
    /// </summary>
    /// <param name="rendered">The rendered translation string.</param>
    /// <param name="options">The translation options containing the quantity and rendering settings.</param>
    /// <param name="culture">The culture to use for formatting the quantity.</param>
    /// <returns>The translation string with quantity rendering applied, if applicable.</returns>
    private static string ApplyQuantityRendering(string rendered, TextTemplateOptions options, CultureInfo culture)
    {
        if (!options.Quantity.HasValue)
            return rendered;

        // Already handled by placeholder
        if (rendered.Contains($"{{{{{nameof(TextTemplateOptions.Quantity)}", StringComparison.OrdinalIgnoreCase))
            return rendered;

        var quantity = FormatValue(options.Quantity.Value, culture, options.QuantityFormat);

        return options.QuantityRenderingMode switch
        {
            QuantityRenderingMode.Prefix => $"{quantity}{options.QuantitySeparator}{rendered}",
            QuantityRenderingMode.Suffix => $"{rendered}{options.QuantitySeparator}{quantity}",
            _ => rendered
        };
    }

    /// <summary>
    /// Matches named placeholders with optional format specifier.
    /// Examples:
    /// {count}
    /// {count:N2}
    /// {price:C}
    /// </summary>
    [GeneratedRegex(@"\{(?<name>[a-zA-Z0-9_]+)(:(?<format>[^}]+))?\}", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex PlaceholderRegex();
}
