// -----------------------------------------------------------------------
// <copyright file="UiRegistrationTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MyNet.UI.Navigation;
using MyNet.UI.ViewModels.Preferences;
using MyNet.UI.ViewModels.Shell;
using Xunit;

namespace MyNet.UI.Tests.DependencyInjection;

public sealed class UiRegistrationTests
{
    [Fact]
    public void AddUi_registers_core_services()
    {
        var services = new ServiceCollection();
        services.AddUi();
        using var provider = services.BuildServiceProvider();

        provider.GetService<INavigationService>().Should().NotBeNull();
        provider.GetService<IShellService>().Should().NotBeNull();
    }

    [Fact]
    public void AddUi_with_builder_can_add_shell_preferences()
    {
        var services = new ServiceCollection();
        services.AddUi(b => b.AddShellPreferences());

        services.Any(d => d.ServiceType == typeof(DisplayPreferencesViewModel)).Should().BeTrue();
    }
}
