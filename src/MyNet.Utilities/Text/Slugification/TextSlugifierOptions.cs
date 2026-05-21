// -----------------------------------------------------------------------
// <copyright file="TextSlugifierOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Text.Slugification;

/// <summary>
/// Options for slugification.
/// </summary>
public sealed record TextSlugifierOptions
{
    /// <summary>
    /// Gets the separator used between slug tokens.
    /// </summary>
    public char Separator { get; init; } = '-';

    /// <summary>
    /// Gets a value indicating whether the output is lower-cased.
    /// </summary>
    public bool Lowercase { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether diacritics are removed.
    /// </summary>
    public bool RemoveDiacritics { get; init; } = true;
}
