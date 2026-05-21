// -----------------------------------------------------------------------
// <copyright file="TextSlugificationExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Utilities.Text;
using MyNet.Utilities.Text.Slugification;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TextSlugificationExtensions
{
    extension(TextPipeline pipeline)
    {
        /// <summary>
        /// Applies a slugification transform to the pipeline.
        /// </summary>
        public TextPipeline Slugify(ITextSlugifierTransform transform)
        {
            ArgumentNullException.ThrowIfNull(transform);
            return pipeline.Apply(transform);
        }
    }

    extension(string input)
    {
        /// <summary>
        /// Applies a slugification transform.
        /// </summary>
        public string Slugify(ITextSlugifierTransform transform, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(transform);
            return transform.Apply(input, culture.OrCurrent());
        }

        /// <summary>
        /// Converts text to a URL-friendly slug.
        /// </summary>
        public string Slugify(TextSlugifierOptions? options = null, CultureInfo? culture = null)
            => input.Slugify(new SlugifyTransform(options ?? new TextSlugifierOptions()), culture);
    }
}
