// -----------------------------------------------------------------------
// <copyright file="ResxDomainFakerProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using MyNet.Fakers.Localization;

namespace MyNet.Fakers.Internet;

/// <summary>
/// Provides fake domain datasets from localized RESX resources.
/// </summary>
public sealed class ResxDomainFakerProvider : ResxProviderBase, IDomainFakerProvider
{
    /// <summary>
    /// Creates a new instance of the <see cref="ResxDomainFakerProvider"/> class for the specified culture.
    /// </summary>
    /// <param name="culture">The culture associated with the provider.</param>
    /// <returns>A new instance of <see cref="ResxDomainFakerProvider"/>.</returns>
    public static ResxDomainFakerProvider Create(CultureInfo culture) => new(culture);

    /// <summary>
    /// Initializes a new instance of the <see cref="ResxDomainFakerProvider"/> class with the specified culture.
    /// </summary>
    /// <param name="culture">The culture associated with the provider.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="culture"/> is null.</exception>
    private ResxDomainFakerProvider(CultureInfo culture)
        : base(culture, DomainsResources.ResourceManager)
    {
        Domains = LoadDataset(nameof(DomainsResources.Domains));
        Hosts = LoadDataset(nameof(DomainsResources.Hosts));
    }

    /// <inheritdoc />
    public IReadOnlyList<string> Domains { get; }

    /// <inheritdoc />
    public IReadOnlyList<string> Hosts { get; }
}
