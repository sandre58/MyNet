// -----------------------------------------------------------------------
// <copyright file="ListFormattingOptionsBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Humanizer.Formatting.Collections;

/// <summary>
/// Fluent builder for configuring <see cref="ListFormattingOptions"/>.
/// Allows for a convenient and readable way to construct formatting options.
/// </summary>
/// <remarks>
/// This builder uses a fluent API pattern to make configuration more expressive.
/// Each method returns the builder instance to allow method chaining.
/// </remarks>
public sealed class ListFormattingOptionsBuilder
{
    private string _separator = ", ";
    private bool _useOxfordComma;
    private bool _trimItems = true;
    private bool _ignoreNullOrWhiteSpace = true;
    private ListConjunction _conjunction = ListConjunction.And;

    /// <summary>
    /// Sets the separator to use between items in the list.
    /// </summary>
    /// <param name="separator">The separator string (e.g., ", " or " | ").</param>
    /// <returns>This builder instance for method chaining.</returns>
    public ListFormattingOptionsBuilder WithSeparator(string separator)
    {
        _separator = separator ?? throw new ArgumentNullException(nameof(separator));
        return this;
    }

    /// <summary>
    /// Enables the use of the Oxford comma (comma before the final conjunction).
    /// </summary>
    /// <returns>This builder instance for method chaining.</returns>
    /// <example>
    /// With Oxford comma: "Apple, Banana, and Cherry".
    /// Without: "Apple, Banana and Cherry".
    /// </example>
    public ListFormattingOptionsBuilder WithOxfordComma()
    {
        _useOxfordComma = true;
        return this;
    }

    /// <summary>
    /// Disables the use of the Oxford comma.
    /// </summary>
    /// <returns>This builder instance for method chaining.</returns>
    public ListFormattingOptionsBuilder WithoutOxfordComma()
    {
        _useOxfordComma = false;
        return this;
    }

    /// <summary>
    /// Sets the conjunction to use when formatting the list.
    /// </summary>
    /// <param name="conjunction">The conjunction type (And, Or, None, or Ampersand).</param>
    /// <returns>This builder instance for method chaining.</returns>
    public ListFormattingOptionsBuilder WithConjunction(ListConjunction conjunction)
    {
        _conjunction = conjunction;
        return this;
    }

    /// <summary>
    /// Uses the "and" conjunction.
    /// </summary>
    /// <returns>This builder instance for method chaining.</returns>
    public ListFormattingOptionsBuilder WithAnd()
    {
        _conjunction = ListConjunction.And;
        return this;
    }

    /// <summary>
    /// Uses the "or" conjunction.
    /// </summary>
    /// <returns>This builder instance for method chaining.</returns>
    public ListFormattingOptionsBuilder WithOr()
    {
        _conjunction = ListConjunction.Or;
        return this;
    }

    /// <summary>
    /// Uses no conjunction (items separated only by the separator).
    /// </summary>
    /// <returns>This builder instance for method chaining.</returns>
    public ListFormattingOptionsBuilder WithNoConjunction()
    {
        _conjunction = ListConjunction.None;
        return this;
    }

    /// <summary>
    /// Uses the ampersand (&amp;) as the conjunction.
    /// </summary>
    /// <returns>This builder instance for method chaining.</returns>
    public ListFormattingOptionsBuilder WithAmpersand()
    {
        _conjunction = ListConjunction.Ampersand;
        return this;
    }

    /// <summary>
    /// Enables trimming of items (removes leading and trailing whitespace).
    /// </summary>
    /// <returns>This builder instance for method chaining.</returns>
    public ListFormattingOptionsBuilder WithTrimming()
    {
        _trimItems = true;
        return this;
    }

    /// <summary>
    /// Disables trimming of items.
    /// </summary>
    /// <returns>This builder instance for method chaining.</returns>
    public ListFormattingOptionsBuilder WithoutTrimming()
    {
        _trimItems = false;
        return this;
    }

    /// <summary>
    /// Enables filtering out null, empty, or whitespace-only items.
    /// </summary>
    /// <returns>This builder instance for method chaining.</returns>
    public ListFormattingOptionsBuilder IgnoreNullOrWhiteSpace()
    {
        _ignoreNullOrWhiteSpace = true;
        return this;
    }

    /// <summary>
    /// Disables filtering of null, empty, or whitespace-only items.
    /// </summary>
    /// <returns>This builder instance for method chaining.</returns>
    public ListFormattingOptionsBuilder IncludeNullOrWhiteSpace()
    {
        _ignoreNullOrWhiteSpace = false;
        return this;
    }

    /// <summary>
    /// Builds and returns the configured <see cref="ListFormattingOptions"/> instance.
    /// </summary>
    /// <returns>A new <see cref="ListFormattingOptions"/> instance with the configured values.</returns>
    public ListFormattingOptions Build()
        => new()
        {
            Separator = _separator,
            UseOxfordComma = _useOxfordComma,
            Conjunction = _conjunction,
            TrimItems = _trimItems,
            IgnoreNullOrWhiteSpace = _ignoreNullOrWhiteSpace
        };
}
