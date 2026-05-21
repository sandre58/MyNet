// -----------------------------------------------------------------------
// <copyright file="TextPipeline.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Utilities.Text;

/// <summary>
/// A pipeline for applying multiple text transformations in sequence. This class allows you to chain multiple transformations together, applying them one after the other to a string value. Each transformation is applied to the result of the previous transformation, allowing for complex text manipulation in a fluent and readable manner.
/// </summary>
/// <param name="value">The initial string value for the pipeline.</param>
/// <param name="culture">The culture to use for culture-specific transformations.</param>
public sealed class TextPipeline(string value, CultureInfo culture)
{
    /// <summary>
    /// Gets the current value of the text after applying transformations. This property holds the result of the transformations applied to the initial string value. As transformations are applied to the pipeline, this property will be updated to reflect the latest transformed string.
    /// </summary>
    public string Value { get; private set; } = value;

    /// <summary>
    /// Applies a text transformation to the current value in the pipeline. This method takes an implementation of the ITextTransform interface and applies it to the current value, updating the Value property with the result. The transformation is applied using the culture specified when the TextPipeline was created, allowing for culture-specific transformations when necessary. The method returns the TextPipeline instance itself, enabling fluent chaining of multiple transformations in a single statement.
    /// </summary>
    /// <param name="transform">The text transformation to apply.</param>
    /// <returns>The TextPipeline instance with the applied transformation.</returns>
    public TextPipeline Apply(ITextTransform transform)
    {
        Value = transform.Apply(Value, culture);
        return this;
    }
}
