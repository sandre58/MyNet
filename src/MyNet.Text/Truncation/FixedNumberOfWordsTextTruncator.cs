// -----------------------------------------------------------------------
// <copyright file="FixedNumberOfWordsTextTruncator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Text.Truncation;

/// <summary>
/// Truncates a string to a fixed number of words, either from the left or the right, depending on the specified truncation direction.
/// </summary>
/// <param name="options">The options for truncation, including the number of words and the truncation direction.</param>
public sealed class FixedNumberOfWordsTextTruncator(TextTruncationOptions options)
    : TextTruncatorBase(options)
{
    /// <summary>
    /// Counts the number of words in the input string by iterating through each character and determining when a new word starts based on whitespace characters.
    /// </summary>
    /// <param name="value">The input string to count words in.</param>
    /// <returns>The number of words in the input string.</returns>
    private static int CountWords(string value)
    {
        var count = 0;
        var insideWord = false;

        foreach (var c in value)
        {
            if (char.IsWhiteSpace(c))
            {
                insideWord = false;
                continue;
            }

            if (!insideWord)
            {
                insideWord = true;
                count++;
            }
        }

        return count;
    }

    /// <inheritdoc/>
    protected override string ApplyCore(string input, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var wordCount = CountWords(input);

        return wordCount <= Options.Length
            ? input
            : Options.Direction switch
            {
                TruncateFrom.Left => TruncateFromLeft(input),
                _ => TruncateFromRight(input)
            };
    }

    /// <summary>
    /// Truncates the input string from the right, keeping only the specified number of words from the left.
    /// </summary>
    /// <param name="input">The input string to truncate.</param>
    /// <returns>The truncated string.</returns>
    private string TruncateFromRight(string input)
    {
        var words = 0;
        var insideWord = false;

        for (var i = 0; i < input.Length; i++)
        {
            var isWhiteSpace = char.IsWhiteSpace(input[i]);

            switch (isWhiteSpace)
            {
                case false when !insideWord:
                    insideWord = true;
                    words++;
                    break;
                case true:
                    insideWord = false;
                    break;
            }

            if (words > Options.Length)
            {
                return TruncateRight(input, i - 1);
            }
        }

        return input;
    }

    /// <summary>
    /// Truncates the input string from the left, keeping only the specified number of words from the right.
    /// </summary>
    /// <param name="input">The input string to truncate.</param>
    /// <returns>The truncated string.</returns>
    private string TruncateFromLeft(string input)
    {
        var words = 0;
        var insideWord = false;

        for (var i = input.Length - 1; i >= 0; i--)
        {
            var isWhiteSpace = char.IsWhiteSpace(input[i]);

            switch (isWhiteSpace)
            {
                case false when !insideWord:
                    insideWord = true;
                    words++;
                    break;
                case true:
                    insideWord = false;
                    break;
            }

            if (words > Options.Length)
            {
                var startIndex = i + 1;

                while (startIndex < input.Length && char.IsWhiteSpace(input[startIndex]))
                {
                    startIndex++;
                }

                return string.Concat(Options.TruncationString, input[startIndex..].TrimEnd());
            }
        }

        return input;
    }
}
