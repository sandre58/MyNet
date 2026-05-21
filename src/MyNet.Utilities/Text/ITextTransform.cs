// -----------------------------------------------------------------------
// <copyright file="ITextTransform.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Utilities.Text;

/// <summary>
/// Represents a transformation that can be applied to a string, potentially using culture-specific rules. Implementations of this interface can perform various types of transformations, such as changing the case of the text, applying ordinalization, or any other kind of string manipulation that may depend on cultural context. The Apply method takes an input string and a CultureInfo object to perform the transformation accordingly.
/// </summary>
public interface ITextTransform
{
    /// <summary>
    /// Applies the transformation to the input string using the specified culture. The transformation logic can vary based on the implementation of this interface, and the culture parameter allows for culture-specific transformations when necessary. For example, a case transformation might behave differently for certain cultures due to unique casing rules, or an ordinalization transformation might produce different results based on cultural conventions for ordinal numbers.
    /// </summary>
    /// <param name="input">The string to transform.</param>
    /// <param name="culture">The culture to use for the transformation.</param>
    /// <returns>The transformed string.</returns>
    string Apply(string input, CultureInfo culture);
}
