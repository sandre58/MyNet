// -----------------------------------------------------------------------
// <copyright file="InflectionRules.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Immutable;

namespace MyNet.Globalization.Inflection;

/// <summary>
/// Represents a collection of inflection rules, including pluralization rules, singularization rules, and uncountable words. This class is used to define the rules for transforming words based on their quantity and inflection patterns.
/// </summary>
public sealed class InflectionRules
{
    /// <summary>
    /// Gets an empty instance of <see cref="InflectionRules"/> with no pluralization rules, singularization rules, or uncountable words. This can be used as a default or placeholder when no specific inflection rules are defined.
    /// </summary>
    public static InflectionRules Empty { get; } = new()
    {
        Plurals = [],
        Singulars = [],
        Uncountables = []
    };

    /// <summary>
    /// Gets the collection of pluralization rules, which define how to transform singular words into their plural forms based on specific patterns and replacements.
    /// </summary>
    public required ImmutableArray<InflectionRule> Plurals { get; init; }

    /// <summary>
    /// Gets the collection of singularization rules, which define how to transform plural words into their singular forms based on specific patterns and replacements.
    /// </summary>
    public required ImmutableArray<InflectionRule> Singulars { get; init; }

    /// <summary>
    /// Gets the collection of uncountable words, which are words that do not have a distinct plural form.
    /// </summary>
    public required ImmutableHashSet<string> Uncountables { get; init; }
}
