// -----------------------------------------------------------------------
// <copyright file="TextTruncationExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Primitives;
using MyNet.Text.Truncation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Text;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TextTruncationExtensions
{
    extension(TextPipeline pipeline)
    {
        /// <summary>
        /// Transforms a string using the provided truncation transform. Transformations are applied in the provided order.
        /// </summary>
        /// <param name="transform">The truncation transform to apply.</param>
        /// <returns>The transformed TextPipeline instance.</returns>
        public TextPipeline Truncate(ITextTruncator transform)
        {
            ArgumentNullException.ThrowIfNull(transform);
            return pipeline.Apply(transform);
        }

        /// <summary>
        /// Truncate the string.
        /// </summary>
        /// <param name="length">The maximum length of the string.</param>
        /// <param name="truncationString">The string to append when truncating.</param>
        /// <param name="direction">The direction from which to truncate.</param>
        /// <returns>The transformed TextPipeline instance.</returns>
        public TextPipeline Truncate(int length, string truncationString = "…", TruncateFrom direction = TruncateFrom.Right)
            => pipeline.Truncate(new FixedLengthTextTruncator(
                new()
                {
                    Length = length,
                    TruncationString = truncationString,
                    Direction = direction
                }));

        /// <summary>
        /// Truncate the string to a fixed number of words.
        /// </summary>
        /// <param name="count">The number of words to keep.</param>
        /// <param name="truncationString">The string to append when truncating.</param>
        /// <param name="direction">The direction from which to truncate.</param>
        /// <returns>The transformed TextPipeline instance.</returns>
        public TextPipeline TruncateWords(int count, string truncationString = "…", TruncateFrom direction = TruncateFrom.Right)
            => pipeline.Truncate(new FixedNumberOfWordsTextTruncator(
                new()
                {
                    Length = count,
                    TruncationString = truncationString,
                    Direction = direction
                }));

        /// <summary>
        /// Truncate the string to a fixed number of characters.
        /// </summary>
        /// <param name="count">The number of characters to keep.</param>
        /// <param name="truncationString">The string to append when truncating.</param>
        /// <param name="direction">The direction from which to truncate.</param>
        /// <returns>The transformed TextPipeline instance.</returns>
        public TextPipeline TruncateCharacters(int count, string truncationString = "…", TruncateFrom direction = TruncateFrom.Right)
            => pipeline.Truncate(new FixedNumberOfCharactersTextTruncator(
                new()
                {
                    Length = count,
                    TruncationString = truncationString,
                    Direction = direction
                }));
    }

    extension(string input)
    {
        /// <summary>
        /// Transforms a string using the provided truncation transform. Transformations are applied in the provided order.
        /// </summary>
        /// <param name="transform">The truncation transform to apply.</param>
        /// <param name="culture">The culture to use for the truncation.</param>
        /// <returns>The transformed string.</returns>
        public string Truncate(ITextTruncator transform, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(transform);
            return transform.Apply(input, culture.OrCurrent());
        }

        /// <summary>
        /// Truncate the string.
        /// </summary>
        /// <param name="length">The maximum length of the string.</param>
        /// <param name="truncationString">The string to append when truncating.</param>
        /// <param name="direction">The direction from which to truncate.</param>
        /// <returns>The transformed string.</returns>
        public string Truncate(int length, string truncationString = "…", TruncateFrom direction = TruncateFrom.Right)
            => input.Truncate(new FixedLengthTextTruncator(
                new()
                {
                    Length = length,
                    TruncationString = truncationString,
                    Direction = direction
                }));

        /// <summary>
        /// Truncate the string to a fixed number of words.
        /// </summary>
        /// <param name="count">The number of words to keep.</param>
        /// <param name="truncationString">The string to append when truncating.</param>
        /// <param name="direction">The direction from which to truncate.</param>
        /// <returns>The transformed string.</returns>
        public string TruncateWords(int count, string truncationString = "…", TruncateFrom direction = TruncateFrom.Right)
            => input.Truncate(new FixedNumberOfWordsTextTruncator(
                new()
                {
                    Length = count,
                    TruncationString = truncationString,
                    Direction = direction
                }));

        /// <summary>
        /// Truncate the string to a fixed number of characters.
        /// </summary>
        /// <param name="count">The number of characters to keep.</param>
        /// <param name="truncationString">The string to append when truncating.</param>
        /// <param name="direction">The direction from which to truncate.</param>
        /// <returns>The transformed string.</returns>
        public string TruncateCharacters(int count, string truncationString = "…", TruncateFrom direction = TruncateFrom.Right)
            => input.Truncate(new FixedNumberOfCharactersTextTruncator(
                new()
                {
                    Length = count,
                    TruncationString = truncationString,
                    Direction = direction
                }));
    }
}
