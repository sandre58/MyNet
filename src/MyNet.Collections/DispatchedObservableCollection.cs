// -----------------------------------------------------------------------
// <copyright file="DispatchedObservableCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MyNet.Collections;

/// <summary>
/// An observable collection that schedules notifications on a specific scheduler (typically UI thread).
/// It only owns notification dispatching responsibility.
/// </summary>
/// <typeparam name="T">The type of items in the collection.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="DispatchedObservableCollection{T}"/> class with an existing inner collection.
/// </remarks>
/// <param name="inner">The wrapped collection.</param>
/// <param name="dispatcher">The dispatcher used for notifications.</param>
public sealed class DispatchedObservableCollection<T>(ObservableRangeCollection<T> inner, ICollectionEventDispatcher dispatcher) : ObservableCollectionDecorator<T>(inner)
{
    private readonly ICollectionEventDispatcher _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

    /// <summary>
    /// Initializes a new instance of the <see cref="DispatchedObservableCollection{T}"/> class with a scheduler.
    /// </summary>
    /// <param name="dispatcher">The dispatcher used for notifications.</param>
    public DispatchedObservableCollection(ICollectionEventDispatcher dispatcher)
        : this([], dispatcher)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DispatchedObservableCollection{T}"/> class with initial capacity and a scheduler.
    /// Optimized for known collection sizes to avoid resizing.
    /// </summary>
    /// <param name="capacity">The initial capacity to pre-allocate.</param>
    /// <param name="dispatcher">The dispatcher used for notifications.</param>
    public DispatchedObservableCollection(int capacity, ICollectionEventDispatcher dispatcher)
        : this(new ObservableRangeCollection<T>(capacity), dispatcher)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DispatchedObservableCollection{T}"/> class with elements from a list and a scheduler.
    /// </summary>
    /// <param name="list">The list whose elements are copied to the new collection.</param>
    /// <param name="dispatcher">The dispatcher used for notifications.</param>
    public DispatchedObservableCollection(IList<T> list, ICollectionEventDispatcher dispatcher)
        : this(new ObservableRangeCollection<T>(list), dispatcher)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DispatchedObservableCollection{T}"/> class with elements from a collection and a scheduler.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new collection.</param>
    /// <param name="dispatcher">The dispatcher used for notifications.</param>
    public DispatchedObservableCollection(IEnumerable<T> collection, ICollectionEventDispatcher dispatcher)
        : this(new ObservableRangeCollection<T>(collection), dispatcher)
    {
    }

    /// <summary>
    /// Handles collection changed events from the inner collection and dispatches them on the specified scheduler.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    protected override void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_dispatcher.CheckAccess())
        {
            base.OnCollectionChanged(this, e);
            return;
        }

        _dispatcher.Dispatch(() => base.OnCollectionChanged(this, e));
    }

    /// <summary>
    /// Handles property changed events from the inner collection and dispatches them on the specified scheduler.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_dispatcher.CheckAccess())
        {
            base.OnPropertyChanged(this, e);
            return;
        }

        _dispatcher.Dispatch(() => base.OnPropertyChanged(this, e));
    }
}
