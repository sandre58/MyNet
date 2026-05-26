// -----------------------------------------------------------------------
// <copyright file="TextSanitizationExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Primitives;
using MyNet.Text.Sanitization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Text;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TextSanitizationExtensions
{
    extension(TextPipeline pipeline)
    {
        /// <summary>
        /// Applies a sanitization transform to the pipeline.
        /// </summary>
        public TextPipeline Sanitize(ITextSanitizerTransform transform)
        {
            ArgumentNullException.ThrowIfNull(transform);
            return pipeline.Apply(transform);
        }
    }

    extension(string input)
    {
        /// <summary>
        /// Applies a sanitization transform.
        /// </summary>
        public string Sanitize(ITextSanitizerTransform transform, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(transform);
            return transform.Apply(input, culture.OrCurrent());
        }

        /// <summary>
        /// Sanitizes text so it can be used as a file name.
        /// </summary>
        public string SanitizeFileName(CultureInfo? culture = null)
            => input.Sanitize(Sanitizer.FileName, culture);
    }
}
