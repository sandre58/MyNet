// -----------------------------------------------------------------------
// <copyright file="RegexTextRedactor.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MyNet.Text.Redaction;

/// <summary>
/// Redacts text parts matching a regular expression.
/// </summary>
public sealed class RegexTextRedactor(Regex pattern, string replacement = "***") : ITextRedactorTransform
{
    /// <inheritdoc/>
    public string Apply(string input, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(culture);
        ArgumentNullException.ThrowIfNull(pattern);

        return pattern.Replace(input, replacement);
    }
}
