// -----------------------------------------------------------------------
// <copyright file="SlugifyTransform.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text;
using MyNet.Utilities.Text.Normalization;

namespace MyNet.Utilities.Text.Slugification;

/// <summary>
/// Converts text to a URL-friendly slug.
/// </summary>
public sealed class SlugifyTransform(TextSlugifierOptions options) : ITextSlugifierTransform
{
    private static readonly DiacriticsRemovalTransform DiacriticsRemoval = new();

    /// <inheritdoc/>
    public string Apply(string input, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(culture);

        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var value = options.RemoveDiacritics ? DiacriticsRemoval.Apply(input, culture) : input;
        var sb = new StringBuilder(value.Length);
        var wroteSeparator = false;

        foreach (var ch in value)
        {
            if (char.IsLetterOrDigit(ch))
            {
                sb.Append(ch);
                wroteSeparator = false;
                continue;
            }

            if (char.IsWhiteSpace(ch) || ch == '_' || ch == '-' || ch == '.' || ch == '/')
            {
                if (sb.Length > 0 && !wroteSeparator)
                {
                    sb.Append(options.Separator);
                    wroteSeparator = true;
                }
            }
        }

        var slug = sb.ToString().Trim(options.Separator);
        return options.Lowercase ? slug.ToLower(culture) : slug;
    }
}
