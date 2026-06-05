// -----------------------------------------------------------------------
// <copyright file="CountryHumanizeTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MyNet.Globalization;
using MyNet.Humanizer;
using MyNet.Humanizer.Facade;
using Xunit;

namespace MyNet.Geography.Tests;

public sealed class CountryHumanizeTests
{
    [Fact]
    public void Humanize_ShouldUseCountryDisplayTextStrategy_NotSmartEnumFallback()
    {
        var services = new ServiceCollection();
        services.AddLocalization();
        services.AddHumanizer();
        services.AddGeographyLocalization();

        using var serviceProvider = services.BuildServiceProvider();
        serviceProvider.UseLocalization();
        serviceProvider.UseDisplayText();

        var result = Country.Germany.Humanize(culture: CultureInfo.GetCultureInfo("fr-FR"));

        result.Should().Be("Allemagne");
    }
}
