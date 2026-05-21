// -----------------------------------------------------------------------
// <copyright file="ListFormattingOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Humanizer.Formatting.Collections;

/// <summary>
/// Options for list formatting.
/// </summary>
public sealed class ListFormattingOptions
{
    /// <summary>
    /// Gets the default list formatting options.
    /// </summary>
    public static ListFormattingOptions Default { get; } = new();

    /// <summary>
    /// Gets the separator to use between items in the list. The default is an empty string, which means that items will be concatenated without any separator. You can specify a custom separator, such as ", " for a comma and space, or " | " for a pipe character with spaces.
    /// </summary>
    public string Separator { get; init; } = ", ";

    /// <summary>
    /// Gets a value indicating whether to use the Oxford comma.
    /// </summary>
    public bool UseOxfordComma { get; init; }

    /// <summary>
    /// Gets a value indicating whether to trim items in the list. If true, leading and trailing whitespace will be removed from each item before formatting the list.
    /// </summary>
    public bool TrimItems { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether to ignore null or whitespace items in the list. If true, any items that are null, empty, or consist only of whitespace will be excluded from the formatted list.
    /// </summary>
    public bool IgnoreNullOrWhiteSpace { get; init; } = true;

    /// <summary>
    /// Gets the conjunction to use when formatting the list.
    /// </summary>
    public ListConjunction Conjunction { get; init; } = ListConjunction.And;
}
