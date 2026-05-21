// -----------------------------------------------------------------------
// <copyright file="ResxNameFakerProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using MyNet.Fakers.Localization;

namespace MyNet.Fakers.Identity;

/// <summary>
/// Provides fake name datasets from localized RESX resources.
/// </summary>
public sealed class ResxNameFakerProvider : ResxProviderBase, INameFakerProvider
{
    /// <summary>
    /// Creates a new instance of the <see cref="ResxNameFakerProvider"/> class for the specified culture.
    /// </summary>
    /// <param name="culture">The culture associated with the provider.</param>
    /// <returns>A new instance of <see cref="ResxNameFakerProvider"/>.</returns>
    public static ResxNameFakerProvider Create(CultureInfo culture) => new(culture);

    /// <summary>
    /// Initializes a new instance of the <see cref="ResxNameFakerProvider"/> class with the specified culture.
    /// </summary>
    /// <param name="culture">The culture associated with the provider.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="culture"/> is null.</exception>
    private ResxNameFakerProvider(CultureInfo culture)
    : base(culture, NamesResources.ResourceManager)
    {
        MaleFirstNames = LoadDataset(nameof(NamesResources.MaleFirstNames));
        FemaleFirstNames = LoadDataset(nameof(NamesResources.FemaleFirstNames));
        LastNames = LoadDataset(nameof(NamesResources.LastNames));
        Prefixes = LoadDataset(nameof(NamesResources.Prefixes));
        Suffixes = LoadDataset(nameof(NamesResources.Suffixes));
    }

    /// <inheritdoc />
    public IReadOnlyList<string> MaleFirstNames { get; }

    /// <inheritdoc />
    public IReadOnlyList<string> FemaleFirstNames { get; }

    /// <inheritdoc />
    public IReadOnlyList<string> LastNames { get; }

    /// <inheritdoc />
    public IReadOnlyList<string> Prefixes { get; }

    /// <inheritdoc />
    public IReadOnlyList<string> Suffixes { get; }
}
