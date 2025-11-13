// -----------------------------------------------------------------------
// <copyright file="UiObservableCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.UI.Collections;

/// <summary>
/// An observable collection that automatically schedules notifications on the UI thread.
/// Optimized for WPF/Avalonia UI binding with batch operations and async notifications.
/// </summary>
/// <typeparam name="T">The type of items in the collection.</typeparam>
public class UiObservableCollection<T> : ScheduleObservableCollection<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UiObservableCollection{T}"/> class that schedules notifications on the UI thread.
    /// </summary>
    /// <param name="useAsyncNotifications">If true, notifications are sent asynchronously (default: true).</param>
    public UiObservableCollection(bool useAsyncNotifications = true)
        : base(() => Threading.Scheduler.UiOrCurrent, useAsyncNotifications) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UiObservableCollection{T}"/> class with initial capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity to pre-allocate.</param>
    /// <param name="useAsyncNotifications">If true, notifications are sent asynchronously (default: true).</param>
    public UiObservableCollection(int capacity, bool useAsyncNotifications = true)
        : base(capacity, () => Threading.Scheduler.UiOrCurrent, useAsyncNotifications) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UiObservableCollection{T}"/> class with elements from a list.
    /// </summary>
    /// <param name="list">The list whose elements are copied to the new collection.</param>
    /// <param name="useAsyncNotifications">If true, notifications are sent asynchronously (default: true).</param>
    public UiObservableCollection(IList<T> list, bool useAsyncNotifications = true)
        : base(list, () => Threading.Scheduler.UiOrCurrent, useAsyncNotifications) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UiObservableCollection{T}"/> class with elements from a collection.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new collection.</param>
    /// <param name="useAsyncNotifications">If true, notifications are sent asynchronously (default: true).</param>
    public UiObservableCollection(IEnumerable<T> collection, bool useAsyncNotifications = true)
      : base(collection, () => Threading.Scheduler.UiOrCurrent, useAsyncNotifications) { }
}
