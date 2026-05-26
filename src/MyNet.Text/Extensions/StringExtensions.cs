// -----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyNet.Text.Sanitization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Text;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class StringExtensions
{
    private const string RelativeUriSeparator = "/";
    private const string WebUriSeparator = "&";
    private const string QueryStringEquals = "=";

    extension(string value)
    {
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
