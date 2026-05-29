// -----------------------------------------------------------------------
// <copyright file="PreferencesViewModelTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MyNet.UI.Dialogs;
using MyNet.UI.Notifications;
using MyNet.UI.Services;
using MyNet.UI.ViewModels.Preferences;
using MyNet.UI.ViewModels.Workspace;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Preferences;

public class PreferencesViewModelTests
{
    [Fact]
    public void Constructor_RegistersPages()
    {
        var sut = CreateSut(Mock.Of<IPersistentPreferencesService>(), new TestPageViewModel());

        sut.Workspaces.Should().HaveCount(1);
    }

    [Fact]
    public async Task ResetAsync_CallsPreferencesServiceAndChildWorkspaces()
    {
        var preferences = new Mock<IPersistentPreferencesService>();
        var child = new TestPageViewModel();

        var sut = CreateSut(preferences.Object, child);

        await sut.ResetAsync().ConfigureAwait(false);

        preferences.Verify(x => x.Reset(), Times.Once);
        preferences.Verify(x => x.Reset(), Times.Once);
    }

    [Fact]
    public async Task RefreshAsync_CallsPreferencesServiceAndChildWorkspaces()
    {
        var preferences = new Mock<IPersistentPreferencesService>();
        var sut = CreateSut(preferences.Object, new TestPageViewModel());

        await sut.RefreshAsync().ConfigureAwait(false);

        preferences.Verify(x => x.Reload(), Times.Once);
    }

    [Fact]
    public void SaveCore_CallsPreferencesServiceSave()
    {
        var preferences = new Mock<IPersistentPreferencesService>();
        var sut = CreateSut(preferences.Object, new TestPageViewModel());

        typeof(PreferencesViewModel)
            .GetMethod("SaveCore", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(sut, null);

        preferences.Verify(x => x.Save(), Times.Once);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var sut = CreateSut(Mock.Of<IPersistentPreferencesService>(), new TestPageViewModel());

        var act = () =>
        {
            sut.Dispose();
            sut.Dispose();
        };

        act.Should().NotThrow();
    }

    private static PreferencesViewModel CreateSut(
        IPersistentPreferencesService preferencesService,
        IWorkspaceViewModel page)
        => new(
            preferencesService,
            [page],
            Mock.Of<IDialogService>(),
            Mock.Of<INotificationPublisher>());

    private sealed class TestPageViewModel : WorkspaceViewModel;
}
