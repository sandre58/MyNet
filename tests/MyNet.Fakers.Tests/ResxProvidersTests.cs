// -----------------------------------------------------------------------
// <copyright file="ResxProvidersTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using FluentAssertions;
using MyNet.Fakers.Contacts;
using MyNet.Fakers.Geography;
using MyNet.Fakers.Identity;
using MyNet.Fakers.Internet;
using Xunit;

namespace MyNet.Fakers.Tests;

public sealed class ResxProvidersTests
{
    [Fact]
    public void NameProvider_Create_ShouldLoadDatasets()
    {
        var provider = ResxNameFakerProvider.Create(CultureInfo.GetCultureInfo("en"));

        provider.MaleFirstNames.Should().NotBeEmpty();
        provider.FemaleFirstNames.Should().NotBeEmpty();
        provider.LastNames.Should().NotBeEmpty();
    }

    [Fact]
    public void PhoneProvider_Create_ShouldLoadDatasets()
    {
        var provider = ResxPhoneFakerProvider.Create(CultureInfo.GetCultureInfo("en"));

        provider.NumberFormats.Should().NotBeEmpty();
        provider.MobileNumberFormats.Should().NotBeEmpty();
        provider.InternationalNumberFormats.Should().NotBeEmpty();
    }

    [Fact]
    public void DomainProvider_Create_ShouldLoadDatasets()
    {
        var provider = ResxDomainFakerProvider.Create(CultureInfo.GetCultureInfo("en"));

        provider.Domains.Should().NotBeEmpty();
        provider.Hosts.Should().NotBeEmpty();
    }

    [Fact]
    public void AddressProvider_Create_ShouldLoadDatasets()
    {
        var provider = ResxAddressFakerProvider.Create(CultureInfo.GetCultureInfo("en"));

        provider.Cities.Should().NotBeEmpty();
        provider.StreetFormats.Should().NotBeEmpty();
        provider.PostalCodeFormats.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithNullCulture_ShouldThrow()
    {
        Action name = () => ResxNameFakerProvider.Create(null!);
        Action phone = () => ResxPhoneFakerProvider.Create(null!);
        Action domain = () => ResxDomainFakerProvider.Create(null!);
        Action address = () => ResxAddressFakerProvider.Create(null!);

        name.Should().Throw<ArgumentNullException>();
        phone.Should().Throw<ArgumentNullException>();
        domain.Should().Throw<ArgumentNullException>();
        address.Should().Throw<ArgumentNullException>();
    }
}
