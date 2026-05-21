// -----------------------------------------------------------------------
// <copyright file="Truncator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Text.Truncation;

/// <summary>
/// Provides predefined text truncators for common truncation scenarios, such as truncating to a fixed length, a fixed number of characters, or a fixed number of words.
/// </summary>
public static class Truncator
{
    /// <summary>
    /// Gets or sets a truncator that truncates text to a fixed length, optionally adding a truncation string (e.g. ellipsis) at the truncation point.
    /// </summary>
    public static FixedLengthTextTruncator FixedLength { get; set; } = new(new() { Length = 100, TruncationString = "..." });

    /// <summary>
    /// Gets or sets a truncator that truncates text to a fixed number of characters, optionally adding a truncation string (e.g. ellipsis) at the truncation point.
    /// </summary>
    public static FixedNumberOfCharactersTextTruncator FixedNumberOfCharacters { get; set; } = new(new() { Length = 100, TruncationString = "..." });

    /// <summary>
    /// Gets or sets a truncator that truncates text to a fixed number of words, optionally adding a truncation string (e.g. ellipsis) at the truncation point.
    /// </summary>
    public static FixedNumberOfWordsTextTruncator FixedNumberOfWords { get; set; } = new(new() { Length = 100, TruncationString = "..." });
}
