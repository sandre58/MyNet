// -----------------------------------------------------------------------
// <copyright file="WebApiException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;

namespace MyNet.Http.Exceptions;

/// <summary>
/// Represents an error returned by a web API.
/// </summary>
public class WebApiException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiException"/> class.
    /// </summary>
    public WebApiException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiException"/> class.
    /// </summary>
    /// <param name="statusCode">HTTP status code returned by the API.</param>
    /// <param name="reasonPhrase">HTTP reason phrase returned by the API.</param>
    /// <param name="responseBody">Raw response body.</param>
    /// <param name="payload">Parsed response payload when available.</param>
    public WebApiException(HttpStatusCode statusCode, string? reasonPhrase, string? responseBody, object? payload = null)
        : base(BuildMessage(statusCode, reasonPhrase, responseBody, payload))
    {
        StatusCode = statusCode;
        ReasonPhrase = reasonPhrase;
        ResponseBody = responseBody;
        Payload = payload;
    }

    /// <summary>
    /// Gets the HTTP status code returned by the API.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets the HTTP reason phrase returned by the API.
    /// </summary>
    public string? ReasonPhrase { get; }

    /// <summary>
    /// Gets the raw response body returned by the API.
    /// </summary>
    public string? ResponseBody { get; }

    /// <summary>
    /// Gets the parsed response payload when available.
    /// </summary>
    public object? Payload { get; }

    protected WebApiException(string message)
        : base(message)
    {
    }

    protected WebApiException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    private static string BuildMessage(HttpStatusCode statusCode, string? reasonPhrase, string? responseBody, object? payload) => !string.IsNullOrWhiteSpace(responseBody)
        ? $"HTTP {(int)statusCode} {reasonPhrase}: {responseBody}"
        : payload is not null ? $"HTTP {(int)statusCode} {reasonPhrase}: {payload}" : $"HTTP {(int)statusCode} {reasonPhrase}".Trim();
}
