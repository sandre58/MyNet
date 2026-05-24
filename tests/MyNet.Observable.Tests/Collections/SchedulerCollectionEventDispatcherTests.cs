// -----------------------------------------------------------------------
// <copyright file="SchedulerCollectionEventDispatcherTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reactive.Concurrency;
using System.Threading;
using FluentAssertions;
using MyNet.Observable.Collections;
using Xunit;

namespace MyNet.Observable.Tests.Collections;

public sealed class SchedulerCollectionEventDispatcherTests
{
    [Fact]
    public void CheckAccess_OnImmediateScheduler_ShouldReturnTrue()
    {
        var dispatcher = new SchedulerCollectionEventDispatcher(ImmediateScheduler.Instance);

        dispatcher.CheckAccess().Should().BeTrue();
    }

    [Fact]
    public void CheckAccess_OnCurrentThreadScheduler_ShouldReturnTrue()
    {
        var dispatcher = new SchedulerCollectionEventDispatcher(CurrentThreadScheduler.Instance);

        dispatcher.CheckAccess().Should().BeTrue();
    }

    [Fact]
    public void Dispatch_OnEventLoopScheduler_ShouldCaptureAffinedThread()
    {
        using var scheduler = new EventLoopScheduler();
        var dispatcher = new SchedulerCollectionEventDispatcher(scheduler);
        var onLoop = false;

        scheduler.Schedule(() => dispatcher.Dispatch(() => onLoop = dispatcher.CheckAccess()));

        SpinWait.SpinUntil(() => onLoop, TimeSpan.FromSeconds(2));

        onLoop.Should().BeTrue();
        dispatcher.CheckAccess().Should().BeFalse();
    }
}
