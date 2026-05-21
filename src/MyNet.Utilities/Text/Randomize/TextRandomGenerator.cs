// -----------------------------------------------------------------------
// <copyright file="TextRandomGenerator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using MyNet.Utilities.Generator;
using MyNet.Utilities.Generator.Static;
using MyNet.Utilities.Helpers;

namespace MyNet.Utilities.Text.Randomize;

/// <summary>
/// Implements a random text generator that can replace specific symbols in a format string with random characters based on provided rules.
/// </summary>
/// <param name="random">The random generator used to generate random values.</param>
public sealed class TextRandomGenerator(IRandomGenerator random) : ITextRandomGenerator
{
    /// <summary>
    /// Gets the current instance of the <see cref="TextRandomGenerator"/> using the default random generator.
    /// </summary>
    public static TextRandomGenerator Current { get; } = new(RandomGenerator.Current);

    /// <inheritdoc/>
    public string Randomize(string pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);

        var builder = new StringBuilder(pattern.Length);

        for (var i = 0; i < pattern.Length; i++)
        {
            var c = pattern[i];

            switch (c)
            {
                case '\\' when i + 1 >= pattern.Length:
                    throw new FormatException("Invalid escape sequence.");

                case '\\':
                    builder.Append(pattern[++i]);
                    continue;

                case '{':
                    {
                        var end = pattern.IndexOf('}', i);

                        if (end < 0)
                            throw new FormatException("Missing closing brace.");

                        ParseComplexToken(pattern.AsSpan(i + 1, end - i - 1), builder);

                        i = end;
                        continue;
                    }

                default:
                    builder.Append(GenerateToken(c));
                    break;
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// Expands a character set defined in a token, supporting both individual characters and ranges (e.g., A-Z). The input is expected to be the content inside square brackets of a complex token.
    /// </summary>
    /// <param name="pattern">The character set pattern to expand.</param>
    /// <returns>An array of characters representing the expanded character set.</returns>
    /// <exception cref="FormatException">Thrown when the character set pattern is invalid.</exception>
    private static char[] ExpandCharacterSet(ReadOnlySpan<char> pattern)
    {
        var chars = new List<char>();

        for (var i = 0; i < pattern.Length; i++)
        {
            if (i + 2 < pattern.Length &&
                pattern[i + 1] == '-')
            {
                var start = pattern[i];
                var end = pattern[i + 2];

                if (start > end)
                    throw new FormatException("Invalid character range.");

                for (var c = start; c <= end; c++)
                    chars.Add(c);

                i += 2;
                continue;
            }

            chars.Add(pattern[i]);
        }

        return [.. chars];
    }

    /// <summary>
    /// Parses a complex token enclosed in braces and appends the generated characters to the provided StringBuilder. The token can specify repetition counts and character sets.
    /// </summary>
    /// <param name="token">The complex token to parse.</param>
    /// <param name="builder">The StringBuilder to append the generated characters to.</param>
    /// <exception cref="FormatException">Thrown when the token format is invalid.</exception>
    private void ParseComplexToken(ReadOnlySpan<char> token, StringBuilder builder)
    {
        if (token.IsEmpty)
            throw new FormatException("Empty token.");

        // {#5}
        if (token.Length >= 2 && char.IsDigit(token[^1]))
        {
            var countStart = token.Length - 1;

            while (countStart > 0 && char.IsDigit(token[countStart - 1]))
                countStart--;

            var countSpan = token[countStart..];

            if (!int.TryParse(countSpan, out var count))
                throw new FormatException("Invalid repetition count.");

            var valueSpan = token[..countStart];

            switch (valueSpan.Length)
            {
                // {[ABC]5}
                case >= 3 when
                    valueSpan[0] == '[' &&
                    valueSpan[^1] == ']':
                    {
                        var charset = ExpandCharacterSet(valueSpan[1..^1]);

                        for (var i = 0; i < count; i++)
                            builder.Append(charset[random.Int(0, charset.Length)]);

                        return;
                    }

                // {#5}
                case 1:
                    {
                        for (var i = 0; i < count; i++)
                            builder.Append(GenerateToken(valueSpan[0]));

                        return;
                    }

                default:
                    throw new FormatException($"Invalid token '{token}'.");
            }
        }

        // {ABC}
        foreach (var c in token)
            builder.Append(c);
    }

    /// <summary>
    /// Generates a random character based on the provided token. The token determines the type of character to generate:
    /// - '#' generates a random digit (0-9).
    /// - '?' generates a random uppercase letter (A-Z).
    /// - 'a' generates a random lowercase letter (a-z).
    /// - '*' generates a random alphanumeric character (0-9, A-Z).
    /// - '&amp;' generates a random hexadecimal character (0-9, A-F).
    /// - '!' generates a random ASCII character (from 33 to 126).
    /// </summary>
    /// <param name="token">The token representing the type of character to generate.</param>
    /// <returns>A randomly generated character based on the token.</returns>
    private char GenerateToken(char token)
        => token switch
        {
            '#' => (char)('0' + random.Int(0, 10)),
            '?' => (char)('A' + random.Int(0, 26)),
            'a' => (char)('a' + random.Int(0, 26)),
            '*' => random.Bool() ? (char)('0' + random.Int(0, 10)) : (char)('A' + random.Int(0, 26)),
            '&' => CharHelper.HexDigits[random.Int(0, 16)],
            '!' => (char)random.Int(33, 127),
            _ => token
        };
}
