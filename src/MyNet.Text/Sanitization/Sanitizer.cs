// -----------------------------------------------------------------------
// <copyright file="Sanitizer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Text.Sanitization;

/// <summary>
/// Provides common text sanitizer transforms.
/// </summary>
public static class Sanitizer
{
    /// <summary>
    /// Gets a sanitizer suitable for file names.
    /// </summary>
    public static ITextSanitizerTransform FileName { get; } = new FileNameTextSanitizer();

    /// <summary>
    /// Gets a sanitizer that keeps letters and digits only.
    /// </summary>
    public static ITextSanitizerTransform AlphaNumeric { get; } = new CharacterFilterTextSanitizer(new());

    /// <summary>
    /// Gets a sanitizer for technical identifiers (letters, digits and underscore).
    /// </summary>
    public static ITextSanitizerTransform Identifier { get; } = new CharacterFilterTextSanitizer(new()
    {
        KeepLetters = true,
        KeepDigits = true,
        KeepWhitespace = false,
        AdditionalAllowedCharacters = "_"
    });

    /// <summary>
    /// Gets a sanitizer for URL segments (letters, digits, hyphen and underscore).
    /// </summary>
    public static ITextSanitizerTransform UrlSegment { get; } = new CharacterFilterTextSanitizer(new()
    {
        KeepLetters = true,
        KeepDigits = true,
        KeepWhitespace = false,
        AdditionalAllowedCharacters = "-_"
    });
}
