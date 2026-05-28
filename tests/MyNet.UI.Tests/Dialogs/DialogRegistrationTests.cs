// -----------------------------------------------------------------------
// <copyright file="DialogRegistrationTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MyNet.UI.Dialogs;
using MyNet.UI.Dialogs.ContentDialogs;
using MyNet.UI.Dialogs.FileDialogs;
using MyNet.UI.Dialogs.MessageBox;
using Xunit;

namespace MyNet.UI.Tests.Dialogs;

public class DialogRegistrationTests
{
    [Fact]
    public void AddDialogs_RegistersCoreServices()
    {
        var services = new ServiceCollection();
        services.AddDialogs();
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IDialogService>().Should().BeOfType<DialogService>();
        provider.GetRequiredService<IContentDialogService>().Should().BeOfType<ContentDialogService>();
        provider.GetRequiredService<IMessageBoxService>().Should().BeOfType<MessageBoxService>();
        provider.GetRequiredService<IMessageBoxFactory>().Should().BeOfType<DefaultMessageBoxFactory>();
        provider.GetRequiredService<IFileDialogService>().Should().BeOfType<CancelledFileDialogService>();
    }

    [Fact]
    public void AddDialogs_RegistersHeadlessAndPresenterStrategies()
    {
        var services = new ServiceCollection();
        services.AddDialogs();
        using var provider = services.BuildServiceProvider();

        var strategies = provider.GetServices<IDialogStrategy>().ToList();
        strategies.Should().Contain(s => s is HeadlessDialogStrategy);
        strategies.Should().Contain(s => s is PresenterDialogStrategy);
    }

    [Fact]
    public void AddDialogs_IsIdempotent()
    {
        var services = new ServiceCollection();
        services.AddDialogs();
        services.AddDialogs();
        using var provider = services.BuildServiceProvider();

        provider.GetServices<IDialogStrategy>()
            .OfType<HeadlessDialogStrategy>()
            .Should()
            .HaveCount(1);
    }
}
