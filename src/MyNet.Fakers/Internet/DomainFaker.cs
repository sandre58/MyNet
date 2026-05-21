// -----------------------------------------------------------------------
// <copyright file="DomainFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Globalization.Localization.Providers;
using MyNet.Utilities.Generator;

namespace MyNet.Fakers.Internet;

/// <summary>
/// Current implementation of <see cref="IDomainFaker"/>.
/// </summary>
public sealed class DomainFaker(IRandomGenerator random, ICultureScopedServiceSource<IDomainFakerProvider> source) : IDomainFaker
{
    /// <inheritdoc/>
    public string Domain(CultureInfo? culture = null)
        => random.Item(source.Get(culture).Domains);

    /// <inheritdoc/>
    public string Host(CultureInfo? culture = null)
    {
        var domain = Domain(culture);
        var provider = source.Get(culture);
        var name = random.Item(provider.Hosts);

        return $"{name}.{domain}";
    }
}
