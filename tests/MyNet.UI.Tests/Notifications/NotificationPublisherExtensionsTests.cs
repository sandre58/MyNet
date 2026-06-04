// -----------------------------------------------------------------------
// <copyright file="NotificationPublisherExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using FluentAssertions;
using Moq;
using MyNet.Primitives.Exceptions;
using MyNet.UI.Notifications;
using MyNet.UI.Notifications.Models;
using MyNet.UI.ViewModels.Shell.Taskbar;
using Xunit;

namespace MyNet.UI.Tests.Notifications;

public sealed class NotificationPublisherExtensionsTests
{
    [Fact]
    public void PublishException_PublishesErrorNotificationWithInnerMessage()
    {
        var publisher = new Mock<INotificationPublisher>();
        var exception = new InvalidOperationException("outer", new InvalidOperationException("inner"));

        publisher.Object.PublishException(exception);

        publisher.Verify(
            x => x.Publish(It.Is<MessageNotification>(n =>
                n.Message == "inner" &&
                n.Severity == NotificationSeverity.Error)),
            Times.Once);
    }

    [Fact]
    public void PublishException_WithTranslatableInnerException_UsesResourceKey()
    {
        var publisher = new Mock<INotificationPublisher>();
        var exception = new InvalidOperationException("outer", new TranslatableException("Error.Key"));

        publisher.Object.PublishException(exception);

        publisher.Verify(
            x => x.Publish(It.Is<MessageNotification>(n => n.Message == "Error.Key")),
            Times.Once);
    }

    [Fact]
    public void PublishException_WithTaskbarProgress_SetsErrorState()
    {
        var publisher = new Mock<INotificationPublisher>();
        var taskbar = new Mock<ITaskbarProgressSource>();

        publisher.Object.PublishException(new InvalidOperationException("fail"), showInTaskBar: true, taskbarProgress: taskbar.Object);

        taskbar.Verify(x => x.SetError(), Times.Once);
    }

    [Fact]
    public void PublishException_WithReportTaskBarCallback_InvokesCallback()
    {
        var publisher = new Mock<INotificationPublisher>();
        TaskbarProgressState? state = null;
        double? value = null;

        publisher.Object.PublishException(
            new InvalidOperationException("fail"),
            showInTaskBar: true,
            reportTaskBar: (progressState, progressValue) =>
            {
                state = progressState;
                value = progressValue;
            });

        state.Should().Be(TaskbarProgressState.Error);
        value.Should().Be(1);
    }

    [Theory]
    [InlineData(NotificationSeverity.Success)]
    [InlineData(NotificationSeverity.Error)]
    [InlineData(NotificationSeverity.Warning)]
    [InlineData(NotificationSeverity.Information)]
    public void PublishMessage_PublishesMessageNotificationWithSeverity(NotificationSeverity severity)
    {
        var publisher = new Mock<INotificationPublisher>();

        publisher.Object.PublishMessage("body", severity, "title");

        publisher.Verify(
            x => x.Publish(It.Is<MessageNotification>(n =>
                n.Message == "body" &&
                n.Title == "title" &&
                n.Severity == severity)),
            Times.Once);
    }

    [Fact]
    public void PublishSuccess_PublishesSuccessNotification()
    {
        var publisher = new Mock<INotificationPublisher>();

        publisher.Object.PublishSuccess("ok");

        publisher.Verify(
            x => x.Publish(It.Is<MessageNotification>(n =>
                n.Message == "ok" && n.Severity == NotificationSeverity.Success)),
            Times.Once);
    }

    [Fact]
    public void PublishError_PublishesErrorNotification()
    {
        var publisher = new Mock<INotificationPublisher>();

        publisher.Object.PublishError("fail", "Error");

        publisher.Verify(
            x => x.Publish(It.Is<MessageNotification>(n =>
                n.Message == "fail" && n.Title == "Error" && n.Severity == NotificationSeverity.Error)),
            Times.Once);
    }

    [Fact]
    public void PublishException_WithOnClick_PublishesActionNotification()
    {
        var publisher = new Mock<INotificationPublisher>();

        publisher.Object.PublishException(
            new InvalidOperationException("fail"),
            onClick: _ => { });

        publisher.Verify(
            x => x.Publish(It.IsAny<ActionNotification>()),
            Times.Once);
    }
}
