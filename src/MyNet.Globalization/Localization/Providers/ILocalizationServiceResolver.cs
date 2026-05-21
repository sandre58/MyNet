// -----------------------------------------------------------------------
// <copyright file="ILocalizationServiceResolver.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MyNet.Globalization.Localization.Providers.Registration;

namespace MyNet.Globalization.Localization.Providers;

/// <summary>
/// Resolves culture-specific localization providers registered in <see cref="ILocalizationFactoryRegistry"/>.
/// Results may be cached for performance.
/// </summary>
public interface ILocalizationServiceResolver
{
    /// <summary>
    /// Attempts to resolve a provider for a specific culture, returning a boolean indicating success or failure.
    /// </summary>
    /// <param name="culture">Target culture.</param>
    /// <param name="provider">The resolved provider if successful; otherwise, null.</param>
    /// <typeparam name="TService">Provider type.</typeparam>
    /// <returns>True if a provider was successfully resolved; otherwise, false.</returns>
    bool TryGet<TService>(CultureInfo culture, [NotNullWhen(true)] out TService? provider)
        where TService : class, ICultureScoped;

    /// <summary>
    /// Resolves a provider for a specific culture, throwing an exception if no provider is registered for the specified culture chain.
    /// </summary>
    /// <param name="culture">Target culture.</param>
    /// <typeparam name="TProvider">Provider type.</typeparam>
    /// <returns>Resolved provider.</returns>
    TProvider GetRequired<TProvider>(CultureInfo culture)
        where TProvider : class, ICultureScoped;

    /// <summary>
    /// Creates a culture-scoped context bound to the given culture,
    /// eliminating the need to pass the culture on subsequent calls.
    /// </summary>
    /// <param name="culture">The culture to scope the context to.</param>
    /// <returns>A new <see cref="ILocalizationServiceContext"/> bound to the specified culture.</returns>
    ILocalizationServiceContext ForCulture(CultureInfo culture);
}
