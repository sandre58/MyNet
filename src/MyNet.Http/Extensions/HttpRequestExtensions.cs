// -----------------------------------------------------------------------
// <copyright file="HttpRequestExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net.Http;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Http;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class HttpRequestExtensions
{
    private const string TimeoutPropertyKey = "RequestTimeout";

    extension(HttpRequestMessage request)
    {
        /// <summary>
        /// Sets a timeout for the HTTP request by storing it in the request's options. This allows for per-request timeout configuration when using HttpClient.
        /// </summary>
        /// <param name="timeout">The timeout duration for the request.</param>
        /// <exception cref="ArgumentNullException">Thrown if the request is null.</exception>
        public void SetTimeout(TimeSpan? timeout)
        {
            ArgumentNullException.ThrowIfNull(request);

            request.Options.Set(new(TimeoutPropertyKey), timeout);
        }

        /// <summary>
        /// Retrieves the timeout value for the HTTP request from its options. If no timeout is set, it returns null. This allows for checking if a specific timeout has been configured for the request.
        /// </summary>
        /// <returns>The timeout duration for the request, or null if no timeout is set.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the request is null.</exception>
        public TimeSpan? GetTimeout()
        {
            ArgumentNullException.ThrowIfNull(request);

            return request.Options.TryGetValue(new HttpRequestOptionsKey<TimeSpan?>(TimeoutPropertyKey), out var value) && value is { } timeout ? timeout : null;
        }
    }
}
