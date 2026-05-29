// -----------------------------------------------------------------------
// <copyright file="ShellServiceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Moq;
using MyNet.UI.ViewModels.Shell;
using MyNet.UI.ViewModels.Shell.Host;
using MyNet.UI.ViewModels.Shell.Services;
using MyNet.UI.ViewModels.Workspace;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Shell;

public class ShellServiceTests
{
    [Fact]
    public void IsAvailable_WhenNoHost_ReturnsFalse()
    {
        var service = new ShellService(new ShellHostProvider());

        service.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public void CloseDrawers_WhenNoHost_Throws()
    {
        var service = new ShellService(new ShellHostProvider());

        var act = service.CloseDrawers;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void OpenFileMenuContent_WhenHostWithoutFileMenu_Throws()
    {
        var provider = new ShellHostProvider();
        provider.Attach(Mock.Of<IShellHost>());
        var service = new ShellService(provider);

        var act = service.OpenFileMenuContent<TestWorkspaceViewModel>;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void SetFileMenuContentVisibility_DelegatesToHost()
    {
        var host = new Mock<IShellHostWithFileMenu>();
        host.As<IShellCapabilities>().Setup(x => x.HasFileMenu).Returns(true);
        var provider = new ShellHostProvider();
        provider.Attach(host.Object);
        var service = new ShellService(provider);

        service.SetFileMenuContentVisibility<TestWorkspaceViewModel>(ShellDrawerAction.Show);

        host.Verify(x => x.SetFileMenuContentVisibility<TestWorkspaceViewModel>(ShellDrawerAction.Show), Times.Once);
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Used as a type parameter for testing.")]
    private sealed class TestWorkspaceViewModel : WorkspaceViewModel;
}
