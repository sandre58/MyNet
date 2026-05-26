// -----------------------------------------------------------------------
// <copyright file="SchedulerCollectionEventDispatcher.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reactive.Concurrency;
using MyNet.Collections;

namespace MyNet.Observable.Collections;

/// <summary>
/// Adapter that bridges an Rx scheduler to the collection event dispatcher abstraction.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SchedulerCollectionEventDispatcher"/> class.
/// </remarks>
/// <param name="scheduler">The scheduler used to dispatch actions.</param>
public sealed class SchedulerCollectionEventDispatcher(IScheduler scheduler) : ICollectionEventDispatcher
{
    private readonly IScheduler _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
    private volatile int _schedulerThreadId = -1;

    /// <summary>
    /// Checks if the current thread can invoke collection notifications without marshaling.
    /// </summary>
    /// <returns><c>true</c> when already on the scheduler's execution context; otherwise <c>false</c>.</returns>
    public bool CheckAccess()
    {
        if (ReferenceEquals(_scheduler, ImmediateScheduler.Instance) ||
            ReferenceEquals(_scheduler, CurrentThreadScheduler.Instance))
        {
            return true;
        }

        var threadId = _schedulerThreadId;
        return threadId >= 0 && threadId == Environment.CurrentManagedThreadId;
    }

    /// <inheritdoc />
    public void Dispatch(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);

        _scheduler.Schedule(() =>
        {
            _schedulerThreadId = Environment.CurrentManagedThreadId;
            action();
        });
    }
}
