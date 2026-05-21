// -----------------------------------------------------------------------
// <copyright file="SchedulerCollectionEventDispatcher.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reactive.Concurrency;
using MyNet.Utilities.Collections;

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

    /// <summary>
    /// Checks if the current thread has access to the scheduler, allowing for optimized dispatching when already on the correct thread.
    /// </summary>
    /// <returns>true if the current thread has access; otherwise, false.</returns>
    public bool CheckAccess() => _scheduler.Now == DateTimeOffset.Now;

    /// <inheritdoc />
    public void Dispatch(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        _scheduler.Schedule(action);
    }
}
