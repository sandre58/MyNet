// -----------------------------------------------------------------------
// <copyright file="AddressFormatter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using MyNet.Geography;

namespace MyNet.Humanizer.Formatting.Addresses;

/// <summary>
/// Current implementation of <see cref="IAddressFormatter"/> that provides a standard address format using invariant culture.
/// </summary>
public class AddressFormatter() : AddressFormatterBase(CultureInfo.InvariantCulture)
{
    /// <inheritdoc/>
    protected override string Template { get; } = "{Street}\n{PostalCode} {City}\n{Country}";
}

/// <summary>
/// Base class for address formatters, providing common functionality and properties for derived classes.
/// </summary>
/// <param name="culture">The culture information for the address formatter.</param>
public abstract partial class AddressFormatterBase(CultureInfo culture) : IAddressFormatter
{
    /// <inheritdoc/>
    public CultureInfo Culture => culture;

    /// <summary>
    /// Gets the address template used for formatting addresses. The template should contain tokens in the format of {TokenName} which will be replaced with the corresponding address components.
    /// </summary>
    protected abstract string Template { get; }

    /// <inheritdoc/>
    public string Format(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);

        var values = new Dictionary<string, string?>
        {
            ["Street"] = address.Street,
            ["City"] = address.City,
            ["PostalCode"] = address.PostalCode,
            ["Country"] = address.Country?.Name
        };

        var lines = Template
            .Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(line => ReplaceTokens(line, values))
            .Select(CleanupWhitespace)
            .Where(static line => !string.IsNullOrWhiteSpace(line));

        return string.Join(Environment.NewLine, lines);
    }

    /// <summary>
    /// Replaces the tokens in the template with the corresponding values from the dictionary. Tokens are defined in the format of {TokenName}. If a token does not have a corresponding value in the dictionary, it will be replaced with an empty string.
    /// </summary>
    /// <param name="template">The template string containing tokens to be replaced.</param>
    /// <param name="values">A dictionary containing the values for the tokens.</param>
    /// <returns>The template string with tokens replaced by their corresponding values.</returns>
    private static string ReplaceTokens(string template, Dictionary<string, string?> values) => TokenRegex().Replace(template, match =>
    {
        var token = match.Groups[1].Value;

        return values.TryGetValue(token, out var value)
            ? value ?? string.Empty
            : string.Empty;
    });

    /// <summary>
    /// Cleans up the whitespace in the provided string by replacing multiple consecutive whitespace characters with a single space and trimming leading and trailing whitespace. This ensures that the formatted address does not contain unnecessary spaces or blank lines.
    /// </summary>
    /// <param name="value">The string value to clean up.</param>
    /// <returns>The cleaned-up string with normalized whitespace.</returns>
    private static string CleanupWhitespace(string value) => MultiWhitespaceRegex().Replace(value, " ").Trim();

    [GeneratedRegex(@"\{(.*?)\}")]
    private static partial Regex TokenRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex MultiWhitespaceRegex();
}
