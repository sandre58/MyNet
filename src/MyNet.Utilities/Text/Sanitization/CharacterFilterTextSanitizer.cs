// -----------------------------------------------------------------------
// <copyright file="CharacterFilterTextSanitizer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MyNet.Utilities.Text.Sanitization;

/// <summary>
/// Sanitizes text by preserving only allowed character classes.
/// </summary>
public sealed class CharacterFilterTextSanitizer(TextSanitizationOptions options) : ITextSanitizerTransform
{
    private readonly HashSet<char> _additionalAllowedCharacters = [.. options.AdditionalAllowedCharacters];

    /// <inheritdoc/>
    public string Apply(string input, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(culture);

        if (input.Length == 0)
            return input;

        var sb = new StringBuilder(input.Length);

        foreach (var ch in input.Where(ch => _additionalAllowedCharacters.Contains(ch)
                                             || (options.KeepLetters && char.IsLetter(ch))
                                             || (options.KeepDigits && char.IsDigit(ch))
                                             || (options.KeepWhitespace && char.IsWhiteSpace(ch))))
        {
            sb.Append(ch);
        }

        return sb.ToString();
    }
}
