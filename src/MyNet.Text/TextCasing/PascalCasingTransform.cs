// -----------------------------------------------------------------------
// <copyright file="PascalCasingTransform.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MyNet.Text.TextCasing;

/// <summary>
/// Transforms a string to PascalCase.
/// </summary>
public sealed partial class PascalCasingTransform : ITextCasingTransform
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PascalCasingTransform"/> class.
    /// </summary>
    internal PascalCasingTransform() { }

    /// <inheritdoc/>
    public string Apply(string input, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(culture);

        return PascalizeRegex().Replace(input, match => match.Groups[1].Value.ToUpper(culture));
    }

    [GeneratedRegex("(?:^|_| +)(.)")]
    private static partial Regex PascalizeRegex();
}
