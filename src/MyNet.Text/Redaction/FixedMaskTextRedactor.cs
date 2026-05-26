// -----------------------------------------------------------------------
// <copyright file="FixedMaskTextRedactor.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;

namespace MyNet.Text.Redaction;

/// <summary>
/// Redacts text by masking the middle section.
/// </summary>
public sealed class FixedMaskTextRedactor(TextRedactionOptions options) : ITextRedactorTransform
{
    /// <inheritdoc/>
    public string Apply(string input, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(culture);

        if (input.Length == 0)
            return input;

        var visible = options.ShowStart + options.ShowEnd;
        if (visible >= input.Length)
            return input;

        var maskedLength = input.Length - visible;
        return string.Concat(
            input.AsSpan(0, options.ShowStart),
            new string(options.MaskCharacter, maskedLength),
            input.AsSpan(input.Length - options.ShowEnd));
    }
}
