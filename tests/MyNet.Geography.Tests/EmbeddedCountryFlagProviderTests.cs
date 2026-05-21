// -----------------------------------------------------------------------
// <copyright file="EmbeddedCountryFlagProviderTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MyNet.Utilities.Geography;
using Xunit;

namespace MyNet.Geography.Tests;

public sealed class EmbeddedCountryFlagProviderTests
{
    [Fact]
    public void Open_ShouldReturnStream_ForExistingCountryAndSize()
    {
        using var serviceProvider = new ServiceCollection()
            .AddGeography()
            .BuildServiceProvider();
        var sut = serviceProvider.GetRequiredService<ICountryFlagProvider>();

        using var stream = sut.Open(Country.France, FlagSize.Pixel32);

        stream.Should().NotBeNull();
        stream.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GetBytes_ShouldReturnPngBytes_ForExistingCountryAndSize()
    {
        using var serviceProvider = new ServiceCollection()
            .AddGeography()
            .BuildServiceProvider();
        var sut = serviceProvider.GetRequiredService<ICountryFlagProvider>();

        var bytes = sut.GetBytes(Country.France, FlagSize.Pixel32);

        bytes.Should().NotBeNull();
        bytes.Should().NotBeEmpty();
        bytes[0].Should().Be(137);
        bytes[1].Should().Be(80);
        bytes[2].Should().Be(78);
        bytes[3].Should().Be(71);
    }

    [Fact]
    public void OpenAndGetBytes_ShouldReturnNull_ForUnknownFlagSize()
    {
        using var serviceProvider = new ServiceCollection()
            .AddGeography()
            .BuildServiceProvider();
        var sut = serviceProvider.GetRequiredService<ICountryFlagProvider>();

        const FlagSize unknownSize = (FlagSize)999;

        sut.Open(Country.France, unknownSize).Should().BeNull();
        sut.GetBytes(Country.France, unknownSize).Should().BeNull();
    }
}
