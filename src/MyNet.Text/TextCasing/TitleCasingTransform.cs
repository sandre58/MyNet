// -----------------------------------------------------------------------
// <copyright file="TitleCasingTransform.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;

namespace MyNet.Text.TextCasing;

/// <summary>
/// Transforms a string to title case, meaning the first letter of each word is capitalized and the rest are lowercase. Words are defined as sequences of letters (including Unicode letters) that may include an apostrophe followed by more letters. Words that are already in all capitals are not modified.
/// </summary>
public sealed class TitleCasingTransform : ITextCasingTransform
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TitleCasingTransform"/> class.
    /// </summary>
    internal TitleCasingTransform() { }

    /// <summary>
    /// Transforms the input string to title case. Each word's first letter is capitalized and the rest are lowercase, except for words that are already in all capitals, which are left unchanged. Words are identified using a regular expression that matches sequences of letters (including Unicode letters) that may include an apostrophe followed by more letters.
    /// </summary>
    /// <param name="input">The string to transform.</param>
    /// <param name="culture">The culture to use for the transformation.</param>
    /// <returns>The transformed string in title case.</returns>
    public string Apply(string input, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(culture);

        return culture.TextInfo.ToTitleCase(input);
    }
}
