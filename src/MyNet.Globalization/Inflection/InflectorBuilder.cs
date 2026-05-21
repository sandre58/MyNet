// -----------------------------------------------------------------------
// <copyright file="InflectorBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MyNet.Globalization.Inflection;

/// <summary>
/// Builder class for constructing an instance of the <see cref="Inflector"/> class. This builder allows for the addition of pluralization and singularization rules, irregular inflection rules, and uncountable words. It uses a fluent interface to enable chaining of method calls for adding rules and building the inflector. The builder takes a <see cref="CultureInfo"/> object as a parameter to ensure that the inflection rules are applied according to the specific linguistic and cultural conventions of the language being targeted.
/// </summary>
public sealed class InflectorBuilder
{
    private readonly List<InflectionRule> _plurals = [];
    private readonly List<InflectionRule> _singulars = [];
    private readonly HashSet<string> _uncountables;

    /// <summary>
    /// Initializes a new instance of the <see cref="InflectorBuilder"/> class with the specified culture. The culture is used to ensure that the inflection rules are applied according to the linguistic and cultural conventions of the language being targeted. The constructor also initializes the internal collections for pluralization rules, singularization rules, and uncountable words, using a case-insensitive string comparer based on the provided culture for the uncountables collection.
    /// </summary>
    /// <param name="culture">The culture information to use for inflection rules.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided culture is null.</exception>
    public InflectorBuilder(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        Culture = culture;

        _uncountables = new(StringComparer.Create(culture, ignoreCase: true));
    }

    /// <summary>
    /// Gets the culture associated with this inflector builder. The culture is used to ensure that the inflection rules are applied according to the linguistic and cultural conventions of the language being targeted. This property is read-only and is set through the constructor when creating an instance of the <see cref="InflectorBuilder"/> class.
    /// </summary>
    public CultureInfo Culture { get; }

    /// <summary>
    /// Adds a pluralization rule to the inflector. The rule consists of a regular expression pattern and a replacement string. When the inflector processes a word, it will apply this rule to transform the word into its plural form if the pattern matches. The pattern is case-insensitive and compiled for performance, and it should be designed to match the appropriate parts of the word for pluralization.
    /// </summary>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="replacement">The replacement string to use when the pattern matches.</param>
    /// <exception cref="ArgumentException">Thrown when the provided pattern is null or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the provided replacement is null.</exception>
    public InflectorBuilder AddPluralRule(string pattern, string replacement)
    {
        ArgumentException.ThrowIfNullOrEmpty(pattern);
        ArgumentNullException.ThrowIfNull(replacement);

        _plurals.Add(CreateRule(pattern, replacement));

        return this;
    }

    /// <summary>
    /// Adds a singularization rule to the inflector. The rule consists of a regular expression pattern and a replacement string. When the inflector processes a word, it will apply this rule to transform the word into its singular form if the pattern matches. The pattern is case-insensitive and compiled for performance, and it should be designed to match the appropriate parts of the word for singularization.
    /// </summary>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="replacement">The replacement string to use when the pattern matches.</param>
    /// <exception cref="ArgumentException">Thrown when the provided pattern is null or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the provided replacement is null.</exception>
    public InflectorBuilder AddSingularRule(string pattern, string replacement)
    {
        ArgumentException.ThrowIfNullOrEmpty(pattern);
        ArgumentNullException.ThrowIfNull(replacement);

        _singulars.Add(CreateRule(pattern, replacement));

        return this;
    }

    /// <summary>
    /// Adds an irregular inflection rule for a word, specifying both its singular and plural forms. This method allows for the addition of exceptions to regular pluralization and singularization rules, which is common in many languages. The `matchEnding` parameter determines whether the rule should match the word on its own as well as at the end of longer words. If `matchEnding` is true, the method constructs regular expression patterns that match the singular and plural forms at the end of words, allowing for inflection of compound words. If `matchEnding` is false, it constructs patterns that match only the exact singular and plural forms.
    /// </summary>
    /// <param name="singular">The singular form of the word.</param>
    /// <param name="plural">The plural form of the word.</param>
    /// <param name="matchEnding">Indicates whether the rule should match the word at the end of longer words.</param>
    /// <exception cref="ArgumentException">Thrown when the provided singular or plural form is null or empty.</exception>
    public InflectorBuilder AddIrregular(string singular, string plural, bool matchEnding = true)
    {
        ArgumentException.ThrowIfNullOrEmpty(singular);
        ArgumentException.ThrowIfNullOrEmpty(plural);

        if (matchEnding)
        {
            AddPluralRule($"({singular[0]}){singular[1..]}$", $"$1{plural[1..]}");
            AddSingularRule($"({plural[0]}){plural[1..]}$", $"$1{singular[1..]}");
        }
        else
        {
            AddPluralRule($"^{singular}$", plural);
            AddSingularRule($"^{plural}$", singular);
        }

        return this;
    }

    /// <summary>
    /// Adds an uncountable word to the inflector. Uncountable words are those that do not have a plural form and should be treated as the same in both singular and plural contexts. When the inflector processes a word, it will check if the word is in the list of uncountables and, if so, it will return the word unchanged regardless of whether it is being pluralized or singularized. This method ensures that uncountable words are handled correctly according to the rules of the language.
    /// </summary>
    /// <param name="word">The uncountable word to add.</param>
    /// <exception cref="ArgumentException">Thrown when the provided word is null or empty.</exception>
    public InflectorBuilder AddUncountable(string word)
    {
        ArgumentException.ThrowIfNullOrEmpty(word);

        _uncountables.Add(word);

        return this;
    }

    /// <summary>
    /// Builds and returns an instance of the <see cref="Inflector"/> class based on the rules that have been added to the builder. This method compiles the inflection rules into a format that can be efficiently applied when pluralizing or singularizing words. The resulting inflector will use the specified culture and the defined rules to perform inflection operations according to the language's grammar and conventions.
    /// </summary>
    /// <returns>An instance of the <see cref="Inflector"/> class.</returns>
    public Inflector Build() => new(Culture, new()
    {
        Plurals = [.._plurals],
        Singulars = [.._singulars],
        Uncountables = _uncountables.ToImmutableHashSet(_uncountables.Comparer)
    });

    /// <summary>
    /// Creates an inflection rule based on the provided regular expression pattern and replacement string. The method compiles the regular expression for performance and sets it to be case-insensitive and culture-invariant. This allows the inflector to efficiently apply the rule when processing words, ensuring that the correct transformations are made according to the defined patterns.
    /// </summary>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="replacement">The replacement string to use when the pattern matches.</param>
    /// <returns>An inflection rule based on the provided pattern and replacement.</returns>
    private static InflectionRule CreateRule(string pattern, string replacement)
        => new(
            new(pattern, RegexOptions.None | RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant),
            replacement);
}
