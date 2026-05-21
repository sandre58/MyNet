// -----------------------------------------------------------------------
// <copyright file="Inflector.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Immutable;
using System.Globalization;

namespace MyNet.Globalization.Inflection;

/// <summary>
/// Represents an immutable implementation of the IInflector interface, which provides methods for pluralizing and singularizing words based on a set of inflection rules. This class is designed to be thread-safe and can be used in concurrent scenarios without the need for synchronization, as its state cannot be modified after it has been created.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Inflector"/> class with the specified culture and inflection rules. The constructor ensures that the provided culture is not null and assigns the inflection rules to the corresponding fields. This allows for the creation of an inflector that can handle inflection operations based on the specified rules and cultural context.
/// </remarks>
/// <param name="culture">The culture to use for inflection operations.</param>
/// <param name="rules">The inflection rules to apply.</param>
public class Inflector(CultureInfo culture, InflectionRules rules) : IInflector
{
    /// <summary>
    /// Creates a new instance of the <see cref="Inflector"/> class with the specified culture. This factory method provides a convenient way to create an inflector for a specific culture, allowing the caller to easily obtain an inflector that is configured with the appropriate inflection rules for that culture. The method initializes the inflector with the given culture and the corresponding inflection rules, ensuring that the resulting inflector is ready to perform pluralization and singularization operations based on the linguistic conventions of the specified culture.
    /// </summary>
    /// <param name="culture">The culture to use for inflection operations.</param>
    /// <returns>An instance of the <see cref="InflectorBuilder"/> class.</returns>
    public static InflectorBuilder Create(CultureInfo culture) => new(culture);

    /// <summary>
    /// Gets the culture associated with this inflector, which is used for inflection operations. The culture is set during the construction of the inflector and cannot be changed afterward, ensuring that the inflector's behavior remains consistent with the specified cultural context throughout its lifetime.
    /// </summary>
    public CultureInfo Culture { get; } = culture;

    /// <summary>
    /// Returns the plural form of the specified word based on the inflection rules of the language.
    /// Uncountable words are returned unchanged.
    /// Rules are evaluated last-added-first, so more specific rules (irregulars) added after general rules take priority.
    /// </summary>
    /// <param name="word">The word to pluralize.</param>
    /// <returns>The plural form of the word, or the original word if no rules match or the word is uncountable.</returns>
    /// <exception cref="ArgumentException">Thrown when the word is null or empty.</exception>
    public virtual string Pluralize(string word)
    {
        ArgumentException.ThrowIfNullOrEmpty(word);

        return ApplyFirstMatchingRule(rules.Plurals, word);
    }

    /// <summary>
    /// Returns the singular form of the specified word based on the inflection rules of the language.
    /// Uncountable words are returned unchanged.
    /// Rules are evaluated last-added-first, so more specific rules (irregulars) added after general rules take priority.
    /// </summary>
    /// <param name="word">The word to singularize.</param>
    /// <returns>The singular form of the word, or the original word if no rules match or the word is uncountable.</returns>
    /// <exception cref="ArgumentException">Thrown when the word is null or empty.</exception>
    public virtual string Singularize(string word)
    {
        ArgumentException.ThrowIfNullOrEmpty(word);

        return ApplyFirstMatchingRule(rules.Singulars, word);
    }

    /// <summary>
    /// Gets a value indicating whether quantities in the <see cref="PluralCategory.Zero"/> category should use the plural form.
    /// </summary>
    protected virtual bool UsePluralForZero => true;

    /// <summary>
    /// Returns whether the specified quantity should use the plural form for this language.
    /// Current behavior maps categories explicitly and keeps zero configurable via <see cref="UsePluralForZero"/>.
    /// </summary>
    /// <param name="count">The quantity to evaluate.</param>
    /// <returns><c>true</c> when the plural form should be used; otherwise <c>false</c>.</returns>
    public virtual bool IsPlural(decimal count)
        => GetPluralCategory(count) switch
        {
            PluralCategory.Singular => false,
            PluralCategory.Zero => UsePluralForZero,
            _ => true
        };

    /// <summary>
    /// Returns the plural category of the specified quantity.
    /// <list type="bullet">
    ///   <item><c>0</c> → <see cref="PluralCategory.Zero"/> (treated as plural in English; language-dependent in others)</item>
    ///   <item><c>1</c> → <see cref="PluralCategory.Singular"/></item>
    ///   <item>anything else → <see cref="PluralCategory.Other"/></item>
    /// </list>
    /// Override in language-specific subclasses to support <see cref="PluralCategory.Dual"/>, <see cref="PluralCategory.Few"/>, or <see cref="PluralCategory.Many"/>.
    /// </summary>
    /// <param name="count">The quantity to evaluate.</param>
    /// <returns>The plural category corresponding to the specified quantity.</returns>
    public virtual PluralCategory GetPluralCategory(decimal count)
        => Math.Abs(count) switch
        {
            0 => PluralCategory.Zero,
            1 => PluralCategory.Singular,
            _ => PluralCategory.Other
        };

    /// <summary>
    /// Applies the first matching inflection rule from the provided list of rules to the given word. The method iterates through the rules in reverse order (from most recently added to least recently added) and applies each rule to the word. If a rule matches and produces a non-empty result, that result is returned immediately. If no rules match, the original word is returned as a fallback. This approach allows for prioritizing more recently added rules over older ones, which can be important when dealing with exceptions or irregular inflection patterns.
    /// </summary>
    /// <param name="inflectionRules">The list of inflection rules to apply.</param>
    /// <param name="word">The word to which the rules should be applied.</param>
    /// <returns>The result of applying the first matching rule, or the original word if no rules match.</returns>
    protected virtual string ApplyFirstMatchingRule(ImmutableArray<InflectionRule> inflectionRules, string word)
    {
        if (IsUncountable(word))
            return word;

        for (var i = inflectionRules.Length - 1; i >= 0; i--)
        {
            var result = inflectionRules[i].Apply(word);

            if (!string.IsNullOrEmpty(result))
                return result;
        }

        return word;
    }

    /// <summary>
    /// Determines whether the specified word is uncountable, meaning that it does not have distinct singular and plural forms. The method checks if the word exists in the set of uncountable words, which is typically defined based on the language's grammar rules. If the word is found in the uncountable set, it returns true; otherwise, it returns false. This check is important to ensure that inflection rules are not applied to words that should remain unchanged regardless of quantity (e.g., "information", "rice").
    /// </summary>
    /// <param name="word">The word to check for uncountability.</param>
    /// <returns>True if the word is uncountable; otherwise, false.</returns>
    protected virtual bool IsUncountable(string word) => rules.Uncountables.Contains(word);
}
