// -----------------------------------------------------------------------
// <copyright file="ShellDrawerCoordinatorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using FluentAssertions;
using Moq;
using MyNet.UI.Dialogs.ContentDialogs;
using MyNet.UI.ViewModels.Shell;
using MyNet.UI.ViewModels.Shell.Drawers;
using MyNet.UI.ViewModels.Shell.Host;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Shell;

public class ShellDrawerCoordinatorTests
{
    [Fact]
    public void DialogOpened_ClosesShellDrawers()
    {
        var host = new Mock<IShellHost>();
        var provider = new ShellHostProvider();
        provider.Attach(host.Object);

        var contentDialogs = new Mock<IContentDialogService>();
        EventHandler<ContentDialogLifecycleEventArgs>? opened = null;
        contentDialogs.SetupAdd(x => x.DialogOpened += It.IsAny<EventHandler<ContentDialogLifecycleEventArgs>>())
            .Callback<EventHandler<ContentDialogLifecycleEventArgs>>(handler => opened = handler);

        _ = new ShellDrawerCoordinator(contentDialogs.Object, provider);

        opened.Should().NotBeNull();
        opened!.Invoke(contentDialogs.Object, new(Mock.Of<IDialog>()));

        host.Verify(x => x.CloseDrawers(), Times.Once);
    }
}
