// -----------------------------------------------------------------------
// <copyright file="LowerCasingTransform.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Text.TextCasing;

public sealed class LowerCasingTransform : ITextCasingTransform
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LowerCasingTransform"/> class.
    /// </summary>
    internal LowerCasingTransform() { }

    /// <summary>
    /// Transforms the input string to lower case using the specified culture.
    /// </summary>
    /// <param name="input">The string to transform.</param>
    /// <param name="culture">The culture to use for the transformation.</param>
    /// <returns>The transformed string in lower case.</returns>
    public string Apply(string input, CultureInfo culture) => culture.TextInfo.ToLower(input);
}
