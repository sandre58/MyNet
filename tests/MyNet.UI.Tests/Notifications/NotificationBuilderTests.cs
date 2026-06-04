// -----------------------------------------------------------------------
// <copyright file="NotificationBuilderTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using FluentAssertions;
using Moq;
using MyNet.UI.Notifications;
using MyNet.UI.Notifications.Models;
using Xunit;

namespace MyNet.UI.Tests.Notifications;

public sealed class NotificationBuilderTests
{
    [Fact]
    public void Build_ReturnsMessageNotificationWithDefaults()
    {
        var notification = new NotificationBuilder()
            .WithMessage("hello")
            .AsSuccess()
            .WithTitle("Title")
            .Build();

        notification.Should().BeOfType<MessageNotification>();
        notification.Message.Should().Be("hello");
        notification.Title.Should().Be("Title");
        notification.Severity.Should().Be(NotificationSeverity.Success);
    }

    [Fact]
    public void Build_Deduplicable_ReturnsDeduplicableMessageNotification()
    {
        var notification = new NotificationBuilder()
            .WithMessage("dup")
            .Deduplicable()
            .Build();

        notification.Should().BeOfType<DeduplicableMessageNotification>();
    }

    [Fact]
    public void Build_OnClick_ReturnsActionNotification()
    {
        var notification = new NotificationBuilder()
            .WithMessage("click me")
            .AsWarning()
            .OnClick(_ => { })
            .Build();

        notification.Should().BeOfType<ActionNotification>();
        var action = (ActionNotification)notification;
        action.Severity.Should().Be(NotificationSeverity.Warning);
        action.Action.Should().NotBeNull();
    }

    [Fact]
    public void Build_ClosableWithoutAction_ReturnsActionNotification()
    {
        var notification = new NotificationBuilder()
            .WithMessage("closable")
            .Closable()
            .Build();

        notification.Should().BeOfType<ActionNotification>();
        ((ActionNotification)notification).IsClosable.Should().BeTrue();
        ((ActionNotification)notification).Action.Should().BeNull();
    }

    [Fact]
    public void Publish_WithPublisher_PublishesBuiltNotification()
    {
        var publisher = new Mock<INotificationPublisher>();

        new NotificationBuilder(publisher.Object)
            .WithMessage("done")
            .AsSuccess()
            .Publish();

        publisher.Verify(
            x => x.Publish(It.Is<MessageNotification>(n =>
                n.Message == "done" && n.Severity == NotificationSeverity.Success)),
            Times.Once);
    }

    [Fact]
    public void Publish_WithoutPublisher_ThrowsInvalidOperationException()
    {
        var act = () => new NotificationBuilder()
            .WithMessage("fail")
            .Publish();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Notify_Extension_ReturnsBuilderBoundToPublisher()
    {
        var publisher = new Mock<INotificationPublisher>();

        publisher.Object.Notify()
            .WithMessage("via extension")
            .AsError()
            .Publish();

        publisher.Verify(
            x => x.Publish(It.Is<MessageNotification>(n => n.Severity == NotificationSeverity.Error)),
            Times.Once);
    }
}
