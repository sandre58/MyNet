// -----------------------------------------------------------------------
// <copyright file="TextTemplateOptionsBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyNet.Text.Templating;

/// <summary>
/// Builder for creating <see cref="TextTemplateOptions"/> instances for use with text templates.
/// </summary>
public sealed class TextTemplateOptionsBuilder
{
    private readonly Dictionary<string, object?> _arguments = new(StringComparer.OrdinalIgnoreCase);
    private decimal? _quantity;
    private QuantityRenderingMode _quantityRenderingMode = QuantityRenderingMode.PlaceholderOnly;
    private string _quantitySeparator = " ";
    private string? _quantityFormat;

    /// <summary>
    /// Sets the count used for pluralization and rendering.
    /// Automatically exposes the "count" rendering argument.
    /// </summary>
    /// <param name="count">The count value.</param>
    /// <param name="quantityRenderingMode">The mode for rendering quantity information.</param>
    /// <returns>The current builder.</returns>
    public TextTemplateOptionsBuilder WithQuantity(decimal count, QuantityRenderingMode quantityRenderingMode = QuantityRenderingMode.PlaceholderOnly)
    {
        _quantity = count;
        _quantityRenderingMode = quantityRenderingMode;

        return this;
    }

    /// <summary>
    /// Sets the count used for pluralization and rendering, with the quantity value rendered as a prefix to the translation.
    /// </summary>
    /// <param name="count">The count value.</param>
    /// <param name="quantityFormat">The format string to use when rendering the quantity value.</param>
    /// <param name="quantitySeparator">The separator to use between the quantity value and its unit.</param>
    /// <returns>The current builder.</returns>
    public TextTemplateOptionsBuilder PrefixWithQuantity(decimal count, string? quantityFormat = null, string quantitySeparator = " ")
    {
        _quantity = count;
        _quantityRenderingMode = QuantityRenderingMode.Prefix;
        _quantitySeparator = quantitySeparator;
        _quantityFormat = quantityFormat;

        return this;
    }

    /// <summary>
    /// Sets the count used for pluralization and rendering, with the quantity value rendered as a suffix to the translation.
    /// </summary>
    /// <param name="count">The count value.</param>
    /// <param name="quantityFormat">The format string to use when rendering the quantity value.</param>
    /// <param name="quantitySeparator">The separator to use between the quantity value and its unit.</param>
    /// <returns>The current builder.</returns>
    public TextTemplateOptionsBuilder SuffixWithQuantity(decimal count, string? quantityFormat = null, string quantitySeparator = " ")
    {
        _quantity = count;
        _quantityRenderingMode = QuantityRenderingMode.Suffix;
        _quantitySeparator = quantitySeparator;
        _quantityFormat = quantityFormat;

        return this;
    }

    /// <summary>
    /// Adds or replaces a rendering argument.
    /// </summary>
    /// <param name="name">The argument name.</param>
    /// <param name="value">The argument value.</param>
    /// <returns>The current builder.</returns>
    public TextTemplateOptionsBuilder WithArgument(string name, object? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        _arguments[name] = value;

        return this;
    }

    /// <summary>
    /// Adds multiple rendering arguments.
    /// </summary>
    /// <param name="arguments">The arguments to add.</param>
    /// <returns>The current builder.</returns>
    public TextTemplateOptionsBuilder WithArguments(IEnumerable<KeyValuePair<string, object?>> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        foreach (var argument in arguments)
        {
            _arguments[argument.Key] = argument.Value;
        }

        return this;
    }

    /// <summary>
    /// Builds the <see cref="TextTemplateOptions"/> instance with the configured options.
    /// </summary>
    /// <returns>The created text template options.</returns>
    public TextTemplateOptions Build()
        => new()
        {
            Quantity = _quantity,
            QuantityFormat = _quantityFormat,
            QuantityRenderingMode = _quantityRenderingMode,
            QuantitySeparator = _quantitySeparator,
            Arguments = new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(_arguments, StringComparer.OrdinalIgnoreCase))
        };
}
