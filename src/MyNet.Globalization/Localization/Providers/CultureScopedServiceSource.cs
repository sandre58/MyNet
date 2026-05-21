// -----------------------------------------------------------------------
// <copyright file="CultureScopedServiceSource.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MyNet.Globalization.Culture;

namespace MyNet.Globalization.Localization.Providers;

/// <summary>
/// Provides access to culture-aware services, allowing retrieval of services based on a specified culture or the current culture from the culture context. This class uses an <see cref="ILocalizationServiceResolver"/> to resolve services for specific cultures and an <see cref="ICultureContext"/> to determine the current culture when no specific culture is provided.
/// </summary>
/// <param name="resolver">The underlying service resolver.</param>
/// <param name="cultureContext">The culture context to determine the current culture.</param>
/// <typeparam name="TService">The type of the culture-aware service.</typeparam>
public sealed class CultureScopedServiceSource<TService>(ILocalizationServiceResolver resolver, ICultureContext cultureContext) : ICultureScopedServiceSource<TService>
    where TService : class, ICultureScoped
{
    /// <inheritdoc />
    public TService Get(CultureInfo? culture = null) => resolver.GetRequired<TService>( culture ?? cultureContext.CurrentCulture);

    /// <inheritdoc />
    public bool TryGet(CultureInfo? culture, [NotNullWhen(true)] out TService? service) => resolver.TryGet(culture ?? cultureContext.CurrentCulture, out service);
}
