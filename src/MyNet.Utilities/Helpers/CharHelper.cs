// -----------------------------------------------------------------------
// <copyright file="CharHelper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Helpers;

/// <summary>
/// Provides helper methods and properties for working with characters.
/// </summary>
public static class CharHelper
{
    /// <summary>
    /// Gets the alphabet as a read-only span of characters.
    /// </summary>
    /// <returns>A read-only span of characters representing the alphabet.</returns>
    public static ReadOnlySpan<char> Alphabet => "abcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// Gets the numbers as a read-only span of characters.
    /// </summary>
    /// <returns>A read-only span of characters representing the numbers.</returns>
    public static ReadOnlySpan<char> Numbers => "0123456789";

    /// <summary>
    /// Gets the numbers as a read-only span of characters.
    /// </summary>
    /// <returns>A read-only span of characters representing the hexadecimal digits.</returns>
    public static ReadOnlySpan<char> HexDigits => "0123456789ABCDEF";

    /// <summary>
    /// Gets the alphabet and numbers as a read-only span of characters.
    /// </summary>
    /// <returns>A read-only span of characters representing the alphabet and numbers.</returns>
    public static ReadOnlySpan<char> AlphaNumbers => "abcdefghijklmnopqrstuvwxyz0123456789";
}
