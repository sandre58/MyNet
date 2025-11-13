// -----------------------------------------------------------------------
// <copyright file="ThreadSafeObservableCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyNet.Utilities.Collections;

/// <summary>
/// An observable collection that provides thread-safe operations and optional UI dispatch.
/// Optimized to minimize lock contention and support concurrent reads.
/// </summary>
/// <typeparam name="T">The type of items in the collection.</typeparam>
public class ThreadSafeObservableCollection<T> : OptimizedObservableCollection<T>, IDisposable
{
#if NET9_0_OR_GREATER
    private readonly Lock _localLock = new();
#else
    // ReaderWriterLockSlim for better concurrent read performance
    // Using SupportsRecursion to handle nested locks in derived classes
    private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.SupportsRecursion);
#endif

    private readonly Action<Action>? _notifyOnUi;
    private readonly bool _useAsyncNotifications;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadSafeObservableCollection{T}"/> class.
    /// </summary>
    /// <param name="notifyOnUi">Optional action used to marshal notifications on the UI thread.</param>
    /// <param name="useAsyncNotifications">If true, notifications are sent asynchronously to avoid blocking.</param>
    public ThreadSafeObservableCollection(Action<Action>? notifyOnUi = null, bool useAsyncNotifications = true)
    {
        _notifyOnUi = notifyOnUi;
        _useAsyncNotifications = useAsyncNotifications;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadSafeObservableCollection{T}"/> class with initial capacity.
    /// </summary>
    public ThreadSafeObservableCollection(int capacity, Action<Action>? notifyOnUi = null, bool useAsyncNotifications = true)
        : base(capacity)
    {
        _notifyOnUi = notifyOnUi;
        _useAsyncNotifications = useAsyncNotifications;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadSafeObservableCollection{T}"/> class with elements from a list.
    /// </summary>
    public ThreadSafeObservableCollection(Collection<T> list, Action<Action>? notifyOnUi = null, bool useAsyncNotifications = true)
        : base(list)
    {
        _notifyOnUi = notifyOnUi;
        _useAsyncNotifications = useAsyncNotifications;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadSafeObservableCollection{T}"/> class with elements from a collection.
    /// </summary>
    public ThreadSafeObservableCollection(IEnumerable<T> collection, Action<Action>? notifyOnUi = null, bool useAsyncNotifications = true)
        : base(collection)
    {
        _notifyOnUi = notifyOnUi;
        _useAsyncNotifications = useAsyncNotifications;
    }

    /// <inheritdoc />
    public override event NotifyCollectionChangedEventHandler? CollectionChanged;

    #region Thread-Safe Operations

    protected override void InsertItem(int index, T item) => ExecuteWriteLocked(() => base.InsertItem(index, item));

    protected override void MoveItem(int oldIndex, int newIndex) => ExecuteWriteLocked(() => base.MoveItem(oldIndex, newIndex));

    protected override void RemoveItem(int index) => ExecuteWriteLocked(() => base.RemoveItem(index));

    protected override void SetItem(int index, T item) => ExecuteWriteLocked(() => base.SetItem(index, item));

    protected override void ClearItems() => ExecuteWriteLocked(() =>
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            base.ClearItems();
        });

    #endregion

    #region Thread-Safe Batch Operations

    /// <summary>
    /// Thread-safe AddRange.
    /// </summary>
    public new void AddRange(IEnumerable<T> collection) => ExecuteWriteLocked(() => base.AddRange(collection));

    /// <summary>
    /// Thread-safe InsertRange.
    /// </summary>
    public new void InsertRange(IEnumerable<T> collection, int index) => ExecuteWriteLocked(() => base.InsertRange(collection, index));

    /// <summary>
    /// Thread-safe Load.
    /// </summary>
    public new void Load(IEnumerable<T> items) => ExecuteWriteLocked(() => base.Load(items));

    /// <summary>
    /// Thread-safe RemoveRange.
    /// </summary>
    public new void RemoveRange(int index, int count) => ExecuteWriteLocked(() => base.RemoveRange(index, count));

    /// <summary>
    /// Thread-safe RemoveAll.
    /// </summary>
    public new int RemoveAll(Func<T, bool> predicate) => ExecuteWriteLocked(() => base.RemoveAll(predicate));

    #endregion

    #region Notification Handling

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        var collectionChanged = CollectionChanged;
        if (collectionChanged == null) return;

        // Create a snapshot of handlers to avoid holding lock during notification
        var handlers = collectionChanged.GetInvocationList().OfType<NotifyCollectionChangedEventHandler>().ToArray();

        using (BlockReentrancy())
        {
            NotifyCollectionChanged(e, handlers);
        }
    }

    protected virtual void InvokeNotifyCollectionChanged(NotifyCollectionChangedEventHandler notifyEventHandler, NotifyCollectionChangedEventArgs e) => notifyEventHandler.Invoke(this, e);

    private void NotifyCollectionChanged(NotifyCollectionChangedEventArgs e, NotifyCollectionChangedEventHandler[] handlers)
    {
        foreach (var handler in handlers)
        {
            try
            {
                if (_notifyOnUi is not null)
                {
                    if (_useAsyncNotifications)
                    {
                        // Fire and forget - don't block
                        _ = Task.Run(() => _notifyOnUi(() => InvokeNotifyCollectionChanged(handler, e)));
                    }
                    else
                    {
                        // Synchronous UI notification
                        _notifyOnUi(() => InvokeNotifyCollectionChanged(handler, e));
                    }
                }
                else
                {
                    // No UI marshaling needed
                    InvokeNotifyCollectionChanged(handler, e);
                }
            }
            catch (TaskCanceledException)
            {
                // Operation canceled by the system
            }
            catch (Exception ex)
            {
                // Log or handle notification errors
                System.Diagnostics.Debug.WriteLine($"Error notifying collection changed: {ex.Message}");
            }
        }
    }

    #endregion

    #region Lock Helpers

    /// <summary>
    /// Executes an action under a write lock.
    /// </summary>
    protected void ExecuteWriteLocked(Action action)
    {
#if NET9_0_OR_GREATER
        _localLock.Enter();
        try
        {
            action();
        }
        finally
        {
            _localLock.Exit();
        }
#else
        _lock.EnterWriteLock();
        try
        {
            action();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
#endif
    }

    /// <summary>
    /// Executes a function under a write lock and returns the result.
    /// </summary>
    protected TResult ExecuteWriteLocked<TResult>(Func<TResult> func)
    {
#if NET9_0_OR_GREATER
        _localLock.Enter();
        try
        {
            return func();
        }
        finally
        {
            _localLock.Exit();
        }
#else
        _lock.EnterWriteLock();
        try
        {
            return func();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
#endif
    }

    /// <summary>
    /// Executes an action under a read lock (for reads that don't modify collection).
    /// Only available on .NET 8/10 (not .NET 9+ Lock which doesn't support read locks).
    /// </summary>
    protected void ExecuteReadLocked(Action action)
    {
#if NET9_0_OR_GREATER
        // Lock type doesn't support read locks, fallback to write lock
        ExecuteWriteLocked(action);
#else
        _lock.EnterReadLock();
        try
        {
            action();
        }
        finally
        {
            _lock.ExitReadLock();
        }
#endif
    }

    #endregion

    #region IDisposable Support

    /// <summary>
    /// Disposes resources used by this collection.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
#if !NET9_0_OR_GREATER
            _lock.Dispose();
#endif
        }

        _disposed = true;
    }

    /// <summary>
    /// Disposes the collection and releases locks.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
