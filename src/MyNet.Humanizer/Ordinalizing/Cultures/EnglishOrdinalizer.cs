// -----------------------------------------------------------------------
// <copyright file="EnglishOrdinalizer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Globalization.Culture;

namespace MyNet.Humanizer.Ordinalizing.Cultures;

/// <summary>
/// Represents an ordinalizer for the English language, which converts numbers into their corresponding ordinal string representations (e.g., 1 to "1st", 2 to "2nd", 3 to "3rd", etc.). This class implements the specific rules for ordinalization in English, including handling special cases for numbers ending in 11, 12, and 13, which all use the "th" suffix regardless of their last digit. The EnglishOrdinalizer is designed to be used within the Humanizer library to provide culture-specific ordinalization functionality for English-speaking users.
/// </summary>
public sealed class EnglishOrdinalizer : OrdinalizerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnglishOrdinalizer"/> class.
    /// </summary>
    internal EnglishOrdinalizer()
        : base(SupportedCultures.English)
    {
    }

    /// <inheritdoc/>
    public override string Ordinalize(long number, OrdinalizationOptions? options = null) => $"{number}{GetSuffix(number)}";

    /// <summary>
    /// Determines the appropriate suffix for a given number based on English ordinalization rules. The method calculates the absolute value of the number to handle negative numbers correctly and then checks the last two digits to account for special cases (11, 12, 13) that always use the "th" suffix. For other numbers, it determines the suffix based on the last digit, returning "st" for numbers ending in 1, "nd" for numbers ending in 2, "rd" for numbers ending in 3, and "th" for all other cases. This logic ensures that the correct ordinal suffix is applied according to English grammar rules.
    /// </summary>
    /// <param name="number">The number for which to determine the ordinal suffix.</param>
    /// <returns>The appropriate ordinal suffix for the given number.</returns>
    private static string GetSuffix(long number)
    {
        var absolute = Math.Abs(number);

        var lastTwoDigits = absolute % 100;

        return lastTwoDigits is >= 11 and <= 13
            ? "th"
            : (absolute % 10) switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th"
        };
    }
}
