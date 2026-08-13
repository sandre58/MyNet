// -----------------------------------------------------------------------
// <copyright file="NavigationParametersTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.UI.Navigation.Models;
using Xunit;

namespace MyNet.UI.Tests.Navigation;

public class NavigationParametersTests
{
    [Fact]
    public void TryGetValue_ConvertsNumericTypes()
    {
        var sut = new NavigationParameters().Set("Id", 42L);

        sut.TryGetValue("Id", out int id).Should().BeTrue();
        id.Should().Be(42);
    }

    [Fact]
    public void Get_ConvertsStringToGuid()
    {
        var guid = System.Guid.NewGuid();
        var sut = new NavigationParameters().Set("Token", guid.ToString());

        sut.Get<System.Guid>("Token").Should().Be(guid);
    }

    [Fact]
    public void TryGetValue_ParsesEnumFromString()
    {
        var sut = new NavigationParameters().Set("Mode", "Back");

        sut.TryGetValue("Mode", out NavigationMode mode).Should().BeTrue();
        mode.Should().Be(NavigationMode.Back);
    }

    [Fact]
    public void TryGetValue_ParsesEnumFromUnderlyingValue()
    {
        var sut = new NavigationParameters().Set("Mode", 2);

        sut.TryGetValue("Mode", out NavigationMode mode).Should().BeTrue();
        mode.Should().Be(NavigationMode.Forward);
    }

    [Fact]
    public void TryGetValue_ReturnsFalse_WhenConversionIsNotSupported()
    {
        var sut = new NavigationParameters().Set("Payload", new object());

        sut.TryGetValue("Payload", out int _).Should().BeFalse();
    }

    [Fact]
    public void Get_ReturnsDefault_WhenKeyIsMissing()
    {
        var sut = new NavigationParameters();

        sut.Get("Missing", 7).Should().Be(7);
    }
}
