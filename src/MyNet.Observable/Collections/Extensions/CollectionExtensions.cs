// -----------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reactive.Concurrency;
using MyNet.Observable.Collections;
using MyNet.Utilities.Collections;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Scheduling helpers for observable range collections.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Wraps an observable range collection so notifications are dispatched on the specified Rx scheduler.
    /// </summary>
    public static DispatchedObservableCollection<T> Scheduled<T>(this ObservableRangeCollection<T> collection, IScheduler scheduler)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(scheduler);
        return new(collection, new SchedulerCollectionEventDispatcher(scheduler));
    }
}
