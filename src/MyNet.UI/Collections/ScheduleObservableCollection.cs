// -----------------------------------------------------------------------
// <copyright file="ScheduleObservableCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using DynamicData;
using DynamicData.Binding;
using MyNet.Utilities.Collections;

namespace MyNet.UI.Collections;

/// <summary>
/// An observable collection that schedules notifications on a specific scheduler (typically UI thread).
/// Inherits all optimizations from ThreadSafeObservableCollection including batch operations,
/// async notifications, and thread-safety.
/// </summary>
/// <typeparam name="T">The type of items in the collection.</typeparam>
public class ScheduleObservableCollection<T> : ThreadSafeObservableCollection<T>, IObservableCollection<T>, IExtendedList<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduleObservableCollection{T}"/> class with a scheduler.
    /// </summary>
    /// <param name="scheduler">Function that returns the scheduler to use for notifications.</param>
    /// <param name="useAsyncNotifications">If true, notifications are sent asynchronously (default: true).</param>
    public ScheduleObservableCollection(Func<IScheduler> scheduler, bool useAsyncNotifications = true)
        : base(x => scheduler().Schedule(x), useAsyncNotifications) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduleObservableCollection{T}"/> class with initial capacity and a scheduler.
    /// Optimized for known collection sizes to avoid resizing.
    /// </summary>
    /// <param name="capacity">The initial capacity to pre-allocate.</param>
    /// <param name="scheduler">Function that returns the scheduler to use for notifications.</param>
    /// <param name="useAsyncNotifications">If true, notifications are sent asynchronously (default: true).</param>
    public ScheduleObservableCollection(int capacity, Func<IScheduler> scheduler, bool useAsyncNotifications = true)
     : base(capacity, x => scheduler().Schedule(x), useAsyncNotifications) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduleObservableCollection{T}"/> class with elements from a list and a scheduler.
    /// </summary>
    /// <param name="list">The list whose elements are copied to the new collection.</param>
    /// <param name="scheduler">Function that returns the scheduler to use for notifications.</param>
    /// <param name="useAsyncNotifications">If true, notifications are sent asynchronously (default: true).</param>
    public ScheduleObservableCollection(IList<T> list, Func<IScheduler> scheduler, bool useAsyncNotifications = true)
        : base(list, x => scheduler().Schedule(x), useAsyncNotifications) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduleObservableCollection{T}"/> class with elements from a collection and a scheduler.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new collection.</param>
    /// <param name="scheduler">Function that returns the scheduler to use for notifications.</param>
    /// <param name="useAsyncNotifications">If true, notifications are sent asynchronously (default: true).</param>
    public ScheduleObservableCollection(IEnumerable<T> collection, Func<IScheduler> scheduler, bool useAsyncNotifications = true)
        : base(collection, x => scheduler().Schedule(x), useAsyncNotifications) { }
}
