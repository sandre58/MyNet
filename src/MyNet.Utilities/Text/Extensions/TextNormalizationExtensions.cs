// -----------------------------------------------------------------------
// <copyright file="TextNormalizationExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text;
using MyNet.Utilities.Text;
using MyNet.Utilities.Text.Normalization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TextNormalizationExtensions
{
    extension(TextPipeline pipeline)
    {
        /// <summary>
        /// Applies a normalization transform to the pipeline.
        /// </summary>
        public TextPipeline Normalize(ITextNormalizationTransform transform)
        {
            ArgumentNullException.ThrowIfNull(transform);
            return pipeline.Apply(transform);
        }
    }

    extension(string input)
    {
        /// <summary>
        /// Applies a normalization transform.
        /// </summary>
        public string Normalize(ITextNormalizationTransform transform, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(transform);
            return transform.Apply(input, culture.OrCurrent());
        }

        /// <summary>
        /// Normalizes Unicode representation.
        /// </summary>
        public string NormalizeUnicode(NormalizationForm form, CultureInfo? culture = null)
            => input.Normalize(new UnicodeNormalizationTransform(form), culture);

        /// <summary>
        /// Removes diacritics from text.
        /// </summary>
        public string RemoveDiacritics(CultureInfo? culture = null)
            => input.Normalize(Normalization.RemoveDiacritics, culture);

        /// <summary>
        /// Trims and collapses whitespace to single spaces.
        /// </summary>
        public string NormalizeWhitespace(CultureInfo? culture = null)
            => input.Normalize(Normalization.CleanWhitespace, culture);
    }
}
