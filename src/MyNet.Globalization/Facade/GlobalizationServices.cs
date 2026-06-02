// -----------------------------------------------------------------------
// <copyright file="GlobalizationServices.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using MyNet.Globalization.Culture;
using MyNet.Globalization.DateTime;

namespace MyNet.Globalization.Facade;

/// <summary>
/// Provides static access to globalization-related services.
/// </summary>
public static class GlobalizationServices
{
    private static readonly GlobalizationServiceProxy CurrentProxy = new(CreateDefaultGlobalization());

    private static int _configured;

    /// <summary>
    /// Configures the globalization services by providing an <see cref="IServiceProvider"/>.
    /// Call this once during application startup after building the DI container.
    /// </summary>
    public static void Configure(IGlobalizationService globalization)
    {
        ArgumentNullException.ThrowIfNull(globalization);

        if (Interlocked.Exchange(ref _configured, 1) == 1)
            return;

        CurrentProxy.SetTarget(globalization);
    }

    /// <summary>
    /// Gets the combined <see cref="IGlobalizationService"/> providing both culture and time zone management.
    /// The returned instance is stable for the application lifetime; <see cref="Configure"/> only replaces its target.
    /// </summary>
    public static IGlobalizationService Current => CurrentProxy;

    private static GlobalizationService CreateDefaultGlobalization()
    {
        CultureService defaultCultureService = new();
        TimeZoneService defaultTimeZoneService = new();

        return new(defaultCultureService, defaultTimeZoneService);
    }
}
