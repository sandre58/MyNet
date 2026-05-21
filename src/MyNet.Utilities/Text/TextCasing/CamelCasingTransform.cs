// -----------------------------------------------------------------------
// <copyright file="CamelCasingTransform.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;

namespace MyNet.Utilities.Text.TextCasing;

/// <summary>
/// Transforms a string to camelCase.
/// </summary>
public sealed class CamelCasingTransform : ITextCasingTransform
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CamelCasingTransform"/> class.
    /// </summary>
    internal CamelCasingTransform() { }

    /// <inheritdoc/>
    public string Apply(string input, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(culture);

        var pascal = new PascalCasingTransform().Apply(input, culture);
        return pascal.Length > 0
            ? $"{char.ToLower(pascal[0], culture)}{pascal[1..]}"
            : pascal;
    }
}
