// -----------------------------------------------------------------------
// <copyright file="ResxPhoneFakerProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using MyNet.Fakers.Localization;

namespace MyNet.Fakers.Contacts;

/// <summary>
/// Provides fake phone datasets from localized RESX resources.
/// </summary>
public sealed class ResxPhoneFakerProvider : ResxProviderBase, IPhoneFakerProvider
{
    /// <summary>
    /// Creates a new instance of the <see cref="ResxPhoneFakerProvider"/> class for the specified culture.
    /// </summary>
    /// <param name="culture">The culture associated with the provider.</param>
    /// <returns>A new instance of <see cref="ResxPhoneFakerProvider"/>.</returns>
    public static ResxPhoneFakerProvider Create(CultureInfo culture) => new(culture);

    /// <summary>
    /// Initializes a new instance of the <see cref="ResxPhoneFakerProvider"/> class with the specified culture.
    /// </summary>
    /// <param name="culture">The culture associated with the provider.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="culture"/> is null.</exception>
    private ResxPhoneFakerProvider(CultureInfo culture)
        : base(culture, PatternsResources.ResourceManager)
    {
        NumberFormats = LoadDataset(nameof(PatternsResources.PhoneNumbers));
        MobileNumberFormats = LoadDataset(nameof(PatternsResources.PhoneMobileNumbers));
        InternationalNumberFormats = LoadDataset(nameof(PatternsResources.PhoneInternationalNumbers));
    }

    /// <inheritdoc />
    public IReadOnlyList<string> NumberFormats { get; }

    /// <inheritdoc />
    public IReadOnlyList<string> MobileNumberFormats { get; }

    /// <inheritdoc />
    public IReadOnlyList<string> InternationalNumberFormats { get; }
}
