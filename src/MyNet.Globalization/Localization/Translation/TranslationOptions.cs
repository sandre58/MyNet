// -----------------------------------------------------------------------
// <copyright file="TranslationOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.Utilities.Text.Templating;

namespace MyNet.Globalization.Localization.Translation;

/// <summary>
/// Options for translation operations, such as display style, count for pluralization, and date pattern usage.
/// </summary>
public sealed class TranslationOptions
{
    /// <summary>
    /// Gets the default translation options.
    /// </summary>
    public DisplayStyle Style { get; init; } = DisplayStyle.Default;

    /// <summary>
    /// Gets the count to use for pluralization when translating a key. If null, no pluralization will be applied.
    /// </summary>
    public decimal? Quantity { get; init; }

    /// <summary>
    /// Gets the mode for rendering quantity information in translation templates. This determines how placeholders related to quantity will be processed and included in the final rendered translation string.
    /// </summary>
    public QuantityRenderingMode QuantityRenderingMode { get; init; } = QuantityRenderingMode.PlaceholderOnly;

    /// <summary>
    /// Gets the separator to use between the quantity value and its unit when rendering quantity information.
    /// </summary>
    public string QuantitySeparator { get; init; } = " ";

    /// <summary>
    /// Gets the format string to use when rendering the quantity value.
    /// </summary>
    public string? QuantityFormat { get; init; }

    /// <summary>
    /// Gets additional named rendering arguments.
    /// </summary>
    public IReadOnlyDictionary<string, object?> Arguments { get; init; } = new Dictionary<string, object?>();

    /// <summary>
    /// Gets a value indicating whether to use inflection fallback when translating a key. If true, inflection fallback will be applied when no direct translation is found. If false, inflection fallback will not be used.
    /// </summary>
    public bool UseInflectionFallback { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether to return the key itself if no translation is found. If true, the translation service will return the key when no translation is available. If false, it will return an empty string or a default value.
    /// </summary>
    public bool UseKeyAsFallback { get; init; } = true;
}
