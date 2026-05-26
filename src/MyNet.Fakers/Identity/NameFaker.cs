// -----------------------------------------------------------------------
// <copyright file="NameFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Generator;
using MyNet.Globalization.Localization.Providers;

namespace MyNet.Fakers.Identity;

/// <summary>
/// Current implementation of <see cref="INameFaker"/> that uses localization providers to generate names based on culture and gender.
/// </summary>
/// <param name="random">The random generator to use for selecting names.</param>
/// <param name="source">The localization provider resolver to use for obtaining name providers.</param>
public sealed class NameFaker(IRandomGenerator random, ICultureScopedServiceSource<INameFakerProvider> source) : INameFaker
{
    /// <inheritdoc />
    public string FirstName(GenderType gender, CultureInfo? culture = null)
    {
        var provider = source.Get(culture);

        var names = gender switch
        {
            GenderType.Female => provider.FemaleFirstNames,
            _ => provider.MaleFirstNames
        };

        return random.Item(names);
    }

    /// <inheritdoc />
    public string LastName(CultureInfo? culture = null)
    {
        var provider = source.Get(culture);

        return random.Item(provider.LastNames);
    }

    /// <inheritdoc />
    public string Prefix(CultureInfo? culture = null)
    {
        var provider = source.Get(culture);

        return random.Item(provider.Prefixes);
    }

    /// <inheritdoc />
    public string Suffix(CultureInfo? culture = null)
    {
        var provider = source.Get(culture);

        return random.Item(provider.Suffixes);
    }

    /// <inheritdoc />
    public string FullName(GenderType gender, NameFormat format = NameFormat.Standard, CultureInfo? culture = null)
    {
        var firstName = FirstName(gender, culture);
        var lastName = LastName(culture);
        var prefix = Prefix(culture);
        var suffix = Suffix(culture);

        return format switch
        {
            NameFormat.Standard => $"{firstName} {lastName}",
            NameFormat.Inverse => $"{lastName} {firstName}",
            NameFormat.WithPrefix => $"{prefix} {firstName} {lastName}",
            NameFormat.InverseWithPrefix => $"{prefix} {lastName} {firstName}",
            NameFormat.WithSuffix => $"{firstName} {lastName} {suffix}",
            NameFormat.InverseWithSuffix => $"{lastName} {firstName} {suffix}",
            _ => $"{firstName} {lastName}"
        };
    }
}
