// -----------------------------------------------------------------------
// <copyright file="HumanizeKeyFormatterTransform.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MyNet.Text.Formatting;

/// <summary>
/// Converts key-like identifiers (PascalCase, camelCase, snake_case, kebab-case) into human-readable text.
/// </summary>
public sealed partial class HumanizeKeyFormatterTransform : ITextFormatterTransform
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HumanizeKeyFormatterTransform"/> class.
    /// </summary>
    internal HumanizeKeyFormatterTransform() { }

    /// <inheritdoc/>
    public string Apply(string input, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(culture);

        if (input.Length == 0)
            return input;

        var normalized = SeparatorRegex().Replace(input, " ");
        normalized = AcronymBoundaryRegex().Replace(normalized, " ");
        normalized = CasingBoundaryRegex().Replace(normalized, " ");
        normalized = LetterToDigitBoundaryRegex().Replace(normalized, " ");
        normalized = DigitToLetterBoundaryRegex().Replace(normalized, " ");
        normalized = MultiWhitespaceRegex().Replace(normalized, " ").Trim();

        if (normalized.Length == 0)
            return normalized;

        var sentence = normalized.ToLower(culture);
        return sentence.Length == 1
            ? sentence.ToUpper(culture)
            : char.ToUpper(sentence[0], culture) + sentence[1..];
    }

    [GeneratedRegex("[\\s_\\-./:]+", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex SeparatorRegex();

    [GeneratedRegex("(?<=[\\p{Lu}])(?=[\\p{Lu}][\\p{Ll}])", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex AcronymBoundaryRegex();

    [GeneratedRegex("(?<=[\\p{Ll}\\p{Nd}])(?=[\\p{Lu}])", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex CasingBoundaryRegex();

    [GeneratedRegex("(?<=[\\p{L}])(?=[\\p{Nd}])", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex LetterToDigitBoundaryRegex();

    [GeneratedRegex("(?<=[\\p{Nd}])(?=[\\p{L}])", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex DigitToLetterBoundaryRegex();

    [GeneratedRegex("\\s+", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex MultiWhitespaceRegex();
}
