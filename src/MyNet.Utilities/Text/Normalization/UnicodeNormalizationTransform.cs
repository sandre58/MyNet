// -----------------------------------------------------------------------
// <copyright file="UnicodeNormalizationTransform.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text;

namespace MyNet.Utilities.Text.Normalization;

/// <summary>
/// Applies Unicode normalization to the input text.
/// </summary>
public sealed class UnicodeNormalizationTransform(NormalizationForm normalizationForm) : ITextNormalizationTransform
{
    /// <inheritdoc/>
    public string Apply(string input, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(culture);

        return input.Normalize(normalizationForm);
    }
}
