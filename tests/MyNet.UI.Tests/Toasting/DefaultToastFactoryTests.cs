// -----------------------------------------------------------------------
// <copyright file="DefaultToastFactoryTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using MyNet.Primitives;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Toasting;
using MyNet.UI.Toasting.Settings;
using Xunit;

namespace MyNet.UI.Tests.Toasting;

public class DefaultToastFactoryTests
{
    [Fact]
    public void Create_WithCustomIClosableNotification_AssignsCloseCommand()
    {
        var sut = new DefaultToastFactory();
        var notification = new CustomClosableNotification("message", isClosable: true);

        var toast = sut.Create(notification);
        toast.CloseCommand.Should().NotBeNull();

        toast.CloseCommand!.Execute(null);
        notification.RequestCloseCount.Should().Be(1);
    }

    [Fact]
    public void Create_WithNotClosableNotification_DoesNotAssignCloseCommand()
    {
        var sut = new DefaultToastFactory();
        var notification = new CustomClosableNotification("message", isClosable: false);

        var toast = sut.Create(notification);

        toast.CloseCommand.Should().BeNull();
    }

    [Fact]
    public void Create_UsesToastManagerOptionsAsGlobalDefaults()
    {
        var options = new ToastManagerOptions
        {
            DefaultClosingStrategy = ToastClosingStrategy.Both,
            DefaultFreezeOnMouseEnter = false
        };

        var toast = new DefaultToastFactory(options).Create(new MessageNotification("hello"));

        toast.Settings.ClosingStrategy.Should().Be(ToastClosingStrategy.Both);
        toast.Settings.FreezeOnMouseEnter.Should().BeFalse();
    }

    [Fact]
    public void Create_AppliesNotificationToastSettingsOverrides()
    {
        var notification = new MessageNotification("hello")
        {
            ToastSettings = new()
            {
                Duration = 3.Seconds(),
                FreezeOnMouseEnter = false
            }
        };

        var toast = new DefaultToastFactory().Create(notification);

        toast.Settings.Duration.Should().Be(3.Seconds());
        toast.Settings.FreezeOnMouseEnter.Should().BeFalse();
    }

    [Fact]
    public void Create_WithClosableNotification_UsesBothClosingStrategyByDefault()
    {
        var toast = new DefaultToastFactory().Create(
            new ClosableNotification("message", "title", NotificationSeverity.Information));

        toast.Settings.ClosingStrategy.Should().Be(ToastClosingStrategy.Both);
    }

    [Fact]
    public void Create_WithActionNotification_AssignsClickCommand()
    {
        var clicked = false;
        var notification = new ActionNotification("message", "title", NotificationSeverity.Information, action: _ => clicked = true);

        var toast = new DefaultToastFactory().Create(notification);

        toast.ClickCommand.Should().NotBeNull();
        toast.ClickCommand!.Execute(null);
        clicked.Should().BeTrue();
    }

    private sealed class CustomClosableNotification(string message, bool isClosable) : NotificationBase(message), IClosableNotification
    {
        public int RequestCloseCount { get; private set; }

        public bool IsClosable { get; } = isClosable;

        public event EventHandler<CloseRequestedEventArgs>? CloseRequested;

        public Task<bool> CanCloseAsync() => Task.FromResult(IsClosable);

        public void RequestClose()
        {
            RequestCloseCount++;
            CloseRequested?.Invoke(this, new());
        }
    }
}
