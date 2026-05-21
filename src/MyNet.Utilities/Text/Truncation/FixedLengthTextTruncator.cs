// -----------------------------------------------------------------------
// <copyright file="FixedLengthTextTruncator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Utilities.Text.Truncation;

/// <summary>
/// Truncates a string to a fixed length, optionally adding a truncation string (e.g. ellipsis) at the truncation point.
/// </summary>
/// <param name="options">The options to use for truncating text.</param>
public sealed class FixedLengthTextTruncator(TextTruncationOptions options) : TextTruncatorBase(options)
{
    /// <inheritdoc/>
    protected override string ApplyCore(string input, CultureInfo culture)
    {
        if (string.IsNullOrEmpty(input) || input.Length <= Options.Length)
            return input;

        if (Options.TruncationString.Length >= Options.Length)
        {
            return Options.Direction == TruncateFrom.Right ? input[..Options.Length] : input[^Options.Length..];
        }

        var contentLength = Options.Length - Options.TruncationString.Length;

        return Options.Direction switch
        {
            TruncateFrom.Left => TruncateLeft(input, input.Length - contentLength),

            _ => TruncateRight(input, contentLength)
        };
    }
}
