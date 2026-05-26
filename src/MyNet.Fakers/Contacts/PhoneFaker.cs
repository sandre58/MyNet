// -----------------------------------------------------------------------
// <copyright file="PhoneFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Generator;
using MyNet.Globalization.Localization.Providers;
using MyNet.Text.Randomize;

namespace MyNet.Fakers.Contacts;

/// <summary>
/// Current implementation of <see cref="IPhoneFaker"/>.
/// </summary>
public sealed class PhoneFaker(ITextRandomGenerator patternGenerator, IRandomGenerator random, ICultureScopedServiceSource<IPhoneFakerProvider> source)
    : PatternFakerBase<IPhoneFakerProvider>(patternGenerator, random, source), IPhoneFaker
{
    /// <inheritdoc />
    public string Number(CultureInfo? culture = null) => Randomize(x => x.NumberFormats, culture);

    /// <inheritdoc />
    public string MobileNumber(CultureInfo? culture = null) => Randomize(x => x.MobileNumberFormats, culture);

    /// <inheritdoc />
    public string InternationalNumber(CultureInfo? culture = null) => Randomize(x => x.InternationalNumberFormats, culture);
}
