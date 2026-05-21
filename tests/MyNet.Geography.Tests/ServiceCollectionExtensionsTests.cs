// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MyNet.Humanizer.Display;
using MyNet.Utilities.Geography;
using Xunit;

namespace MyNet.Geography.Tests;

public sealed class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddGeography_ShouldRegisterExpectedServicesAsSingletons()
    {
        var services = new ServiceCollection();

        services.AddGeography();

        var flagProviderRegistration = services.Single(x => x.ServiceType == typeof(ICountryFlagProvider));
        var displayNameRegistration = services.Single(x => x.ServiceType == typeof(IDisplayTextStrategy<Country>));

        flagProviderRegistration.Lifetime.Should().Be(ServiceLifetime.Singleton);
        displayNameRegistration.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddGeography_CalledTwice_ShouldNotDuplicateRegistrations()
    {
        var services = new ServiceCollection();

        services.AddGeography();
        services.AddGeography();

        services.Count(x => x.ServiceType == typeof(ICountryFlagProvider)).Should().Be(1);
        services.Count(x => x.ServiceType == typeof(IDisplayTextStrategy<Country>)).Should().Be(1);
    }
}
