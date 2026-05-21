// -----------------------------------------------------------------------
// <copyright file="ViewFactoryTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MyNet.UI.Locators;
using MyNet.UI.Locators.Conventions;
using MyNet.UI.Locators.Factories;
using MyNet.UI.Tests.ViewModels;
using MyNet.UI.Tests.Views;
using Xunit;

namespace MyNet.UI.Tests.Locators;

public class ViewFactoryTests
{
    private static ViewFactory BuildFactory(ITypeResolver resolver, IServiceProvider? provider = null)
    {
        provider ??= new ServiceCollection().BuildServiceProvider();
        return new(resolver, new ViewLocator(provider));
    }

    [Fact]
    public void CreateView_ReturnsView_WhenResolverFindsMapping()
    {
        var resolver = new TypeResolver([]);
        resolver.Register(typeof(PersonViewModel), typeof(PersonView));

        var factory = BuildFactory(resolver);

        var result = factory.CreateView(typeof(PersonViewModel));

        result.Should().NotBeNull().And.BeOfType<PersonView>();
    }

    [Fact]
    public void CreateView_ReturnsNull_WhenResolverReturnsNull()
    {
        var resolver = new TypeResolver([]);

        var factory = BuildFactory(resolver);

        var result = factory.CreateView(typeof(PersonViewModel));

        result.Should().BeNull();
    }

    [Fact]
    public void CreateView_Generic_ReturnsTypedView()
    {
        var resolver = new TypeResolver([]);
        resolver.Register(typeof(PersonViewModel), typeof(PersonView));

        var factory = BuildFactory(resolver);

        var result = factory.CreateView<PersonViewModel, PersonView>();

        result.Should().NotBeNull().And.BeOfType<PersonView>();
    }

    [Fact]
    public void CreateView_WithConvention_ResolvesView()
    {
        var resolver = new TypeResolver([new SuffixConvention()]);

        var factory = BuildFactory(resolver);

        var result = factory.CreateView(typeof(PersonViewModel));

        result.Should().NotBeNull().And.BeOfType<PersonView>();
    }

    [Fact]
    public void AddViewLocators_DI_WiresEverythingCorrectly()
    {
        var services = new ServiceCollection();
        services.AddViewLocators(resolver => resolver.Register(typeof(PersonViewModel), typeof(PersonView)));

        var provider = services.BuildServiceProvider();

        var factory = provider.GetRequiredService<IViewFactory>();
        var result = factory.CreateView(typeof(PersonViewModel));

        result.Should().NotBeNull().And.BeOfType<PersonView>();
    }
}
