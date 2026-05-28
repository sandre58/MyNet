// -----------------------------------------------------------------------
// <copyright file="BusyLoadingRegistrationTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MyNet.UI.Loading;
using MyNet.UI.ViewModels;
using Xunit;

namespace MyNet.UI.Tests.Loading;

public class BusyLoadingRegistrationTests
{
    [Fact]
    public void AddBusyLoading_RegistersSingletonBusyService()
    {
        var services = new ServiceCollection();
        services.AddBusy();
        using var provider = services.BuildServiceProvider();

        var busy = provider.GetRequiredService<IBusyService>();

        busy.Should().BeOfType<BusyService>();
    }

    [Fact]
    public void ViewModelBase_UsesLocalBusyServiceSeparateFromApplicationSingleton()
    {
        var services = new ServiceCollection();
        services.AddBusy();
        using var provider = services.BuildServiceProvider();

        var vm = new TestViewModel();
        var appBusy = provider.GetRequiredService<IBusyService>();

        vm.BusyService.Should().NotBeSameAs(appBusy);
        vm.BusyService.Should().BeOfType<BusyService>();
    }

    private sealed class TestViewModel : ViewModelBase;
}
