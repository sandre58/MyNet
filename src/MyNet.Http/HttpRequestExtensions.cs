// -----------------------------------------------------------------------
// <copyright file="HttpRequestExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net.Http;

namespace MyNet.Http;

public static class HttpRequestExtensions
{
    private const string TimeoutPropertyKey = "RequestTimeout";

    extension(HttpRequestMessage request)
    {
        public void SetTimeout(TimeSpan? timeout)
        {
            ArgumentNullException.ThrowIfNull(request);

            request.Options.Set(new(TimeoutPropertyKey), timeout);
        }

        public TimeSpan? GetTimeout()
        {
            ArgumentNullException.ThrowIfNull(request);

            return request.Options.TryGetValue(new HttpRequestOptionsKey<TimeSpan?>(TimeoutPropertyKey), out var value) && value is { } timeout ? timeout : null;
        }
    }
}
