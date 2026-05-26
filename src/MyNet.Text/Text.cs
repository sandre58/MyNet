// -----------------------------------------------------------------------
// <copyright file="Text.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Text;

/// <summary>
/// A portal to string transformation using ITextTransform.
/// </summary>
public static class Text
{
    /// <summary>
    /// Transforms a string using the provided transformers. Transformations are applied in the provided order.
    /// </summary>
    /// <param name="value">The initial string value to transform.</param>
    /// <param name="culture">The culture to use for culture-specific transformations.</param>
    /// <returns>A TextPipeline instance for chaining transformations.</returns>
    public static TextPipeline For(string value, CultureInfo? culture = null) => new(value, culture ?? CultureInfo.CurrentCulture);
}
