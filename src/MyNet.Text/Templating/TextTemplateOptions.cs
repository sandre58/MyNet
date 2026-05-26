// -----------------------------------------------------------------------
// <copyright file="TextTemplateOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.Text.Templating;

/// <summary>
/// Represents options for rendering text templates, including pluralization and quantity formatting settings.
/// </summary>
public sealed class TextTemplateOptions
{
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
}
