// -----------------------------------------------------------------------
// <copyright file="TextTemplating.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Text.Templating;

/// <summary>
/// Provides access to text templating utilities, such as sanitizers and transformers.
/// </summary>
public static class TextTemplating
{
    /// <summary>
    /// Gets a sanitizer suitable for file names.
    /// </summary>
    public static ITemplateTransform Default { get; } = new TemplateTransform(new());

    /// <summary>
    /// Creates a new template transform with the specified configuration.
    /// </summary>
    /// <param name="configure">An action to configure the text template options.</param>
    /// <returns>A new template transform instance.</returns>
    public static ITemplateTransform Create(Action<TextTemplateOptionsBuilder> configure)
    {
        var builder = new TextTemplateOptionsBuilder();
        configure(builder);
        return new TemplateTransform(builder.Build());
    }
}
