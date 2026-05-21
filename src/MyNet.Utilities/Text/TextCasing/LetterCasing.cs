// -----------------------------------------------------------------------
// <copyright file="LetterCasing.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Text.TextCasing;

/// <summary>
/// Options for specifying the desired letter casing for the output string.
/// </summary>
public enum LetterCasing
{
    /// <summary>
    /// SoMeStrIng -> SoMeStrIng.
    /// </summary>
    Normal,

    /// <summary>
    /// SomeString -> Some String.
    /// </summary>
    Title,

    /// <summary>
    /// SomeString -> SOME STRING.
    /// </summary>
    Upper,

    /// <summary>
    /// SomeString -> some string.
    /// </summary>
    Lower,

    /// <summary>
    /// SomeString -> Some string.
    /// </summary>
    Sentence,

    /// <summary>
    /// SomeString -> SomeString.
    /// </summary>
    Pascal,

    /// <summary>
    /// SomeString -> someString.
    /// </summary>
    Camel,

    /// <summary>
    /// SomeString -> some-string.
    /// </summary>
    Snake,

    /// <summary>
    /// SomeString -> some-string.
    /// </summary>
    Kebab
}
