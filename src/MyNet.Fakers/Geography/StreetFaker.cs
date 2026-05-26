// -----------------------------------------------------------------------
// <copyright file="StreetFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Generator;
using MyNet.Globalization.Localization.Providers;

namespace MyNet.Fakers.Geography;

/// <summary>
/// Current implementation of <see cref="IStreetFaker"/> that generates fake street addresses based on localized datasets and customizable templates. The class utilizes a random generator to create street numbers, select random street names, types, and suffixes from the provided datasets, and applies these components to predefined street address formats to produce realistic and culturally appropriate street addresses. The implementation also includes a helper method to apply the selected components to the chosen template while ensuring proper formatting and spacing in the final output.
/// </summary>
/// <param name="random">The random generator used to create street numbers and select random elements from datasets.</param>
/// <param name="source">The localization provider resolver used to obtain localized datasets.</param>
public sealed class StreetFaker(IRandomGenerator random, ICultureScopedServiceSource<IAddressFakerProvider> source) : IStreetFaker
{
    /// <inheritdoc />
    public string Number(CultureInfo? culture = null)
    {
        var provider = source.Get(culture);
        culture ??= provider.Culture;

        var number = random.Int(1, 200).ToString(culture);
        var suffix = random.Item(provider.StreetSuffixes);

        return string.IsNullOrEmpty(suffix) ? number : $"{number}{suffix}";
    }

    /// <inheritdoc />
    public string Type(CultureInfo? culture = null)
    {
        var provider = source.Get(culture);

        return random.Item(provider.StreetTypes);
    }

    /// <inheritdoc />
    public string Name(CultureInfo? culture = null)
    {
        var provider = source.Get(culture);

        return random.Item(provider.StreetNames);
    }

    /// <inheritdoc />
    public string Street(CultureInfo? culture = null)
    {
        var provider = source.Get(culture);
        culture ??= provider.Culture;

        var parts = new StreetParts(
            Number: random.Int(1, 200).ToString(culture),
            Suffix: random.Item(provider.StreetSuffixes),
            Type: random.Item(provider.StreetTypes),
            Name: random.Item(provider.StreetNames));

        var template = random.Item(provider.StreetFormats);

        return Apply(template, parts);
    }

    /// <summary>
    /// Applies the given street address template by replacing the placeholders with the corresponding parts of the street address.
    /// Uses StringBuilder instead of chained Replaces to avoid intermediate allocations.
    /// </summary>
    /// <param name="template">The street address template containing placeholders.</param>
    /// <param name="parts">The <see cref="StreetParts"/> record containing the actual values to replace the placeholders.</param>
    /// <returns>A formatted street address string.</returns>
    private static string Apply(string template, StreetParts parts)
    {
        var builder = new System.Text.StringBuilder(template.Length);

        for (var i = 0; i < template.Length; i++)
        {
            if (template[i] == '{')
            {
                var end = template.IndexOf('}', i);
                if (end > i)
                {
                    var placeholder = template[(i + 1)..end];
                    var replacement = placeholder switch
                    {
                        "Number" => parts.Number,
                        "Suffix" => parts.Suffix ?? string.Empty,
                        "Type" => parts.Type,
                        "Name" => parts.Name,
                        _ => placeholder
                    };

                    builder.Append(replacement);
                    i = end;
                    continue;
                }
            }

            builder.Append(template[i]);
        }

        // Trim multiple spaces to single space
        var result = builder.ToString();
        while (result.Contains("  ", StringComparison.OrdinalIgnoreCase))
            result = result.Replace("  ", " ", StringComparison.OrdinalIgnoreCase);

        return result.Trim();
    }

    /// <summary>
    /// Represents the components of a street address, including the street number, optional suffix, street type, and street name. This record is used to hold the individual parts of a street address that can be combined according to different formats when generating fake street addresses.
    /// </summary>
    /// <param name="Number">The street number.</param>
    /// <param name="Suffix">The optional street suffix.</param>
    /// <param name="Type">The street type.</param>
    /// <param name="Name">The street name.</param>
    private sealed record StreetParts(string Number, string? Suffix, string Type, string Name);
}
