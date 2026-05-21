// -----------------------------------------------------------------------
// <copyright file="ITextRandomGenerator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Text.Randomize;

/// <summary>
/// Interface for generating random text based on a format string, allowing for replacement of symbols and clamping of string lengths.
/// </summary>
public interface ITextRandomGenerator
{
    /// <summary>
    /// Generates a randomized string from the specified pattern.
    /// </summary>
    /// <remarks>
    /// <para>Supported pattern tokens:</para>
    /// <para>Basic tokens:</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Token</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>#</term>
    /// <description>Random digit (0-9).</description>
    /// </item>
    /// <item>
    /// <term>?</term>
    /// <description>Random uppercase letter (A-Z).</description>
    /// </item>
    /// <item>
    /// <term>a</term>
    /// <description>Random lowercase letter (a-z).</description>
    /// </item>
    /// <item>
    /// <term>*</term>
    /// <description>Random alphanumeric character.</description>
    /// </item>
    /// <item>
    /// <term>&amp;</term>
    /// <description>Random hexadecimal character.</description>
    /// </item>
    /// <item>
    /// <term>!</term>
    /// <description>Random printable ASCII character.</description>
    /// </item>
    /// </list>
    ///
    /// Repetition syntax:
    /// <code>
    /// {#5}    => 5 random digits
    /// {?3}    => 3 uppercase letters
    /// {*10}   => 10 alphanumeric characters
    /// </code>
    ///
    /// Custom character sets:
    /// <code>
    /// {[ABC]5}       => 5 characters from A, B or C
    /// {[0-9A-F]8}    => 8 hexadecimal characters
    /// </code>
    ///
    /// Examples:
    /// <code>
    /// INV-{#6}
    /// USR-{?3}-{#4}
    /// TOKEN-{&amp;32}
    /// </code>
    ///
    /// Use <c>\</c> to escape reserved characters.
    /// </remarks>
    /// <param name="pattern">The pattern to generate.</param>
    /// <returns>A randomized string matching the pattern.</returns>
    string Randomize(string pattern);
}
