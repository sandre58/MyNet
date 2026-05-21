// -----------------------------------------------------------------------
// <copyright file="IPluralizationService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Globalization.Inflection;

public interface IPluralizationService
{
    /// <summary>
    /// Returns the plural form of the specified word based on the inflection rules of the language.
    /// </summary>
    /// <param name="word">The word to be pluralized.</param>
    /// <param name="culture">The culture used to determine the correct form of the word. If <c>null</c>, the current culture is used.</param>
    /// <returns>The plural form of the word.</returns>
    string Pluralize(string word, CultureInfo? culture = null);

    /// <summary>
    /// Returns the singular form of the specified word based on the inflection rules of the language.
    /// </summary>
    /// <param name="word">The word to be singularized.</param>
    /// <param name="culture">The culture used to determine the correct form of the word. If <c>null</c>, the current culture is used.</param>
    /// <returns>The singular form of the word.</returns>
    string Singularize(string word, CultureInfo? culture = null);

    /// <summary>
    /// Returns whether the specified quantity should use the plural form for this language.
    /// </summary>
    /// <param name="count">The quantity to evaluate.</param>
    /// <param name="culture">The culture used to determine the correct form of the word. If <c>null</c>, the current culture is used.</param>
    /// <returns><c>true</c> when the plural form should be used; otherwise <c>false</c>.</returns>
    bool IsPlural(decimal count, CultureInfo? culture = null);

    /// <summary>
    /// Returns the plural category of the specified quantity based on the inflection rules of the language.
    /// </summary>
    /// <param name="count">The quantity to determine the plural category for.</param>
    /// <param name="culture">The culture used to determine the correct form of the word. If <c>null</c>, the current culture is used.</param>
    /// <returns>The plural category of the quantity.</returns>
    PluralCategory GetPluralCategory(decimal count, CultureInfo? culture = null);
}
