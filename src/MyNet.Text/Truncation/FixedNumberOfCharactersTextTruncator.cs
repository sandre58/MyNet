// -----------------------------------------------------------------------
// <copyright file="FixedNumberOfCharactersTextTruncator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using System.Linq;

namespace MyNet.Text.Truncation;

public sealed class FixedNumberOfCharactersTextTruncator(TextTruncationOptions options) : TextTruncatorBase(options)
{
    /// <summary>
    /// Counts the number of alphanumeric characters in the given string.
    /// </summary>
    private static int CountAlphaNumeric(string value) => value.Count(char.IsLetterOrDigit);

    /// <inheritdoc/>
    protected override string ApplyCore(string input, CultureInfo culture)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var totalAlphaNumeric = CountAlphaNumeric(input);

        return totalAlphaNumeric <= Options.Length
            ? input
            : Options.TruncationString.Length >= Options.Length
                ? Options.Direction == TruncateFrom.Right
                    ? input[..Options.Length]
                    : input[^Options.Length..]
                : Options.Direction switch
                {
                    TruncateFrom.Left => TruncateFromLeft(input),
                    _ => TruncateFromRight(input)
                };
    }

    /// <summary>
    /// Truncate the string from the right, starting from the end of the string and moving towards the start until the desired length is reached.
    /// </summary>
    /// <param name="input">The input string to truncate.</param>
    /// <returns>The truncated string.</returns>
    private string TruncateFromRight(string input)
    {
        var processed = 0;

        for (var i = 0; i < input.Length; i++)
        {
            if (char.IsLetterOrDigit(input[i]))
            {
                processed++;
            }

            if (processed + Options.TruncationString.Length >= Options.Length)
            {
                return TruncateRight(input, i + 1);
            }
        }

        return input;
    }

    /// <summary>
    /// Truncate the string from the left, starting from the end of the string and moving towards the start until the desired length is reached.
    /// </summary>
    /// <param name="input">The input string to truncate.</param>
    /// <returns>The truncated string.</returns>
    private string TruncateFromLeft(string input)
    {
        var processed = 0;

        for (var i = input.Length - 1; i >= 0; i--)
        {
            if (char.IsLetterOrDigit(input[i]))
            {
                processed++;
            }

            if (processed + Options.TruncationString.Length >= Options.Length)
            {
                return TruncateLeft(input, i);
            }
        }

        return input;
    }
}
