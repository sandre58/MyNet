// -----------------------------------------------------------------------
// <copyright file="InitialsFormatterTransform.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text;

namespace MyNet.Text.Formatting;

/// <summary>
/// Builds initials by taking the first character of each whitespace-delimited token.
/// </summary>
public sealed class InitialsFormatterTransform : ITextFormatterTransform
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InitialsFormatterTransform"/> class.
    /// </summary>
    internal InitialsFormatterTransform() { }

    /// <inheritdoc/>
    public string Apply(string input, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(culture);

        if (input.Length == 0)
            return input;

        var builder = new StringBuilder();
        var insideToken = false;

        foreach (var c in input)
        {
            if (char.IsWhiteSpace(c))
            {
                insideToken = false;
                continue;
            }

            if (insideToken)
                continue;

            builder.Append(char.ToUpper(c, culture));
            insideToken = true;
        }

        return builder.ToString();
    }
}
