// -----------------------------------------------------------------------
// <copyright file="WhitespaceNormalizationTransform.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text;

namespace MyNet.Text.Normalization;

/// <summary>
/// Trims text and optionally collapses whitespace runs.
/// </summary>
public sealed class WhitespaceNormalizationTransform(TextNormalizationOptions options) : ITextNormalizationTransform
{
    /// <inheritdoc/>
    public string Apply(string input, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(culture);

        var value = options.Trim ? input.Trim() : input;
        if (!options.CollapseWhitespace || value.Length == 0)
            return value;

        var sb = new StringBuilder(value.Length);
        var seenWhitespace = false;

        foreach (var ch in value)
        {
            if (char.IsWhiteSpace(ch))
            {
                seenWhitespace = true;
                continue;
            }

            if (seenWhitespace && sb.Length > 0)
                sb.Append(' ');

            sb.Append(ch);
            seenWhitespace = false;
        }

        return sb.ToString();
    }
}
