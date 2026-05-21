// -----------------------------------------------------------------------
// <copyright file="DiacriticsRemovalTransform.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MyNet.Utilities.Text.Normalization;

/// <summary>
/// Removes diacritical marks from text.
/// </summary>
public sealed class DiacriticsRemovalTransform : ITextNormalizationTransform
{
    /// <inheritdoc/>
    public string Apply(string input, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(culture);

        if (string.IsNullOrEmpty(input))
            return input;

        var normalized = input.Normalize(NormalizationForm.FormD);
        var buffer = new StringBuilder(normalized.Length);

        foreach (var c in from c in normalized let category = CharUnicodeInfo.GetUnicodeCategory(c) where category != UnicodeCategory.NonSpacingMark && category != UnicodeCategory.SpacingCombiningMark select c)
        {
            buffer.Append(c);
        }

        return buffer.ToString().Normalize(NormalizationForm.FormC);
    }
}
