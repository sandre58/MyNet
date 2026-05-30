// -----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Primitives;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class StringExtensions
{
    extension(string? value)
    {
        /// <summary>
        /// Returns the original string if it is not null, or an empty string if it is null.
        /// </summary>
        /// <returns>The original string or an empty string.</returns>
        public string OrEmpty() => value ?? string.Empty;

        /// <summary>
        /// Returns the original string if it is not null or whitespace, or the specified placeholder string if it is null or whitespace.
        /// </summary>
        /// <param name="placeholder">The placeholder string to return if the original string is null or whitespace.</param>
        /// <returns>The original string or the placeholder string.</returns>
        public string Or(string placeholder) => string.IsNullOrWhiteSpace(value) ? placeholder : value;

        /// <summary>
        /// Add a prefix before value, separated by period.
        /// </summary>
        /// <param name="prefix">Prefix to add.</param>
        /// <param name="separator">Separator to use between prefix and value.</param>
        /// <returns>prefix.value.</returns>
        public string? WithPrefix(string? prefix = null, string? separator = "-")
            => !string.IsNullOrWhiteSpace(prefix) && !string.IsNullOrWhiteSpace(value) ? $"{prefix}{separator}{value}" : value;

        /// <summary>
        /// Determines whether the original string contains any of the specified values, using a case-insensitive comparison. If the original string is null or empty, this method returns false.
        /// </summary>
        /// <param name="values">The values to check for in the original string.</param>
        /// <returns>True if the original string contains any of the specified values; otherwise, false.</returns>
        public bool ContainsAny(params ReadOnlySpan<string> values)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            foreach (var item in values)
            {
                if (!string.IsNullOrEmpty(item) && value.Contains(item, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the original string does not contain any of the specified values, using a case-insensitive comparison. If the original string is null or empty, this method returns true.
        /// </summary>
        /// <param name="values">The values to check for in the original string.</param>
        /// <returns>True if the original string does not contain any of the specified values; otherwise, false.</returns>
        public bool NotContainsAny(params ReadOnlySpan<string> values)
        {
            if (string.IsNullOrEmpty(value))
                return true;

            foreach (var item in values)
            {
                if (!string.IsNullOrEmpty(item) && value.Contains(item, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }
    }

    extension(string value)
    {
        /// <summary>
        /// Extension method to format string with passed arguments. Current thread's current culture is used.
        /// </summary>
        /// <param name="args">arguments.</param>
        public string FormatWith(params object?[] args) => string.Format(CultureInfo.CurrentCulture, value, args);

        /// <summary>
        /// Extension method to format string with passed arguments using specified format provider (i.e. CultureInfo).
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="args">arguments.</param>
        public string FormatWith(IFormatProvider provider, params object?[] args) => string.Format(provider, value, args);

        /// <summary>
        /// Extension method to format string with passed arguments using the invariant culture.
        /// </summary>
        /// <param name="args">arguments.</param>
        public string FormatWithInvariant(params object?[] args) => string.Format(CultureInfo.InvariantCulture, value, args);

        /// <summary>
        /// Converts the string to a <see cref="Version"/>.
        /// </summary>
        /// <returns>The parsed <see cref="Version"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the input is empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the input is null.</exception>
        /// <exception cref="FormatException">Thrown when the input format is invalid.</exception>
        /// <exception cref="OverflowException">Thrown when a numeric component is too large.</exception>
        public Version ToVersion() => new(value);

        /// <summary>
        /// Attempts to convert the string to a <see cref="Version"/>.
        /// </summary>
        /// <param name="version">When this method returns, contains the parsed version if successful; otherwise <see langword="null"/>.</param>
        /// <returns><see langword="true"/> when parsing succeeds; otherwise <see langword="false"/>.</returns>
        public bool TryToVersion(out Version? version)
        {
            try
            {
                version = new(value);
                return true;
            }
            catch (ArgumentException)
            {
                version = null;
                return false;
            }
            catch (FormatException)
            {
                version = null;
                return false;
            }
            catch (OverflowException)
            {
                version = null;
                return false;
            }
        }
    }
}
