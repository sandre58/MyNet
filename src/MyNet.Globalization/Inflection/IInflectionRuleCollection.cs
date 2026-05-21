// -----------------------------------------------------------------------
// <copyright file="IInflectionRuleCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace MyNet.Globalization.Inflection;

/// <summary>
/// Defines methods for managing inflection rules, including adding irregular, uncountable, plural, and singular rules.
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "The name 'IInflectionRuleCollection' accurately describes the purpose of the interface as a collection of inflection rules.")]
public interface IInflectionRuleCollection
{
    /// <summary>
    /// Adds an irregular inflection rule for a word.
    /// </summary>
    /// <param name="singular">The singular form of the word.</param>
    /// <param name="plural">The plural form of the word.</param>
    /// <param name="matchEnding">Indicates whether to match the ending of the word.</param>
    void AddIrregular(string singular, string plural, bool matchEnding = true);

    /// <summary>
    /// Adds an uncountable inflection rule for a word.
    /// </summary>
    /// <param name="word">The word to be marked as uncountable.</param>
    void AddUncountable(string word);

    /// <summary>
    /// Adds a plural inflection pattern for a word.
    /// </summary>
    /// <param name="pattern">The pattern to apply for pluralization.</param>
    /// <param name="replacement">The replacement string for the plural form.</param>
    void AddPluralRule(string pattern, string replacement);

    /// <summary>
    /// Adds a singular inflection pattern for a word.
    /// </summary>
    /// <param name="pattern">The pattern to apply for singularization.</param>
    /// <param name="replacement">The replacement string for the singular form.</param>
    void AddSingularRule(string pattern, string replacement);
}
