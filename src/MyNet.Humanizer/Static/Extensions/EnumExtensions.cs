// -----------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using MyNet.Humanizer.Display;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Humanizer.Static;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Contains extension methods for humanizing Enums.
/// </summary>
public static class EnumExtensions
{
    extension(Enum value)
    {
        /// <summary>
        /// Gets the display name of the enum value, if it has a <see cref="DisplayNameAttribute"/>. Otherwise, returns the humanized name of the enum value.
        /// </summary>
        /// <param name="culture">The culture to use when retrieving the display name.</param>
        /// <returns>The display name of the enum value.</returns>
        public string Humanize(CultureInfo? culture = null) => value.Humanize(DisplayTextOptions.Default, culture);

        /// <summary>
        /// Gets the display name of the enum value, if it has a <see cref="DisplayNameAttribute"/>. Otherwise, returns the humanized name of the enum value.
        /// Uses the DI-registered <see cref="IDisplayTextStrategy{T}"/>.
        /// </summary>
        /// <param name="options">The options to use when retrieving the display name.</param>
        /// <param name="culture">The culture to use when retrieving the display name.</param>
        /// <returns>The display name of the enum value.</returns>
        public string Humanize(DisplayTextOptions options, CultureInfo? culture = null)
            => TextHumanizer.Humanize(value, options, culture);
    }

    extension(string? input)
    {
        /// <summary>
        /// Dehumanizes the input string to the specified target enum type. The method first attempts to parse the input string directly to the enum values, and if that fails, it compares the humanized versions of the enum values with the input string. If no match is found, it throws a KeyNotFoundException.
        /// </summary>
        /// <param name="culture">The culture to use for humanization.</param>
        /// <typeparam name="TEnum">The type of the target enum.</typeparam>
        /// <returns>The matched enum value.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no matching enum value is found.</exception>
        public TEnum DehumanizeTo<TEnum>(CultureInfo? culture = null)
            where TEnum : struct, Enum
            => (TEnum)DehumanizeCore(input, typeof(TEnum), culture);

        /// <summary>
        /// Dehumanizes the input string to the specified target enum type. The method first attempts to parse the input string directly to the enum values, and if that fails, it compares the humanized versions of the enum values with the input string. If no match is found, it handles the situation based on the specified OnNoMatch behavior.
        /// </summary>
        /// <param name="targetEnum">The target enum type to dehumanize to.</param>
        /// <param name="culture">The culture to use for humanization.</param>
        /// <returns>The matched enum value.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no matching enum value is found.</exception>
        public Enum DehumanizeTo(Type targetEnum, CultureInfo? culture = null) => DehumanizeCore(input, targetEnum, culture);

        /// <summary>
        /// Attempts to dehumanize the input string to the specified target enum type. The method first attempts to parse the input string directly to the enum values, and if that fails, it compares the humanized versions of the enum values with the input string. If no match is found, it returns false and sets the result to null.
        /// </summary>
        /// <param name="result">The resulting enum value if a match is found; otherwise, null.</param>
        /// <param name="culture">The culture to use for humanization.</param>
        /// <typeparam name="TEnum">The type of the target enum.</typeparam>
        /// <returns>True if a match is found; otherwise, false.</returns>
        public bool TryDehumanizeTo<TEnum>(out TEnum? result, CultureInfo? culture = null)
            where TEnum : struct, Enum
        {
            try
            {
                result = input.DehumanizeTo<TEnum>(culture);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }
    }

    /// <summary>
    /// Core method that performs the dehumanization logic for both generic and non-generic versions of DehumanizeTo.
    /// </summary>
    /// <param name="input">The input string that was being dehumanized.</param>
    /// <param name="targetEnum">The target enum type to dehumanize to.</param>
    /// <param name="culture">The culture to use for humanization.</param>
    /// <returns>The matched enum value or null if no match is found and the behavior is set to return default.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the targetEnum is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the targetEnum is not an enum.</exception>
    private static Enum DehumanizeCore(string? input, Type targetEnum, CultureInfo? culture)
    {
        ArgumentNullException.ThrowIfNull(targetEnum);

        if (!targetEnum.IsEnum)
        {
            throw new ArgumentException($"Type '{targetEnum}' is not an enum.", nameof(targetEnum));
        }

        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException($"Input '{input}' cannot be null or empty.", nameof(input));
        }

        // 1. Current parsing
        if (Enum.TryParse(targetEnum, input, true, out var parsed) && parsed is Enum parsedEnum)
        {
            return parsedEnum;
        }

        // 2. Humanized parsing
        foreach (var value in Enum.GetValues(targetEnum))
        {
            var enumValue = (Enum)value;

            if (string.Equals(enumValue.Humanize(culture: culture), input, StringComparison.OrdinalIgnoreCase))
            {
                return enumValue;
            }
        }

        // 3. Not found
        throw new KeyNotFoundException($"No matching enum value found for input '{input}' in enum type '{targetEnum}'.");
    }
}
