// -----------------------------------------------------------------------
// <copyright file="IncrementTransform.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;

namespace MyNet.Utilities.Text.Formatting;

/// <summary>
/// Represents a text transformation that increments a string by appending an incrementing number until a unique value is found in the existing strings.
/// </summary>
public sealed class IncrementTransform : ITextFormatterTransform
{
    private readonly IncrementTransformOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="IncrementTransform"/> class.
    /// </summary>
    /// <param name="existStrings">The collection of existing strings to check against.</param>
    /// <param name="minIncrement">The minimum increment value. Current is 1.</param>
    /// <param name="step">The step value for incrementing. Current is 1.</param>
    /// <param name="format">Optional format string for the incrementing number.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="existStrings"/> is null.</exception>
    public IncrementTransform(IEnumerable<string> existStrings, int minIncrement = 1, int step = 1, string? format = null)
        : this(new()
        {
            ExistingStrings = existStrings,
            Kind = IncrementKind.Numeric,
            MinIncrement = minIncrement,
            Step = step,
            NumericFormat = format
        })
    {
    }

    private static string ToAlphabeticSuffix(int index)
    {
        var chars = new Stack<char>();
        var current = index;

        while (current > 0)
        {
            current--;
            chars.Push((char)('A' + (current % 26)));
            current /= 26;
        }

        return new([.. chars]);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IncrementTransform"/> class with options.
    /// </summary>
    /// <param name="options">The options used to configure increment behavior.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> or its required members are null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when increment values are invalid.</exception>
    public IncrementTransform(IncrementTransformOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.ExistingStrings);
        ArgumentOutOfRangeException.ThrowIfLessThan(options.MinIncrement, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(options.Step, 1);

        _options = options;
    }

    /// <summary>
    /// Applies the increment transformation to the input string.
    /// </summary>
    /// <param name="input">The string to increment.</param>
    /// <param name="culture">The culture to use for formatting the incrementing number.</param>
    /// <returns>A new string that is an incremented version of the input string.</returns>
    public string Apply(string input, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(culture);

        var inc = _options.MinIncrement;
        var hashSet = new HashSet<string>(_options.ExistingStrings, StringComparer.Ordinal);
        while (true)
        {
            var newName = input + GetSuffix(inc, culture);
            if (!hashSet.Contains(newName))
                return newName;

            inc += _options.Step;
        }
    }

    private string GetSuffix(int inc, CultureInfo culture) => _options.Kind switch
    {
        IncrementKind.Numeric => inc.ToString(_options.NumericFormat, culture),
        IncrementKind.Alpha => ToAlphabeticSuffix(inc),
        _ => throw new NotSupportedException($"Unsupported increment kind: {_options.Kind}")
    };
}
