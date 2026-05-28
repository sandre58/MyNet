// -----------------------------------------------------------------------
// <copyright file="NotificationsManagerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reactive.Concurrency;
using System.Threading;
using FluentAssertions;
using MyNet.UI.Notifications;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Threading;
using Xunit;

namespace MyNet.UI.Tests.Notifications;

public class NotificationsManagerTests
{
    [Fact]
    public void CloseRequested_RemovesNotification()
    {
        using var service = new NotificationService();
        var scheduler = new CountingSchedulerProvider();
        using var sut = new NotificationsManager(service, scheduler);
        var notification = new ClosableNotification("message", "title", NotificationSeverity.Information);

        service.Publish(notification);
        sut.Notifications.Should().ContainSingle();

        notification.RequestClose();

        sut.Notifications.Should().BeEmpty();
    }

    [Fact]
    public void DeduplicableNotification_IsNotAddedTwice()
    {
        using var service = new NotificationService();
        var scheduler = new CountingSchedulerProvider();
        using var sut = new NotificationsManager(service, scheduler);

        service.Publish(new DeduplicableMessageNotification("same", severity: NotificationSeverity.Warning));
        service.Publish(new DeduplicableMessageNotification("same", severity: NotificationSeverity.Warning));

        sut.Notifications.Should().ContainSingle();
    }

    [Fact]
    public void Remove_UsesUiScheduler()
    {
        using var service = new NotificationService();
        var scheduler = new CountingSchedulerProvider();
        using var sut = new NotificationsManager(service, scheduler);
        var notification = new MessageNotification("message");

        service.Publish(notification);
        var scheduleCountBeforeRemove = scheduler.UiScheduler.ScheduleCallCount;

        sut.Remove(notification);

        scheduler.UiScheduler.ScheduleCallCount.Should().BeGreaterThan(scheduleCountBeforeRemove);
    }

    [Fact]
    public void Clear_UsesUiScheduler()
    {
        using var service = new NotificationService();
        var scheduler = new CountingSchedulerProvider();
        using var sut = new NotificationsManager(service, scheduler);
        service.Publish(new MessageNotification("message"));
        var scheduleCountBeforeClear = scheduler.UiScheduler.ScheduleCallCount;

        sut.Clear();

        scheduler.UiScheduler.ScheduleCallCount.Should().BeGreaterThan(scheduleCountBeforeClear);
    }

    private sealed class CountingSchedulerProvider : ISchedulerProvider
    {
        public CountingScheduler UiScheduler { get; } = new();

        public IScheduler Ui => UiScheduler;

        public IScheduler Background => UiScheduler;
    }

    private sealed class CountingScheduler : IScheduler
    {
        private int _scheduleCallCount;

        public int ScheduleCallCount => _scheduleCallCount;

        public DateTimeOffset Now => DateTimeOffset.UtcNow;

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            Interlocked.Increment(ref _scheduleCallCount);
            return action(this, state);
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            Interlocked.Increment(ref _scheduleCallCount);
            return action(this, state);
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            Interlocked.Increment(ref _scheduleCallCount);
            return action(this, state);
        }
    }
}
