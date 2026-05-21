// -----------------------------------------------------------------------
// <copyright file="InflectionRule.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace MyNet.Globalization.Inflection;

/// <summary>
/// Represents a single inflection rule, consisting of a regular expression pattern and a replacement string. The rule can be applied to a word to transform it according to the specified pattern and replacement.
/// </summary>
/// <param name="Regex">The regular expression pattern to match.</param>
/// <param name="Replacement">The replacement string to use when the pattern matches.</param>
public sealed record InflectionRule(Regex Regex, string Replacement)
{
    /// <summary>
    /// Applies the inflection rule to the specified word. If the regular expression pattern matches the word, it returns the transformed word using the replacement string; otherwise, it returns null.
    /// </summary>
    /// <param name="word">The word to which the inflection rule will be applied.</param>
    /// <returns>The transformed word if the pattern matches; otherwise, null.</returns>
    public string? Apply(string word) => !Regex.IsMatch(word) ? null : Regex.Replace(word, Replacement);
}
