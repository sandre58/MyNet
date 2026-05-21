// -----------------------------------------------------------------------
// <copyright file="SnakeCasingTransform.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MyNet.Utilities.Text.TextCasing;

/// <summary>
/// Transforms a string to snake_case.
/// </summary>
public sealed partial class SnakeCasingTransform : ITextCasingTransform
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SnakeCasingTransform"/> class.
    /// </summary>
    internal SnakeCasingTransform() { }

    /// <inheritdoc/>
    public string Apply(string input, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(culture);

        return NormalizeSeparatorsRegex()
            .Replace(UpperLowerBoundaryRegex().Replace(AcronymBoundaryRegex().Replace(input, "$1_$2"), "$1_$2"), "_")
            .ToLower(culture);
    }

    [GeneratedRegex("[-\\s]")]
    private static partial Regex NormalizeSeparatorsRegex();

    [GeneratedRegex("([\\p{Ll}\\d])([\\p{Lu}])")]
    private static partial Regex UpperLowerBoundaryRegex();

    [GeneratedRegex("([\\p{Lu}]+)([\\p{Lu}][\\p{Ll}])")]
    private static partial Regex AcronymBoundaryRegex();
}
