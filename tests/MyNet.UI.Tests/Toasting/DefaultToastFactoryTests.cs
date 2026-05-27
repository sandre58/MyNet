// -----------------------------------------------------------------------
// <copyright file="DefaultToastFactoryTests.cs" company="Stephane ANDRE">
// Copyright (c) Stephane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Toasting;
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
