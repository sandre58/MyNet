// -----------------------------------------------------------------------
// <copyright file="TextSanitizationOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Text.Sanitization;

/// <summary>
/// Options for character-based text sanitization.
/// </summary>
public sealed record TextSanitizationOptions
{
    /// <summary>
    /// Gets a value indicating whether letters are kept.
    /// </summary>
    public bool KeepLetters { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether digits are kept.
    /// </summary>
    public bool KeepDigits { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether whitespace is kept.
    /// </summary>
    public bool KeepWhitespace { get; init; }

    /// <summary>
    /// Gets the additional characters that should be preserved.
    /// </summary>
    public string AdditionalAllowedCharacters { get; init; } = string.Empty;
}
