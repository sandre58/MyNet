// -----------------------------------------------------------------------
// <copyright file="KebabCasingTransform.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;

namespace MyNet.Utilities.Text.TextCasing;

/// <summary>
/// Transforms a string to kebab-case.
/// </summary>
public sealed class KebabCasingTransform : ITextCasingTransform
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KebabCasingTransform"/> class.
    /// </summary>
    internal KebabCasingTransform() { }

    /// <inheritdoc/>
    public string Apply(string input, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(culture);

        var snake = new SnakeCasingTransform().Apply(input, culture);
        return snake.Replace('_', '-');
    }
}
