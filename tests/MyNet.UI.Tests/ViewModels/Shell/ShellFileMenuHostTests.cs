// -----------------------------------------------------------------------
// <copyright file="ShellFileMenuHostTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Moq;
using MyNet.UI.Dialogs;
using MyNet.UI.Services;
using MyNet.UI.ViewModels.Shell;
using MyNet.UI.ViewModels.Workspace;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Shell;

public class ShellFileMenuHostTests
{
    [Fact]
    public void OpenFileMenuContent_SetsDrawerAndContent()
    {
        var workspace = new TestWorkspaceViewModel();
        var drawer = new TestFileMenuDrawer();
        var host = CreateHost([workspace], drawer);

        host.OpenFileMenuContent<TestWorkspaceViewModel>();

        drawer.IsFileMenuOpen.Should().BeTrue();
        host.FileMenuViewModel.Content.Should().BeSameAs(workspace);
    }

    [Fact]
    public void CloseFileMenuContent_ClearsDrawerAndContent()
    {
        var drawer = new TestFileMenuDrawer();
        var host = CreateHost([new TestWorkspaceViewModel()], drawer);
        host.OpenFileMenuContent<TestWorkspaceViewModel>();

        host.CloseFileMenuContent();

        drawer.IsFileMenuOpen.Should().BeFalse();
        host.FileMenuViewModel.Content.Should().BeNull();
    }

    [Fact]
    public void SetFileMenuContentVisibility_ToggleWhenOpenWithSameContent_ClosesDrawer()
    {
        var drawer = new TestFileMenuDrawer();
        var host = CreateHost([new TestWorkspaceViewModel()], drawer);
        host.OpenFileMenuContent<TestWorkspaceViewModel>();

        host.SetFileMenuContentVisibility<TestWorkspaceViewModel>(ShellDrawerAction.Toggle);

        drawer.IsFileMenuOpen.Should().BeFalse();
        host.FileMenuViewModel.Content.Should().BeNull();
    }

    [Fact]
    public void SetDrawer_Toggle_FlipsDrawerWithoutChangingContent()
    {
        var drawer = new TestFileMenuDrawer { IsFileMenuOpen = false };
        var host = CreateHost([new TestWorkspaceViewModel()], drawer);

        host.SetDrawer(ShellDrawerAction.Toggle);

        drawer.IsFileMenuOpen.Should().BeTrue();
        host.FileMenuViewModel.Content.Should().BeNull();
    }

    private static ShellFileMenuHost CreateHost(TestWorkspaceViewModel[] content, IShellFileMenuDrawer drawer)
    {
        var dialogService = new Mock<IDialogService>();
        dialogService.Setup(x => x.HasOpenedDialogs).Returns(false);

        var fileMenu = new FileMenuViewModel(
            content,
            Mock.Of<IAppCommandsService>(),
            dialogService.Object);

        return new ShellFileMenuHost(fileMenu, drawer);
    }

    private sealed class TestFileMenuDrawer : IShellFileMenuDrawer
    {
        public bool IsFileMenuOpen { get; set; }
    }

    private sealed class TestWorkspaceViewModel : WorkspaceViewModel;
}
