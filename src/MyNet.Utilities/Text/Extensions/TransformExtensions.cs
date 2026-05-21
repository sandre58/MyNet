// -----------------------------------------------------------------------
// <copyright file="TransformExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using MyNet.Utilities.Text;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TransformExtensions
{
    extension(TextPipeline pipeline)
    {
        /// <summary>
        /// Transforms the text in the pipeline using the provided transformers. Transformations are applied in the provided order.
        /// </summary>
        /// <param name="transformers">The transformers to apply.</param>
        /// <returns>The transformed text.</returns>
        public string Transform(params ITextTransform[] transformers) => pipeline.Apply(transformers).Value;

        /// <summary>
        /// Applies the provided transformers to the pipeline. Transformations are applied in the provided order.
        /// </summary>
        /// <param name="transformers">The transformers to apply.</param>
        /// <returns>The transformed TextPipeline instance.</returns>
        public TextPipeline Apply(params ITextTransform[] transformers) => transformers.Aggregate(pipeline, (current, transform) => current.Apply(transform));
    }

    extension(string input)
    {
        /// <summary>
        /// Transforms a string using the provided transformers. Transformations are applied in the provided order.
        /// </summary>
        public string Apply(CultureInfo culture, params ITextTransform[] transformers)
        {
            ArgumentNullException.ThrowIfNull(input);
            ArgumentNullException.ThrowIfNull(culture);

            return transformers.Aggregate(input, (current, transform) => transform.Apply(current, culture));
        }

        /// <summary>
        /// Transforms a string using the provided transformers. Transformations are applied in the provided order. The current culture is used for culture-specific transformations.
        /// </summary>
        /// <param name="transformers">The transformers to apply.</param>
        /// <returns>The transformed string.</returns>
        public string Apply(params ITextTransform[] transformers) => input.Apply(CultureInfo.CurrentCulture, transformers);
    }
}
