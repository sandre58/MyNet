// -----------------------------------------------------------------------
// <copyright file="ICultureScopedServiceSource.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MyNet.Globalization.Localization.Providers;

/// <summary>
/// Defines an accessor for culture-aware services, allowing retrieval of services based on culture. This interface abstracts the logic of resolving culture-specific services, enabling consumers to obtain the appropriate service for a given culture without needing to manage the resolution process themselves.
/// </summary>
/// <typeparam name="TService">The type of the culture-aware service.</typeparam>
public interface ICultureScopedServiceSource<TService>
    where TService : class, ICultureScoped
{
    /// <summary>
    /// Gets the service for the specified culture,
    /// or the current culture if none is specified.
    /// </summary>
    TService Get(CultureInfo? culture = null);

    /// <summary>
    /// Attempts to get the service for the specified culture,
    /// or the current culture if none is specified.
    /// </summary>
    bool TryGet(CultureInfo? culture, [NotNullWhen(true)] out TService? service);
}
