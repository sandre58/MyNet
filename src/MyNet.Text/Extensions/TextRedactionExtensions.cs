// -----------------------------------------------------------------------
// <copyright file="TextRedactionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using MyNet.Primitives;
using MyNet.Text.Redaction;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Text;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TextRedactionExtensions
{
    extension(TextPipeline pipeline)
    {
        /// <summary>
        /// Applies a redaction transform to the pipeline.
        /// </summary>
        public TextPipeline Redact(ITextRedactorTransform transform)
        {
            ArgumentNullException.ThrowIfNull(transform);
            return pipeline.Apply(transform);
        }
    }

    extension(string input)
    {
        /// <summary>
        /// Applies a redaction transform.
        /// </summary>
        public string Redact(ITextRedactorTransform transform, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(transform);
            return transform.Apply(input, culture.OrCurrent());
        }

        /// <summary>
        /// Redacts the middle part of the text and keeps edge characters visible.
        /// </summary>
        public string Redact(int showStart, int showEnd, char maskCharacter = '*', CultureInfo? culture = null)
            => input.Redact(new FixedMaskTextRedactor(new()
                {
                    ShowStart = showStart,
                    ShowEnd = showEnd,
                    MaskCharacter = maskCharacter
                }),
                culture);

        /// <summary>
        /// Redacts parts matching a regular expression.
        /// </summary>
        public string Redact(Regex pattern, string replacement = "***", CultureInfo? culture = null) => input.Redact(new RegexTextRedactor(pattern, replacement), culture);
    }
}
