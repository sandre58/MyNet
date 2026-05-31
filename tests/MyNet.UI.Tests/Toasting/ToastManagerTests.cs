// -----------------------------------------------------------------------
// <copyright file="ToastManagerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reactive.Concurrency;
using FluentAssertions;
using MyNet.UI.Notifications;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Threading;
using MyNet.UI.Toasting;
using MyNet.UI.Toasting.Filters;
using MyNet.UI.Toasting.Models;
using MyNet.UI.Toasting.Settings;
using Xunit;

namespace MyNet.UI.Tests.Toasting;

public class ToastManagerTests
{
    [Fact]
    public void Enqueue_RespectsMaxQueueSize()
    {
        using var service = new NotificationService();
        var options = new ToastManagerOptions { MaxVisibleToasts = 1, MaxQueueSize = 1, DefaultDuration = TimeSpan.FromHours(1) };

        using var sut = new ToastManager(service, new TestSchedulerProvider(), new TestToastFactory(), new AllToastsFilter(), options);

        var first = new MessageNotification("first");
        var second = new MessageNotification("second");
        var third = new MessageNotification("third");

        service.Publish(first);
        service.Publish(second);
        service.Publish(third);

        sut.Toasts.Should().ContainSingle();
        sut.Toasts[0].Notification.Id.Should().Be(first.Id);

        sut.Remove(sut.Toasts[0]);

        sut.Toasts.Should().ContainSingle();
        sut.Toasts[0].Notification.Id.Should().Be(second.Id);

        sut.Remove(sut.Toasts[0]);

        sut.Toasts.Should().BeEmpty();
    }

    [Fact]
    public void CloseRequested_RemovesVisibleToast()
    {
        using var service = new NotificationService();
        using var sut = new ToastManager(service, new TestSchedulerProvider(), new TestToastFactory(), new AllToastsFilter());
        var notification = new ClosableNotification("message", "title", NotificationSeverity.Information);

        service.Publish(notification);

        sut.Toasts.Should().ContainSingle();

        notification.RequestClose();

        sut.Toasts.Should().BeEmpty();
    }

    [Fact]
    public void StartLifetime_SkipsAutoCloseWhenFreezeOnMouseEnter()
    {
        using var service = new NotificationService();
        using var sut = new ToastManager(
            service,
            new TestSchedulerProvider(),
            new FrozenAutoCloseToastFactory(),
            new AllToastsFilter(),
            new ToastManagerOptions { DefaultDuration = TimeSpan.FromMilliseconds(50) });

        service.Publish(new MessageNotification("hello"));

        System.Threading.Thread.Sleep(150);

        sut.Toasts.Should().ContainSingle();
    }

    private sealed class FrozenAutoCloseToastFactory : IToastFactory
    {
        public IToast Create(INotification notification) =>
            new Toast(notification, new()
            {
                ClosingStrategy = ToastClosingStrategy.AutoClose,
                Duration = TimeSpan.FromMilliseconds(50),
                FreezeOnMouseEnter = true
            });
    }

    private sealed class TestToastFactory : IToastFactory
    {
        public IToast Create(INotification notification) =>
            new Toast(notification, new() { ClosingStrategy = ToastClosingStrategy.None });
    }

    private sealed class TestSchedulerProvider : ISchedulerProvider
    {
        public IScheduler Background { get; } = CurrentThreadScheduler.Instance;

        public IScheduler Ui { get; } = CurrentThreadScheduler.Instance;
    }
}
