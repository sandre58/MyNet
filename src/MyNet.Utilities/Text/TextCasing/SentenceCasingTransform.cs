// -----------------------------------------------------------------------
// <copyright file="SentenceCasingTransform.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;

namespace MyNet.Utilities.Text.TextCasing;

/// <summary>
/// Transforms a string to sentence case, meaning the first letter is capitalized and the rest are unchanged.
/// </summary>
public sealed class SentenceCasingTransform : ITextCasingTransform
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SentenceCasingTransform"/> class.
    /// </summary>
    internal SentenceCasingTransform() { }

    /// <summary>
    /// Transforms the input to sentence case.
    /// </summary>
    /// <param name="input">The string to transform.</param>
    /// <param name="culture">The culture to use for the transformation.</param>
    /// <returns>The transformed string in sentence case.</returns>
    public string Apply(string input, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(culture);

        return string.IsNullOrEmpty(input)
            ? input
            : input.Length == 1
            ? input.ToUpper(culture)
            : string.Concat(
            culture.TextInfo.ToUpper(input[..1]),
            input[1..]);
    }
}
