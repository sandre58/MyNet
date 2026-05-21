// -----------------------------------------------------------------------
// <copyright file="NotificationServiceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using FluentAssertions;
using MyNet.UI.Notifications;
using MyNet.UI.Notifications.Models;
using Xunit;

namespace MyNet.UI.Tests.Notifications;

public class NotificationServiceTests
{
    [Fact]
    public void Publish_ThrowsObjectDisposedException_AfterDispose()
    {
        var sut = new NotificationService();
        sut.Dispose();

        var act = () => sut.Publish(new MessageNotification("test"));

        act.Should().Throw<ObjectDisposedException>();
    }
}
