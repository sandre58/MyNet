// -----------------------------------------------------------------------
// <copyright file="ViewLocatorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MyNet.UI.Locators;
using MyNet.UI.Tests.ViewModels;
using MyNet.UI.Tests.Views;
using Xunit;

namespace MyNet.UI.Tests.Locators;

public class ViewLocatorTests
{
    // --- DI-first resolution ---
    [Fact]
    public void Get_ReturnsRegisteredInstance_WhenInDI()
    {
        var services = new ServiceCollection();
        services.AddTransient<PersonView>();
        var provider = services.BuildServiceProvider();

        var sut = new ViewLocator(provider);

        var result = sut.Get<PersonView>();

        result.Should().NotBeNull().And.BeOfType<PersonView>();
    }

    [Fact]
    public void Get_FallsBackToActivator_WhenNotInDI()
    {
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();

        var sut = new ViewLocator(provider);

        // PersonView is not registered → Activator.CreateInstance fallback
        var result = sut.Get<PersonView>();

        result.Should().NotBeNull().And.BeOfType<PersonView>();
    }

    [Fact]
    public void Get_Generic_ReturnsTypedInstance()
    {
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();

        var sut = new ViewLocator(provider);

        var result = sut.Get<PersonView>();

        result.Should().NotBeNull();
    }

    [Fact]
    public void Get_ThrowsArgumentNullException_WhenTypeIsNull()
    {
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();

        var sut = new ViewLocator(provider);

        var act = () => sut.Get(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}

public class ViewModelLocatorTests
{
    // --- DI-only resolution ---
    [Fact]
    public void Get_ReturnsDIInstance_WhenRegistered()
    {
        var services = new ServiceCollection();
        services.AddTransient<PersonViewModel>();
        var provider = services.BuildServiceProvider();

        var sut = new ViewModelLocator(provider);

        var result = sut.Get<PersonViewModel>();

        result.Should().NotBeNull().And.BeOfType<PersonViewModel>();
    }

    [Fact]
    public void Get_ThrowsInvalidOperationException_WhenNotRegistered()
    {
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();

        var sut = new ViewModelLocator(provider);

        var act = sut.Get<PersonViewModel>;

        act.Should().Throw<InvalidOperationException>();
    }
}
