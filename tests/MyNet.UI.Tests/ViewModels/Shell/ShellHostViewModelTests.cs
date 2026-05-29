// -----------------------------------------------------------------------
// <copyright file="ShellHostViewModelTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using FluentAssertions;
using Moq;
using MyNet.Globalization.Culture;
using MyNet.UI.Dialogs;
using MyNet.UI.Notifications;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Services;
using MyNet.UI.Tests.Loading;
using MyNet.UI.Theming;
using MyNet.UI.Threading;
using MyNet.UI.ViewModels.Shell;
using MyNet.UI.ViewModels.Shell.Chrome;
using MyNet.UI.ViewModels.Shell.FileMenu;
using MyNet.UI.ViewModels.Shell.Host;
using MyNet.UI.ViewModels.Shell.Notifications;
using MyNet.UI.ViewModels.Shell.Taskbar;
using MyNet.UI.ViewModels.Workspace;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Shell;

public class ShellHostViewModelTests
{
    [Fact]
    public void Constructor_WithoutFileMenu_HasFileMenuIsFalse()
    {
        var sut = CreateSut(fileMenuContent: null);

        sut.HasFileMenu.Should().BeFalse();
        sut.FileMenuHost.Should().BeNull();
    }

    [Fact]
    public void OpenFileMenuContent_WithoutFileMenu_Throws()
    {
        var sut = CreateSut(fileMenuContent: null);

        var act = () => sut.OpenFileMenuContent<TestWorkspaceViewModel>();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void OpenFileMenuContent_WithFileMenu_SetsDrawerAndContent()
    {
        var workspace = new TestWorkspaceViewModel();
        var sut = CreateSut([workspace]);

        sut.OpenFileMenuContent<TestWorkspaceViewModel>();

        sut.IsFileMenuOpen.Should().BeTrue();
        sut.FileMenuViewModel.Content.Should().BeSameAs(workspace);
    }

    [Fact]
    public void CloseFileMenuContent_ClearsDrawerAndContent()
    {
        var sut = CreateSut([new TestWorkspaceViewModel()]);
        sut.OpenFileMenuContent<TestWorkspaceViewModel>();

        sut.CloseFileMenuContent();

        sut.IsFileMenuOpen.Should().BeFalse();
        sut.FileMenuViewModel.Content.Should().BeNull();
    }

    [Fact]
    public void SetFileMenuContentVisibility_ToggleWhenOpenWithSameContent_ClosesDrawer()
    {
        var sut = CreateSut([new TestWorkspaceViewModel()]);
        sut.OpenFileMenuContent<TestWorkspaceViewModel>();

        sut.SetFileMenuContentVisibility<TestWorkspaceViewModel>(ShellDrawerAction.Toggle);

        sut.IsFileMenuOpen.Should().BeFalse();
        sut.FileMenuViewModel.Content.Should().BeNull();
    }

    [Fact]
    public void SetNotificationsDrawer_Toggle_FlipsState()
    {
        var sut = CreateSut([new TestWorkspaceViewModel()]);

        sut.SetNotificationsDrawer(ShellDrawerAction.Show);
        sut.IsNotificationsOpen.Should().BeTrue();

        sut.SetNotificationsDrawer(ShellDrawerAction.Toggle);
        sut.IsNotificationsOpen.Should().BeFalse();
    }

    [Fact]
    public void OpenFileMenu_WithMutuallyExclusiveDrawers_ClosesNotifications()
    {
        var sut = CreateSut([new TestWorkspaceViewModel()]);
        sut.SetNotificationsDrawer(ShellDrawerAction.Show);

        sut.OpenFileMenuContent<TestWorkspaceViewModel>();

        sut.IsNotificationsOpen.Should().BeFalse();
        sut.IsFileMenuOpen.Should().BeTrue();
    }

    [Fact]
    public void Attach_RegistersShellHostProvider()
    {
        var provider = new ShellHostProvider();
        var sut = CreateSut([new TestWorkspaceViewModel()], provider);

        provider.Current.Should().BeSameAs(sut);

        sut.Dispose();
        provider.Current.Should().BeNull();
    }

    private static ShellHostViewModel CreateSut(
        TestWorkspaceViewModel[]? fileMenuContent = null,
        ShellHostProvider? shellHostProvider = null,
        ShellOptions? options = null)
    {
        var items = new ObservableCollection<INotification>();
        var notificationsManager = new Mock<INotificationsManager>();
        notificationsManager.Setup(x => x.Notifications).Returns(new ReadOnlyObservableCollection<INotification>(items));

        var lightBase = new Mock<IThemeBase>();
        lightBase.Setup(x => x.IsDark).Returns(false);
        var darkBase = new Mock<IThemeBase>();
        darkBase.Setup(x => x.IsDark).Returns(true);

        var theme = new Mock<IThemeService>();
        theme.Setup(x => x.CurrentTheme).Returns(new Theme(lightBase.Object, "#000000", "#000000"));

        var registry = new Mock<IThemeBaseRegistry>();
        registry.Setup(x => x.Light).Returns(lightBase.Object);
        registry.Setup(x => x.Dark).Returns(darkBase.Object);

        var dialogService = new Mock<IDialogService>();
        dialogService.Setup(x => x.HasOpenedDialogs).Returns(false);

        var notificationsVm = new ShellNotificationsViewModel(
            notificationsManager.Object,
            new DefaultSchedulerProvider());

        FileMenuViewModel? fileMenu = fileMenuContent is null
            ? null
            : new FileMenuViewModel(fileMenuContent);

        var cultureService = new CultureService(SupportedCultures.English);
        var applicationInfo = new ApplicationInfo();

        return new ShellHostViewModel(
            applicationInfo,
            notificationsVm,
            new ShellCultureViewModel(cultureService, [SupportedCultures.English]),
            new ShellThemeViewModel(theme.Object, registry.Object),
            Mock.Of<IAppCommandsService>(),
            new NoOpBusyService(),
            new TaskbarProgressSource(),
            dialogService.Object,
            new ShellNotificationsDrawerCoordinator(),
            fileMenu,
            shellHostProvider,
            options);
    }

    private sealed class TestWorkspaceViewModel : WorkspaceViewModel;
}
