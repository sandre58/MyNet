// -----------------------------------------------------------------------
// <copyright file="ILocalizationServiceContext.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MyNet.Globalization.Localization.Providers;

/// <summary>
/// Provides culture-scoped access to localization services.
/// Wraps an <see cref="ILocalizationServiceResolver"/> with a fixed <see cref="Culture"/>
/// so callers do not need to pass the culture on every call.
/// Analogous to <c>ITranslationService</c> for services.
/// </summary>
public interface ILocalizationServiceContext
{
    /// <summary>
    /// Gets the culture this context is scoped to.
    /// </summary>
    CultureInfo Culture { get; }

    /// <summary>
    /// Attempts to resolve a service for the scoped culture, returning a boolean indicating success or failure.
    /// </summary>
    /// <param name="service">The resolved service if successful; otherwise, null.</param>
    /// <typeparam name="TService">Service type.</typeparam>
    /// <returns>True if a service was successfully resolved; otherwise, false.</returns>
    bool TryGet<TService>([NotNullWhen(true)] out TService? service)
        where TService : class, ICultureScoped;

    /// <summary>
    /// Resolves a service for the scoped culture, throwing an exception if no service is registered for the specified culture chain.
    /// </summary>
    /// <typeparam name="TService">Service type.</typeparam>
    /// <returns>Resolved service.</returns>
    TService GetRequired<TService>()
        where TService : class, ICultureScoped;
}
