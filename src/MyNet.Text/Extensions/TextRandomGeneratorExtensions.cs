// -----------------------------------------------------------------------
// <copyright file="TextRandomGeneratorExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Generator;
using MyNet.Generator.Facade;
using MyNet.Text.Randomize;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Text;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TextRandomGeneratorExtensions
{
    extension(string value)
    {
        /// <summary>
        /// Replaces characters in the string with random numbers based on the provided symbols. Each character in the string that matches any of the specified symbols will be replaced with a random number.
        /// </summary>
        /// <param name="random">The random generator used to generate random values.</param>
        /// <param name="symbols">The symbols to be replaced with random numbers.</param>
        /// <returns>A new string with the specified symbols replaced by random numbers.</returns>
        public string ReplaceSymbolsByNumbers(IRandomGenerator random, params char[] symbols) => value.ReplaceSymbols(random, symbols, _ => (char)('0' + random.Int(0, 10)));

        /// <summary>
        /// Replaces characters in the string with random numbers based on the provided symbols. Each character in the string that matches any of the specified symbols will be replaced with a random number.
        /// </summary>
        /// <param name="symbols">The symbols to be replaced with random numbers.</param>
        /// <returns>A new string with the specified symbols replaced by random numbers.</returns>
        public string ReplaceSymbolsByNumbers(params char[] symbols) => value.ReplaceSymbolsByNumbers(RandomGenerator.Current, symbols);

        /// <summary>
        /// Replaces characters in the string with random letters based on the provided symbols. Each character in the string that matches any of the specified symbols will be replaced with a random letter.
        /// </summary>
        /// <param name="random">The random generator used to generate random values.</param>
        /// <param name="symbols">The symbols to be replaced with random letters.</param>
        /// <param name="func">A function that defines how each symbol is replaced with a random letter.</param>
        /// <returns>A new string with the specified symbols replaced by random letters.</returns>
        public string ReplaceSymbols(IRandomGenerator random, char[] symbols, Func<char, char> func)
        {
            ArgumentNullException.ThrowIfNull(symbols);
            ArgumentNullException.ThrowIfNull(func);

            var buffer = value.Length <= 256
                ? stackalloc char[value.Length]
                : new char[value.Length];

            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                buffer[i] = symbols.AsSpan().Contains(c)
                    ? func(c)
                    : c;
            }

            return new(buffer);
        }

        /// <summary>
        /// Replaces characters in the string with random letters based on the provided symbols. Each character in the string that matches any of the specified symbols will be replaced with a random letter.
        /// </summary>
        /// <param name="symbols">The symbols to be replaced with random letters.</param>
        /// <param name="func">A function that defines how each symbol is replaced with a random letter.</param>
        /// <returns>A new string with the specified symbols replaced by random letters.</returns>
        public string ReplaceSymbols(char[] symbols, Func<char, char> func) => value.ReplaceSymbols(RandomGenerator.Current, symbols, func);

        /// <summary>
        /// Adjusts the length of the string to fit within the specified minimum and maximum bounds. If the string is shorter than the minimum length, it will be padded with filler characters. If it is longer than the maximum length, it will be truncated. The method ensures that the resulting string's length is between the specified minimum and maximum values.
        /// </summary>
        /// <param name="random">The random generator used to generate random values.</param>
        /// <param name="min">The minimum length of the string.</param>
        /// <param name="max">The maximum length of the string.</param>
        /// <returns>The adjusted string.</returns>
        public string FitLength(ITextRandomGenerator random, int? min = null, int? max = null)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (min is < 0 || max is < 0)
                throw new ArgumentOutOfRangeException(min is < 0 ? nameof(min) : nameof(max), "min/max cannot be negative.");

            if (min > max)
                throw new ArgumentOutOfRangeException(nameof(min), "min cannot be greater than max.");

            var result = value;
            if (max is not null && result.Length > max)
                result = result[..max.Value];

            if (min is null || min <= result.Length)
                return result;

            var missingChars = min.Value - result.Length;
            var fillerChars = string.Empty.PadRight(missingChars, '?').RandomizePattern(random);
            return result + fillerChars;
        }

        /// <summary>
        /// Adjusts the length of the string to fit within the specified minimum and maximum bounds. If the string is shorter than the minimum length, it will be padded with filler characters. If it is longer than the maximum length, it will be truncated. The method ensures that the resulting string's length is between the specified minimum and maximum values.
        /// </summary>
        /// <param name="min">The minimum length of the string.</param>
        /// <param name="max">The maximum length of the string.</param>
        /// <returns>The adjusted string.</returns>
        public string FitLength(int? min = null, int? max = null) => value.FitLength(TextRandomGenerator.Current, min, max);

        /// <summary>
        /// Randomizes the format of the string based on predefined rules.
        /// </summary>
        /// <param name="random">The random generator used to generate random values.</param>
        /// <returns>A new string with the format randomized.</returns>
        public string RandomizePattern(ITextRandomGenerator random) => random.Randomize(value);

        /// <summary>
        /// Randomizes the format of the string based on predefined rules.
        /// </summary>
        /// <returns>A new string with the format randomized.</returns>
        public string RandomizePattern() => value.RandomizePattern(TextRandomGenerator.Current);
    }
}
