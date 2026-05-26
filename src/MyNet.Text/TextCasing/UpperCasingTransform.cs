// -----------------------------------------------------------------------
// <copyright file="UpperCasingTransform.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Text.TextCasing;

/// <summary>
/// Transforms a string to upper case.
/// </summary>
public sealed class UpperCasingTransform : ITextCasingTransform
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpperCasingTransform"/> class.
    /// </summary>
    internal UpperCasingTransform() { }

    /// <summary>
    /// Transforms the input to upper case.
    /// </summary>
    /// <param name="input">The string to transform.</param>
    /// <param name="culture">The culture to use for the transformation.</param>
    /// <returns>The transformed string in upper case.</returns>
    public string Apply(string input, CultureInfo culture) => input.ToUpper(culture);
}
