// -----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyNet.Utilities.Text.Sanitization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class StringExtensions
{
    private const string RelativeUriSeparator = "/";
    private const string WebUriSeparator = "&";
    private const string QueryStringEquals = "=";

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

        /// <summary>
        /// Builds a relative URI by appending sanitized non-empty path segments to the base path.
        /// </summary>
        /// <param name="parameters">Optional path segments appended to the base path.</param>
        /// <returns>A relative <see cref="Uri"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when a segment is "." or "..".</exception>
        public Uri ToRelativeUri(params string?[] parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            var allSegments = string.SplitAndSanitizeSegments(value)
                .Concat(parameters.SelectMany(string.SplitAndSanitizeSegments));

            var relativePath = string.Join(RelativeUriSeparator, allSegments);
            return new(relativePath, UriKind.Relative);
        }

        /// <summary>
        /// Builds an absolute web URI and appends query-string parameters safely.
        /// </summary>
        /// <param name="parameters">Query-string key/value pairs.</param>
        /// <returns>An absolute <see cref="Uri"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the base URI is not absolute.</exception>
        public Uri ToWebUri(params (string Key, string Value)[] parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);
            if (!Uri.TryCreate(value, UriKind.Absolute, out var baseUri))
            {
                throw new ArgumentException("The base URI must be absolute.");
            }

            var builder = new UriBuilder(baseUri);
            var currentQuery = builder.Query.TrimStart('?');
            var appendedQuery = string.Join(
                WebUriSeparator,
                parameters
                    .Where(x => !string.IsNullOrWhiteSpace(x.Key))
                    .Select(x => $"{Sanitizer.UrlSegment.Apply(x.Key, CultureInfo.InvariantCulture)}{QueryStringEquals}{Sanitizer.UrlSegment.Apply(x.Value, CultureInfo.InvariantCulture)}"));

            builder.Query = string.IsNullOrEmpty(currentQuery)
                ? appendedQuery
                : string.IsNullOrEmpty(appendedQuery)
                    ? currentQuery
                    : currentQuery + WebUriSeparator + appendedQuery;

            return builder.Uri;
        }

        /// <summary>
        /// Splits the input string into segments based on the defined separator, trims whitespace, removes empty segments, and sanitizes each segment for safe URI usage. Segments that are "." or ".." are considered invalid and will cause an exception to be thrown.
        /// </summary>
        /// <param name="path">The input string to split and sanitize.</param>
        /// <returns>An enumerable of sanitized segments.</returns>
        private static IEnumerable<string> SplitAndSanitizeSegments(string? path) =>
            string.IsNullOrWhiteSpace(path)
                ? []
                : path
                    .Split([RelativeUriSeparator], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(SanitizePathSegment)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => x!);

        /// <summary>
        /// Sanitizes a path segment by trimming whitespace and applying URL-safe sanitization. If the segment is null, empty, or consists only of whitespace, it returns null. If the segment is "." or "..", it throws an exception since these are not valid path segments in this context.
        /// </summary>
        /// <param name="segment">The path segment to sanitize.</param>
        /// <returns>The sanitized path segment, or null if the segment is null, empty, or consists only of whitespace.</returns>
        /// <exception cref="ArgumentException">Thrown when the segment is "." or "..".</exception>
        private static string? SanitizePathSegment(string? segment)
        {
            if (string.IsNullOrWhiteSpace(segment))
                return null;

            var normalized = segment.Trim();
            return normalized is "." or ".." ? throw new ArgumentException("Path segments '.' and '..' are not allowed.", nameof(segment)) : Sanitizer.UrlSegment.Apply(normalized, CultureInfo.InvariantCulture);
        }
    }
}
