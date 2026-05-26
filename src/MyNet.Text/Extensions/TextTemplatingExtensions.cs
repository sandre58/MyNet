// -----------------------------------------------------------------------
// <copyright file="TextTemplatingExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using MyNet.Text.Templating;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Text;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TextTemplatingExtensions
{
    extension(TextPipeline pipeline)
    {
        /// <summary>
        /// Resolves the template using the provided transform. Transformations are applied in the provided order.
        /// </summary>
        /// <param name="transform">The template transform to apply.</param>
        /// <returns>The transformed TextPipeline instance.</returns>
        public TextPipeline ResolveTemplate(ITemplateTransform transform)
        {
            ArgumentNullException.ThrowIfNull(transform);
            return pipeline.Apply(transform);
        }

        /// <summary>
        /// Resolves the template using the provided options. Transformations are applied in the provided order.
        /// </summary>
        /// <param name="options">The template options to apply.</param>
        /// <returns>The transformed TextPipeline instance.</returns>
        public TextPipeline ResolveTemplate(TextTemplateOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);
            return pipeline.Apply(new TemplateTransform(options));
        }

        /// <summary>
        /// Resolves the template using the provided configuration action. Transformations are applied in the provided order.
        /// </summary>
        /// <param name="configure">The configuration action to apply.</param>
        /// <returns>The transformed TextPipeline instance.</returns>
        public TextPipeline ResolveTemplate(Action<TextTemplateOptionsBuilder> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);
            return pipeline.Apply(TextTemplating.Create(configure));
        }

        /// <summary>
        /// Resolves the template using the provided arguments. Transformations are applied in the provided order.
        /// </summary>
        /// <param name="arguments">The arguments to apply.</param>
        /// <returns>The transformed TextPipeline instance.</returns>
        public TextPipeline ResolveTemplate(IEnumerable<KeyValuePair<string, object?>> arguments)
        {
            ArgumentNullException.ThrowIfNull(arguments);
            return pipeline.Apply(TextTemplating.Create(x => x.WithArguments(arguments)));
        }
    }

    extension(string input)
    {
        /// <summary>
        /// Resolves the template using the provided transform. Transformations are applied in the provided order.
        /// </summary>
        /// <param name="transform">The template transform to apply.</param>
        /// <param name="culture">The culture to use for the transformation.</param>
        /// <returns>The transformed string.</returns>
        public string ResolveTemplate(ITemplateTransform transform, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(transform);
            return transform.Apply(input, culture ?? CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Resolves the template using the provided options. Transformations are applied in the provided order.
        /// </summary>
        /// <param name="options">The template options to apply.</param>
        /// <param name="culture">The culture to use for the transformation.</param>
        /// <returns>The transformed string.</returns>
        public string ResolveTemplate(TextTemplateOptions options, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(options);
            return new TemplateTransform(options).Apply(input, culture ?? CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Resolves the template using the provided configuration action. Transformations are applied in the provided order.
        /// </summary>
        /// <param name="configure">The configuration action to apply.</param>
        /// <param name="culture">The culture to use for the transformation.</param>
        /// <returns>The transformed string.</returns>
        public string ResolveTemplate(Action<TextTemplateOptionsBuilder> configure, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(configure);
            return TextTemplating.Create(configure).Apply(input, culture ?? CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Resolves the template using the provided arguments. Transformations are applied in the provided order.
        /// </summary>
        /// <param name="arguments">The arguments to apply.</param>
        /// <param name="culture">The culture to use for the transformation.</param>
        /// <returns>The transformed string.</returns>
        public string ResolveTemplate(IEnumerable<KeyValuePair<string, object?>> arguments, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(arguments);
            return TextTemplating.Create(x => x.WithArguments(arguments)).Apply(input, culture ?? CultureInfo.CurrentCulture);
        }
    }
}
