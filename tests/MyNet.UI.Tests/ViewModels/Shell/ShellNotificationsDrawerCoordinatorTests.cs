// -----------------------------------------------------------------------
// <copyright file="ShellNotificationsDrawerCoordinatorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Windows.Input;
using FluentAssertions;
using Moq;
using MyNet.UI.Commands;
using MyNet.UI.Notifications;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Threading;
using MyNet.UI.ViewModels.Shell;
using MyNet.UI.ViewModels.Shell.Notifications;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Shell;

public class ShellNotificationsDrawerCoordinatorTests
{
    [Fact]
    public void Bind_WhenNotificationsEmpty_ClosesDrawer()
    {
        var coordinator = new ShellNotificationsDrawerCoordinator();
        var host = new TestNotificationsDrawerHost { IsNotificationsOpen = true };
        var notificationsVm = CreateNotificationsViewModel([]);

        coordinator.Bind(host, notificationsVm);

        host.IsNotificationsOpen.Should().BeFalse();
    }

    [Fact]
    public void Unbind_StopsReactingToNotificationChanges()
    {
        var items = new ObservableCollection<INotification>();
        var notificationsManager = new Mock<INotificationsManager>();
        notificationsManager.Setup(x => x.Notifications).Returns(new ReadOnlyObservableCollection<INotification>(items));

        var notificationsVm = new ShellNotificationsViewModel(
            notificationsManager.Object,
            new DefaultSchedulerProvider());

        var coordinator = new ShellNotificationsDrawerCoordinator();
        var host = new TestNotificationsDrawerHost { IsNotificationsOpen = true };

        coordinator.Bind(host, notificationsVm);
        host.IsNotificationsOpen = true;
        coordinator.Unbind();

        items.Add(new MessageNotification("test", severity: NotificationSeverity.Information));

        host.IsNotificationsOpen.Should().BeTrue();
    }

    private static ShellNotificationsViewModel CreateNotificationsViewModel(INotification[] seed)
    {
        var items = new ObservableCollection<INotification>(seed);
        var notificationsManager = new Mock<INotificationsManager>();
        notificationsManager.Setup(x => x.Notifications).Returns(new ReadOnlyObservableCollection<INotification>(items));

        return new(
            notificationsManager.Object,
            new DefaultSchedulerProvider());
    }

    private sealed class TestNotificationsDrawerHost : IShellNotificationsDrawerHost
    {
        public bool IsNotificationsOpen { get; set; }

        public ICommand ToggleNotificationsCommand { get; } =
            RelayCommandFactory.Default.Create(static () => { }, static () => true);
    }
}
