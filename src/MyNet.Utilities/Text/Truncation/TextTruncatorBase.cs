// -----------------------------------------------------------------------
// <copyright file="TextTruncatorBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;

namespace MyNet.Utilities.Text.Truncation;

/// <summary>
/// Base class for text truncation, providing common functionality for truncating strings based on specified options.
/// </summary>
/// <param name="options">The options to use for truncating text.</param>
public abstract class TextTruncatorBase(TextTruncationOptions options) : ITextTruncator
{
    /// <summary>
    /// Gets the options used for truncating text.
    /// </summary>
    protected TextTruncationOptions Options { get; } = options;

    /// <summary>
    /// Truncate the input string based on the specified options and culture.
    /// </summary>
    /// <param name="input">The input string to truncate.</param>
    /// <param name="culture">The culture to use for truncation.</param>
    /// <returns>The truncated string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input string is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified length is less than zero.</exception>
    public string Apply(string input, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(culture);

        return ApplyCore(input, culture);
    }

    /// <summary>
    /// When overridden in a derived class, applies the truncation logic to the input string based on the specified culture.
    /// </summary>
    /// <param name="input">The input string to truncate.</param>
    /// <param name="culture">The culture to use for truncation.</param>
    /// <returns>The truncated string.</returns>
    protected abstract string ApplyCore(string input, CultureInfo culture);

    /// <summary>
    /// Truncates the input string from the left, starting at the specified index, and prepending the truncation string defined in the options.
    /// </summary>
    /// <param name="value">The input string to truncate.</param>
    /// <param name="startIndex">The index at which to start truncation.</param>
    /// <returns>The truncated string.</returns>
    protected string TruncateLeft(string value, int startIndex)
        => string.Concat(Options.TruncationString, value.AsSpan(startIndex));

    /// <summary>
    /// Truncates the input string from the right, ending at the specified index, and appending the truncation string defined in the options.
    /// </summary>
    /// <param name="value">The input string to truncate.</param>
    /// <param name="endIndex">The index at which to end truncation.</param>
    /// <returns>The truncated string.</returns>
    protected string TruncateRight(string value, int endIndex)
        => string.Concat(value.AsSpan(0, endIndex), Options.TruncationString);
}
