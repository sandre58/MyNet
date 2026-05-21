// -----------------------------------------------------------------------
// <copyright file="LocalizationServiceContext.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MyNet.Globalization.Localization.Providers;

/// <summary>
/// Current implementation of <see cref="ILocalizationServiceContext"/>.
/// Binds a specific <see cref="Culture"/> to an <see cref="ILocalizationServiceResolver"/>,
/// eliminating the need to pass the culture on every provider resolution call.
/// </summary>
/// <param name="resolver">The underlying provider resolver.</param>
/// <param name="culture">The culture this context is scoped to.</param>
public sealed class LocalizationServiceContext(ILocalizationServiceResolver resolver, CultureInfo culture) : ILocalizationServiceContext
{
    /// <inheritdoc />
    public CultureInfo Culture { get; } = culture ?? throw new ArgumentNullException(nameof(culture));

    public bool TryGet<TService>([NotNullWhen(true)] out TService? service)
        where TService : class, ICultureScoped => resolver.TryGet(Culture, out service);

    /// <inheritdoc />
    public TService GetRequired<TService>()
        where TService : class, ICultureScoped => resolver.GetRequired<TService>(Culture);
}
