// -----------------------------------------------------------------------
// <copyright file="IInflector.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Globalization.Localization.Providers;

namespace MyNet.Globalization.Inflection;

/// <summary>
/// Defines methods for inflecting words based on quantity, including pluralization and singularization.
/// </summary>
public interface IInflector : ICultureScoped
{
    /// <summary>
    /// Returns the plural form of the specified word based on the inflection rules of the language.
    /// </summary>
    /// <param name="word">The word to be pluralized.</param>
    /// <returns>The plural form of the word.</returns>
    string Pluralize(string word);

    /// <summary>
    /// Returns the singular form of the specified word based on the inflection rules of the language.
    /// </summary>
    /// <param name="word">The word to be singularized.</param>
    /// <returns>The singular form of the word.</returns>
    string Singularize(string word);

    /// <summary>
    /// Returns whether the specified quantity should use the plural form for this language.
    /// </summary>
    /// <param name="count">The quantity to evaluate.</param>
    /// <returns><c>true</c> when the plural form should be used; otherwise <c>false</c>.</returns>
    bool IsPlural(decimal count);

    /// <summary>
    /// Returns the plural category of the specified quantity based on the inflection rules of the language.
    /// </summary>
    /// <param name="count">The quantity to determine the plural category for.</param>
    /// <returns>The plural category of the quantity.</returns>
    PluralCategory GetPluralCategory(decimal count);
}
