// -----------------------------------------------------------------------
// <copyright file="TextCasingExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Utilities.Text;
using MyNet.Utilities.Text.TextCasing;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TextCasingExtensions
{
    extension(TextPipeline pipeline)
    {
        /// <summary>
        /// Transforms a string using the provided casing transform. Transformations are applied in the provided order.
        /// </summary>
        /// <param name="transform">The casing transform to apply.</param>
        /// <returns>The transformed TextPipeline instance.</returns>
        public TextPipeline Case(ITextCasingTransform transform) => pipeline.Apply(transform);

        /// <summary>
        /// Transforms the string to lower case.
        /// </summary>
        /// <returns>The transformed TextPipeline instance.</returns>
        public TextPipeline LowerCase() => pipeline.Apply(Casing.LowerCase);

        /// <summary>
        /// Transforms the string to upper case.
        /// </summary>
        /// <returns>The transformed TextPipeline instance.</returns>
        public TextPipeline UpperCase() => pipeline.Apply(Casing.UpperCase);

        /// <summary>
        /// Transforms the string to title case.
        /// </summary>
        /// <returns>The transformed TextPipeline instance.</returns>
        public TextPipeline TitleCase() => pipeline.Apply(Casing.TitleCase);

        /// <summary>
        /// Transforms the string to sentence case.
        /// </summary>
        /// <returns>The transformed TextPipeline instance.</returns>
        public TextPipeline SentenceCase() => pipeline.Apply(Casing.SentenceCase);

        /// <summary>
        /// Transforms the string to PascalCase.
        /// </summary>
        /// <returns>The transformed TextPipeline instance.</returns>
        public TextPipeline PascalCase() => pipeline.Apply(Casing.PascalCase);

        /// <summary>
        /// Transforms the string to camelCase.
        /// </summary>
        /// <returns>The transformed TextPipeline instance.</returns>
        public TextPipeline CamelCase() => pipeline.Apply(Casing.CamelCase);

        /// <summary>
        /// Transforms the string to snake_case.
        /// </summary>
        /// <returns>The transformed TextPipeline instance.</returns>
        public TextPipeline SnakeCase() => pipeline.Apply(Casing.SnakeCase);

        /// <summary>
        /// Transforms the string to kebab-case.
        /// </summary>
        /// <returns>The transformed TextPipeline instance.</returns>
        public TextPipeline KebabCase() => pipeline.Apply(Casing.KebabCase);
    }

    extension(string input)
    {
        /// <summary>
        /// Transforms a string using the provided casing transform. Transformations are applied in the provided order.
        /// </summary>
        /// <param name="casing">The casing to apply.</param>
        /// <param name="culture">The culture to use for the transformation.</param>
        /// <returns>The transformed string.</returns>
        public string ApplyCase(LetterCasing casing, CultureInfo? culture = null)
        {
            var effectiveCulture = culture.OrCurrent();
            return casing switch
            {
                LetterCasing.Normal => input,
                LetterCasing.Title => input.Apply(effectiveCulture, Casing.TitleCase),
                LetterCasing.Lower => input.Apply(effectiveCulture, Casing.LowerCase),
                LetterCasing.Upper => input.Apply(effectiveCulture, Casing.UpperCase),
                LetterCasing.Sentence => input.Apply(effectiveCulture, Casing.SentenceCase),
                LetterCasing.Pascal => input.Apply(effectiveCulture, Casing.PascalCase),
                LetterCasing.Camel => input.Apply(effectiveCulture, Casing.CamelCase),
                LetterCasing.Snake => input.Apply(effectiveCulture, Casing.SnakeCase),
                LetterCasing.Kebab => input.Apply(effectiveCulture, Casing.KebabCase),
                _ => throw new ArgumentOutOfRangeException(nameof(casing))
            };
        }

        /// <summary>
        /// Transforms the string to upper case.
        /// </summary>
        /// <returns>The transformed string.</returns>
        public string ToUpperCase() => input.ApplyCase(LetterCasing.Upper);

        /// <summary>
        /// Transforms the string to lower case.
        /// </summary>
        /// <returns>The transformed string.</returns>
        public string ToLowerCase() => input.ApplyCase(LetterCasing.Lower);

        /// <summary>
        /// Transforms the string to title case.
        /// </summary>
        /// <returns>The transformed string.</returns>
        public string ToTitleCase() => input.ApplyCase(LetterCasing.Title);

        /// <summary>
        /// Transforms the string to sentence case.
        /// </summary>
        /// <returns>The transformed string.</returns>
        public string ToSentenceCase() => input.ApplyCase(LetterCasing.Sentence);

        /// <summary>
        /// Transforms the string to PascalCase.
        /// </summary>
        public string ToPascalCase() => input.ApplyCase(LetterCasing.Pascal);

        /// <summary>
        /// Transforms the string to camelCase.
        /// </summary>
        public string ToCamelCase() => input.ApplyCase(LetterCasing.Camel);

        /// <summary>
        /// Converts the string to snake_case.
        /// </summary>
        public string ToSnakeCase() => input.ApplyCase(LetterCasing.Snake);

        /// <summary>
        /// Converts the string to kebab-case.
        /// </summary>
        public string ToKebabCase() => input.ApplyCase(LetterCasing.Kebab);
    }
}
