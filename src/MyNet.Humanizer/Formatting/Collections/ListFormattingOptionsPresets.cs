// -----------------------------------------------------------------------
// <copyright file="ListFormattingOptionsPresets.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Humanizer.Formatting.Collections;

/// <summary>
/// Common presets for list formatting without changing global defaults.
/// </summary>
public static class ListFormattingOptionsPresets
{
    /// <summary>
    /// Gets comma-separated list without conjunction ("a, b, c").
    /// </summary>
    public static ListFormattingOptions CommaSeparated { get; } = new()
    {
        Separator = ", ",
        Conjunction = ListConjunction.None,
        TrimItems = true,
        IgnoreNullOrWhiteSpace = true
    };

    /// <summary>
    /// Gets natural language list using conjunctions ("a, b and c").
    /// </summary>
    public static ListFormattingOptions NaturalLanguage { get; } = new()
    {
        Separator = ", ",
        Conjunction = ListConjunction.And,
        TrimItems = true,
        IgnoreNullOrWhiteSpace = true
    };
}
