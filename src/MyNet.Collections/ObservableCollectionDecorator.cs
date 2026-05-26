// -----------------------------------------------------------------------
// <copyright file="ObservableCollectionDecorator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MyNet.Collections;

/// <summary>
/// An observable collection that schedules notifications on a specific scheduler (typically UI thread).
/// It only owns notification dispatching responsibility.
/// </summary>
/// <typeparam name="T">The type of items in the collection.</typeparam>
public abstract class ObservableCollectionDecorator<T> : IObservableRangeCollection<T>, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableCollectionDecorator{T}"/> class with an existing inner collection.
    /// </summary>
    /// <param name="inner">The wrapped collection.</param>
    protected ObservableCollectionDecorator(IObservableRangeCollection<T> inner)
    {
        Inner = inner ?? throw new ArgumentNullException(nameof(inner));

        Inner.CollectionChanged += OnCollectionChanged;
        Inner.PropertyChanged += OnPropertyChanged;
    }

    /// <summary>
    /// Gets the inner observable range collection that actually holds the items and raises notifications.
    /// </summary>
    protected IObservableRangeCollection<T> Inner { get; }

    /// <inheritdoc />
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public virtual int Count => Inner.Count;

    /// <inheritdoc />
    public virtual bool IsReadOnly => false;

    /// <inheritdoc />
    public virtual T this[int index]
    {
        get => Inner[index];
        set => Inner[index] = value;
    }

    /// <inheritdoc />
    public virtual void Add(T item) => Inner.Add(item);

    /// <summary>
    /// Adds a range of items to the collection.
    /// </summary>
    public virtual void AddRange(IEnumerable<T> items) => Inner.AddRange(items);

    /// <inheritdoc />
    public virtual void Clear() => Inner.Clear();

    /// <inheritdoc />
    public virtual bool Contains(T item) => Inner.Contains(item);

    /// <inheritdoc />
    public virtual void CopyTo(T[] array, int arrayIndex) => Inner.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public virtual IEnumerator<T> GetEnumerator() => Inner.GetEnumerator();

    /// <inheritdoc />
    public virtual int IndexOf(T item) => Inner.IndexOf(item);

    /// <inheritdoc />
    public virtual void Insert(int index, T item) => Inner.Insert(index, item);

    /// <summary>
    /// Moves an item from one index to another.
    /// </summary>
    public virtual void Move(int oldIndex, int newIndex) => Inner.Move(oldIndex, newIndex);

    /// <inheritdoc />
    public virtual bool Remove(T item) => Inner.Remove(item);

    /// <inheritdoc />
    public virtual void RemoveAt(int index) => Inner.RemoveAt(index);

    /// <summary>
    /// Replaces the current items with the specified items.
    /// </summary>
    public virtual void Load(IEnumerable<T> items) => Inner.Load(items);

    /// <summary>
    /// Removes a range of items from the collection.
    /// </summary>
    public virtual void RemoveRange(int index, int count) => Inner.RemoveRange(index, count);

    /// <summary>
    /// Exposes RemoveAll from the wrapped range collection.
    /// </summary>
    public virtual int RemoveAll(Func<T, bool> predicate) => Inner.RemoveAll(predicate);

    /// <summary>
    /// Exposes InsertRange from the wrapped range collection.
    /// </summary>
    public virtual void InsertRange(IEnumerable<T> items, int index) => Inner.InsertRange(items, index);

    /// <summary>
    /// Exposes capacity configuration from the wrapped range collection.
    /// </summary>
    public virtual void SetCapacity(int capacity) => Inner.SetCapacity(capacity);

    /// <summary>
    /// Gets the current capacity of the wrapped range collection.
    /// </summary>
    public virtual int Capacity => Inner.Capacity;

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Suspends count notifications, allowing batch updates without triggering count change events until the returned disposable is disposed.
    /// </summary>
    /// <returns>A disposable that, when disposed, resumes count notifications.</returns>
    public virtual IDisposable SuspendCount() => Inner.SuspendCount();

    /// <summary>
    /// Suspends collection and property change notifications, allowing batch updates without triggering events until the returned disposable is disposed.
    /// </summary>
    /// <returns>A disposable that, when disposed, resumes collection and property change notifications.</returns>
    public virtual IDisposable SuspendNotifications() => Inner.SuspendNotifications();

    /// <summary>
    /// Handles collection changed events from the inner collection and dispatches them on the specified scheduler.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    protected virtual void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var collectionChanged = CollectionChanged;

        collectionChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Handles property changed events from the inner collection and dispatches them on the specified scheduler.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    protected virtual void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var propertyChanged = PropertyChanged;

        propertyChanged?.Invoke(this, e);
    }

    #region IDisposable Support

    private bool _isDisposed;

    /// <summary>
    /// Disposes the object and releases all resources.
    /// </summary>
    /// <param name="disposing">Indicates whether the method is called from Dispose (true) or from a finalizer (false).</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        if (disposing)
        {
            Inner.CollectionChanged -= OnCollectionChanged;
            Inner.PropertyChanged -= OnPropertyChanged;
        }

        _isDisposed = true;
    }

    /// <summary>
    /// Disposes the object and releases all resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion IDisposable Support
}
