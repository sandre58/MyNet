// -----------------------------------------------------------------------
// <copyright file="ProblemDetailsParser.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Text.Json;

namespace MyNet.Http.Exceptions;

/// <summary>
/// Parses RFC 7807 problem details payloads into HTTP exceptions.
/// </summary>
public static class ProblemDetailsParser
{
    /// <summary>
    /// Attempts to parse a problem details JSON payload into an exception.
    /// </summary>
    /// <param name="json">Problem details JSON payload.</param>
    /// <returns>A parsed exception, or <c>null</c> when the payload is not problem details.</returns>
    public static Exception? TryParseException(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            var title = root.TryGetProperty("title", out var titleElement) && titleElement.ValueKind == JsonValueKind.String
                ? titleElement.GetString() ?? string.Empty
                : string.Empty;

            if (!root.TryGetProperty("errors", out var errorsElement) || errorsElement.ValueKind != JsonValueKind.Object)
            {
                return string.IsNullOrWhiteSpace(title) ? null : new HttpException(title);
            }

            var errors = (from property in errorsElement.EnumerateObject()
                let message = property.Value.ValueKind switch
                {
                    JsonValueKind.Array when property.Value.GetArrayLength() > 0 => property.Value[0].GetString(),
                    JsonValueKind.String => property.Value.GetString(),
                    _ => property.Value.ToString()
                }
                where !string.IsNullOrWhiteSpace(message)
                select new HttpError(property.Name, message)).ToList();

            return errors.Count == 0
                ? (string.IsNullOrWhiteSpace(title) ? null : new HttpException(title))
                : new MultipleHttpException(title, errors);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
