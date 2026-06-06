// -----------------------------------------------------------------------
// <copyright file="TextFormattingExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using MyNet.Primitives;
using MyNet.Text.Formatting;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Text;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Provides text formatting extension methods.
/// </summary>
public static class TextFormattingExtensions
{
    extension(TextPipeline pipeline)
    {
        /// <summary>
        /// Converts the current text to initials.
        /// </summary>
        public TextPipeline Initials() => pipeline.Apply(Formatter.Initials);

        /// <summary>
        /// Converts key-like identifiers (PascalCase, camelCase, snake_case, kebab-case) to human-readable text.
        /// </summary>
        public TextPipeline HumanizeKey() => pipeline.Apply(Formatter.HumanizeKey);

        /// <summary>
        /// Applies an increment transformation to the current text using explicit options.
        /// </summary>
        /// <param name="options">Options used to configure increment behavior.</param>
        /// <returns>The TextPipeline instance with the applied transformation.</returns>
        public TextPipeline Increment(IncrementTransformOptions options) =>
            pipeline.Apply(new IncrementTransform(options));

        /// <summary>
        /// Applies an increment transformation to the current text by appending an incrementing number until a unique value is found.
        /// </summary>
        /// <param name="existStrings">The collection of existing strings to check against.</param>
        /// <param name="minIncrement">The minimum increment value. Current is 1.</param>
        /// <param name="step">The step value for incrementing. Current is 1.</param>
        /// <param name="format">Optional format string for the incrementing number.</param>
        /// <returns>The TextPipeline instance with the applied transformation.</returns>
        public TextPipeline Increment(IEnumerable<string> existStrings, int minIncrement = 1, int step = 1, string? format = null) =>
            pipeline.Increment(new()
            {
                ExistingStrings = existStrings,
                Kind = IncrementKind.Numeric,
                MinIncrement = minIncrement,
                Step = step,
                NumericFormat = format
            });

        /// <summary>
        /// Applies an alphabetic increment transformation to the current text by appending alphabetic suffixes until a unique value is found.
        /// </summary>
        /// <param name="existStrings">The collection of existing strings to check against.</param>
        /// <param name="minIncrement">The minimum increment value (1 for 'A', 2 for 'B', etc.). Current is 1.</param>
        /// <param name="step">The step value for incrementing. Current is 1.</param>
        /// <returns>The TextPipeline instance with the applied transformation.</returns>
        public TextPipeline IncrementAlpha(IEnumerable<string> existStrings, int minIncrement = 1, int step = 1) =>
            pipeline.Increment(new()
            {
                ExistingStrings = existStrings,
                Kind = IncrementKind.Alpha,
                MinIncrement = minIncrement,
                Step = step
            });
    }

    extension(string input)
    {
        /// <summary>
        /// Converts the string to initials.
        /// </summary>
        public string Initials(CultureInfo? culture = null) => input.Apply(culture.OrCurrent(), Formatter.Initials);

        /// <summary>
        /// Converts key-like identifiers (PascalCase, camelCase, snake_case, kebab-case) to human-readable text.
        /// </summary>
        public string HumanizeKey(CultureInfo? culture = null) => input.Apply(culture.OrCurrent(), Formatter.HumanizeKey);

        /// <summary>
        /// Applies an increment transformation using explicit options.
        /// </summary>
        /// <param name="options">Options used to configure increment behavior.</param>
        /// <param name="culture">Optional culture used for formatting numeric increments.</param>
        /// <returns>A unique incremented version of the input string.</returns>
        public string Increment(IncrementTransformOptions options, CultureInfo? culture = null) =>
            input.Apply(culture.OrCurrent(), new IncrementTransform(options));

        /// <summary>
        /// Applies an increment transformation by appending an incrementing number until a unique value is found.
        /// </summary>
        /// <param name="existStrings">The collection of existing strings to check against.</param>
        /// <param name="minIncrement">The minimum increment value. Current is 1.</param>
        /// <param name="step">The step value for incrementing. Current is 1.</param>
        /// <param name="format">Optional format string for the incrementing number.</param>
        /// <returns>A new string that is an incremented version of the input string.</returns>
        public string Increment(IEnumerable<string> existStrings, int minIncrement = 1, int step = 1, string? format = null) =>
            input.Increment(new IncrementTransformOptions
            {
                ExistingStrings = existStrings,
                Kind = IncrementKind.Numeric,
                MinIncrement = minIncrement,
                Step = step,
                NumericFormat = format
            });

        /// <summary>
        /// Applies an alphabetic increment transformation by appending alphabetic suffixes until a unique value is found.
        /// </summary>
        /// <param name="existStrings">The collection of existing strings to check against.</param>
        /// <param name="minIncrement">The minimum increment value (1 for 'A', 2 for 'B', etc.). Current is 1.</param>
        /// <param name="step">The step value for incrementing. Current is 1.</param>
        /// <returns>A unique incremented string with alphabetic suffix.</returns>
        public string IncrementAlpha(IEnumerable<string> existStrings, int minIncrement = 1, int step = 1) =>
            input.Increment(new IncrementTransformOptions
            {
                ExistingStrings = existStrings,
                Kind = IncrementKind.Alpha,
                MinIncrement = minIncrement,
                Step = step
            });
    }
}
