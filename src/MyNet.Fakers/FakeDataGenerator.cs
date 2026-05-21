// -----------------------------------------------------------------------
// <copyright file="FakeDataGenerator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Fakers.Contacts;
using MyNet.Fakers.Geography;
using MyNet.Fakers.Identity;
using MyNet.Fakers.Internet;
using MyNet.Fakers.Media;
using MyNet.Fakers.Text;

namespace MyNet.Fakers;

/// <summary>
/// Current implementation of IFakeDataGenerator that provides various faker services for generating random data such as names, phones, mails, countries, addresses, streets, identities, texts, domains, and colors.
/// </summary>
public class FakeDataGenerator(
    INameFaker names,
    IPhoneFaker phones,
    IMailFaker mails,
    ICountryFaker countries,
    IAddressFaker addresses,
    IStreetFaker streets,
    IIdentityFaker identities,
    ITextFaker texts,
    IDomainFaker domains,
    IColorFaker colors) : IFakeDataGenerator
{
    /// <inheritdoc/>
    public INameFaker Names { get; } = names;

    /// <inheritdoc/>
    public IPhoneFaker Phones { get; } = phones;

    /// <inheritdoc/>
    public IMailFaker Mails { get; } = mails;

    /// <inheritdoc/>
    public ICountryFaker Countries { get; } = countries;

    /// <inheritdoc/>
    public IAddressFaker Addresses { get; } = addresses;

    /// <inheritdoc/>
    public IStreetFaker Streets { get; } = streets;

    /// <inheritdoc/>
    public IIdentityFaker Identities { get; } = identities;

    /// <inheritdoc/>
    public ITextFaker Texts { get; } = texts;

    /// <inheritdoc/>
    public IDomainFaker Domains { get; } = domains;

    /// <inheritdoc/>
    public IColorFaker Colors { get; } = colors;
}
