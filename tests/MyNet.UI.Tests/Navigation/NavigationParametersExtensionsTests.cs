// -----------------------------------------------------------------------
// <copyright file="NavigationParametersExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;
using MyNet.UI.Navigation.Models;
using Xunit;

namespace MyNet.UI.Tests.Navigation;

public sealed class NavigationParametersExtensionsTests
{
    [Fact]
    public void ToNavigationParameters_WithNull_ReturnsEmptyBag() => NavigationParametersExtensions.ToNavigationParameters(null)
        .Should()
        .BeEquivalentTo(new NavigationParameters());

    [Fact]
    public void ToNavigationParameters_WithNavigationParameters_CreatesCopy()
    {
        var source = new NavigationParameters().Set("Key", "Value");

        var converted = source.ToNavigationParameters();

        converted.Should().NotBeSameAs(source);
        converted.Get<string>("Key").Should().Be("Value");
    }

    [Fact]
    public void ToNavigationParameters_WithKeyValuePairs_CreatesBag()
    {
        IEnumerable<KeyValuePair<string, object?>> pairs =
        [
            new("Id", 42),
            new("Name", "alpha")
        ];

        var converted = pairs.ToNavigationParameters();

        converted.Get<int>("Id").Should().Be(42);
        converted.Get<string>("Name").Should().Be("alpha");
    }

    [Fact]
    public void ToNavigationParameters_WithAnonymousObject_CreatesBag()
    {
        var converted = new { Id = 7, Mode = NavigationMode.Forward }.ToNavigationParameters();

        converted.Get<int>("Id").Should().Be(7);
        converted.TryGetValue("Mode", out NavigationMode mode).Should().BeTrue();
        mode.Should().Be(NavigationMode.Forward);
    }

    [Fact]
    public void Get_OnExtension_ReturnsValueFromNavigationParametersBag()
    {
        INavigationParameters parameters = new NavigationParameters().Set("Count", 3);

        parameters.Get("Count", 0).Should().Be(3);
    }

    [Fact]
    public void Get_OnExtension_ReturnsDefaultWhenBagIsNotNavigationParameters()
    {
        INavigationParameters parameters = new OtherNavigationParameters();

        parameters.Get("Missing", 99).Should().Be(99);
    }

    [Fact]
    public void TryGetValue_OnExtension_ReturnsFalseWhenBagIsNotNavigationParameters()
    {
        INavigationParameters parameters = new OtherNavigationParameters();

        parameters.TryGetValue("Key", out string? value).Should().BeFalse();
        value.Should().BeNull();
    }

    private sealed class OtherNavigationParameters : INavigationParameters;
}
