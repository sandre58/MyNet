// -----------------------------------------------------------------------
// <copyright file="TranslationOptionsBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MyNet.Utilities.Text.Templating;

namespace MyNet.Globalization.Localization.Translation;

/// <summary>
/// Fluent builder used to create immutable <see cref="TranslationOptions"/> instances.
/// </summary>
public sealed class TranslationOptionsBuilder
{
    private readonly Dictionary<string, object?> _arguments = new(StringComparer.OrdinalIgnoreCase);
    private DisplayStyle _style = DisplayStyle.Default;
    private decimal? _quantity;
    private bool _useInflectionFallback = true;
    private bool _useKeyAsFallback = true;
    private QuantityRenderingMode _quantityRenderingMode = QuantityRenderingMode.PlaceholderOnly;
    private string _quantitySeparator = " ";
    private string? _quantityFormat;

    /// <summary>
    /// Sets the display style.
    /// </summary>
    /// <param name="style">The display style.</param>
    /// <returns>The current builder.</returns>
    public TranslationOptionsBuilder WithStyle(DisplayStyle style)
    {
        _style = style;
        return this;
    }

    /// <summary>
    /// Sets the count used for pluralization and rendering.
    /// Automatically exposes the "count" rendering argument.
    /// </summary>
    /// <param name="count">The count value.</param>
    /// <param name="quantityRenderingMode">The mode for rendering quantity information.</param>
    /// <returns>The current builder.</returns>
    public TranslationOptionsBuilder WithQuantity(decimal count, QuantityRenderingMode quantityRenderingMode = QuantityRenderingMode.PlaceholderOnly)
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
    public TranslationOptionsBuilder PrefixWithQuantity(decimal count, string? quantityFormat = null, string quantitySeparator = " ")
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
    public TranslationOptionsBuilder SuffixWithQuantity(decimal count, string? quantityFormat = null, string quantitySeparator = " ")
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
    public TranslationOptionsBuilder WithArgument(string name, object? value)
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
    public TranslationOptionsBuilder WithArguments(IEnumerable<KeyValuePair<string, object?>> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        foreach (var argument in arguments)
        {
            _arguments[argument.Key] = argument.Value;
        }

        return this;
    }

    /// <summary>
    /// Enables inflection fallback.
    /// </summary>
    /// <returns>The current builder.</returns>
    public TranslationOptionsBuilder UseInflectionFallback()
    {
        _useInflectionFallback = true;
        return this;
    }

    /// <summary>
    /// Disables inflection fallback.
    /// </summary>
    /// <returns>The current builder.</returns>
    public TranslationOptionsBuilder WithoutInflectionFallback()
    {
        _useInflectionFallback = false;
        return this;
    }

    /// <summary>
    /// Enables key fallback.
    /// </summary>
    /// <returns>The current builder.</returns>
    public TranslationOptionsBuilder UseKeyFallback()
    {
        _useKeyAsFallback = true;
        return this;
    }

    /// <summary>
    /// Disables key fallback.
    /// </summary>
    /// <returns>The current builder.</returns>
    public TranslationOptionsBuilder WithoutKeyFallback()
    {
        _useKeyAsFallback = false;
        return this;
    }

    /// <summary>
    /// Builds an immutable <see cref="TranslationOptions"/> instance.
    /// </summary>
    /// <returns>The created translation options.</returns>
    public TranslationOptions Build()
        => new()
        {
            Style = _style,
            Quantity = _quantity,
            QuantityFormat = _quantityFormat,
            QuantityRenderingMode = _quantityRenderingMode,
            QuantitySeparator = _quantitySeparator,
            Arguments = new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(_arguments, StringComparer.OrdinalIgnoreCase)),
            UseInflectionFallback = _useInflectionFallback,
            UseKeyAsFallback = _useKeyAsFallback
        };
}
