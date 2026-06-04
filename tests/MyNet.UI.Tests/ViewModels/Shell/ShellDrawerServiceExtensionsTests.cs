// -----------------------------------------------------------------------
// <copyright file="ShellDrawerServiceExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Moq;
using MyNet.UI.ViewModels.Shell;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Shell;

public sealed class ShellDrawerServiceExtensionsTests
{
    [Fact]
    public void ShowNotificationsDrawer_CallsSetNotificationsDrawerWithShow()
    {
        var shell = new Mock<IShellDrawerService>();

        shell.Object.ShowNotificationsDrawer();

        shell.Verify(x => x.SetNotificationsDrawer(ShellDrawerAction.Show), Times.Once);
    }

    [Fact]
    public void ToggleFileMenuDrawer_CallsSetFileMenuDrawerWithToggle()
    {
        var shell = new Mock<IShellDrawerService>();

        shell.Object.ToggleFileMenuDrawer();

        shell.Verify(x => x.SetFileMenuDrawer(ShellDrawerAction.Toggle), Times.Once);
    }
}
