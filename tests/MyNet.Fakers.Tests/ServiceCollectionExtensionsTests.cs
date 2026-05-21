// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MyNet.Fakers.Contacts;
using MyNet.Fakers.Geography;
using MyNet.Fakers.Identity;
using MyNet.Fakers.Internet;
using MyNet.Fakers.Media;
using MyNet.Fakers.Text;
using Xunit;

namespace MyNet.Fakers.Tests;

public sealed class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddFakers_ShouldRegisterAllDefaultFakersAsSingletons()
    {
        var services = new ServiceCollection();

        services.AddFakers();

        services.Should().ContainSingle(x => x.ServiceType == typeof(IPhoneFaker) && x.ImplementationType == typeof(PhoneFaker) && x.Lifetime == ServiceLifetime.Singleton);
        services.Should().ContainSingle(x => x.ServiceType == typeof(IMailFaker) && x.ImplementationType == typeof(MailFaker) && x.Lifetime == ServiceLifetime.Singleton);
        services.Should().ContainSingle(x => x.ServiceType == typeof(IStreetFaker) && x.ImplementationType == typeof(StreetFaker) && x.Lifetime == ServiceLifetime.Singleton);
        services.Should().ContainSingle(x => x.ServiceType == typeof(IAddressFaker) && x.ImplementationType == typeof(AddressFaker) && x.Lifetime == ServiceLifetime.Singleton);
        services.Should().ContainSingle(x => x.ServiceType == typeof(ICountryFaker) && x.ImplementationType == typeof(CountryFaker) && x.Lifetime == ServiceLifetime.Singleton);
        services.Should().ContainSingle(x => x.ServiceType == typeof(INameFaker) && x.ImplementationType == typeof(NameFaker) && x.Lifetime == ServiceLifetime.Singleton);
        services.Should().ContainSingle(x => x.ServiceType == typeof(IDomainFaker) && x.ImplementationType == typeof(DomainFaker) && x.Lifetime == ServiceLifetime.Singleton);
        services.Should().ContainSingle(x => x.ServiceType == typeof(IColorFaker) && x.ImplementationType == typeof(ColorFaker) && x.Lifetime == ServiceLifetime.Singleton);
        services.Should().ContainSingle(x => x.ServiceType == typeof(ITextFaker) && x.ImplementationType == typeof(TextFaker) && x.Lifetime == ServiceLifetime.Singleton);
        services.Should().ContainSingle(x => x.ServiceType == typeof(IFakeDataGenerator) && x.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddFakers_WithDI_ShouldResolveSuccessfully()
    {
        var services = new ServiceCollection();
        services.AddFakers();

        IServiceProvider provider = services.BuildServiceProvider();
        var faker = provider.GetRequiredService<IFakeDataGenerator>();

        faker.Should().NotBeNull();
        faker.Names.Should().NotBeNull();
        faker.Phones.Should().NotBeNull();
        faker.Addresses.Should().NotBeNull();
    }
}
