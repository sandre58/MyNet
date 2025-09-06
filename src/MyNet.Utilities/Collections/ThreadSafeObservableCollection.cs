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
using System.Threading.Tasks;

#if NET9_0_OR_GREATER
using System.Threading;
#endif

namespace MyNet.Utilities.Collections;

/// <summary>
/// An observable collection that provides thread-safe operations and optional UI dispatch.
/// </summary>
/// <typeparam name="T">The type of items in the collection.</typeparam>
public class ThreadSafeObservableCollection<T> : OptimizedObservableCollection<T>
{
#if NET9_0_OR_GREATER
    private readonly Lock _localLock = new();
#else
    private readonly object _localLock = new();
#endif

    private readonly Action<Action>? _notifyOnUi;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadSafeObservableCollection{T}"/> class.
    /// </summary>
    /// <param name="notifyOnUi">Optional action used to marshal notifications on the UI thread.</param>
    public ThreadSafeObservableCollection(Action<Action>? notifyOnUi = null) => _notifyOnUi = notifyOnUi;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadSafeObservableCollection{T}"/> class that contains elements copied from the specified list.
    /// </summary>
    /// <param name="list">The list whose elements are copied to the new collection.</param>
    /// <param name="notifyOnUi">Optional UI notifier.</param>
    public ThreadSafeObservableCollection(Collection<T> list, Action<Action>? notifyOnUi = null)
        : base(list) => _notifyOnUi = notifyOnUi;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadSafeObservableCollection{T}"/> class that contains elements copied from the specified collection.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new collection.</param>
    /// <param name="notifyOnUi">Optional UI notifier.</param>
    public ThreadSafeObservableCollection(IEnumerable<T> collection, Action<Action>? notifyOnUi = null)
        : base(collection) => _notifyOnUi = notifyOnUi;

    /// <inheritdoc />
    public override event NotifyCollectionChangedEventHandler? CollectionChanged;

    protected override void InsertItem(int index, T item) => ExecuteThreadSafe(() => base.InsertItem(index, item));

    protected override void MoveItem(int oldIndex, int newIndex) => ExecuteThreadSafe(() => base.MoveItem(oldIndex, newIndex));

    protected override void RemoveItem(int index) => ExecuteThreadSafe(() => base.RemoveItem(index));

    protected override void SetItem(int index, T item) => ExecuteThreadSafe(() => base.SetItem(index, item));

    protected override void ClearItems() => ExecuteThreadSafe(base.ClearItems);

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        var collectionChanged = CollectionChanged;
        if (collectionChanged == null) return;

        using (BlockReentrancy())
        {
            NotifyCollectionChanged(e, collectionChanged);
        }
    }

    protected virtual void InvokeNotifyCollectionChanged(NotifyCollectionChangedEventHandler notifyEventHandler, NotifyCollectionChangedEventArgs e) => notifyEventHandler.Invoke(this, e);

    /// <summary>
    /// Executes the provided action under a lock to ensure thread-safety.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    protected void ExecuteThreadSafe(Action action)
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
        lock (_localLock)
        {
            action();
        }
#endif
    }

    private void NotifyCollectionChanged(NotifyCollectionChangedEventArgs e, NotifyCollectionChangedEventHandler collectionChanged)
    {
        foreach (var notifyEventHandler in collectionChanged.GetInvocationList().OfType<NotifyCollectionChangedEventHandler>())
        {
            try
            {
                if (_notifyOnUi is not null)
                    _notifyOnUi(() => InvokeNotifyCollectionChanged(notifyEventHandler, e));
                else
                    InvokeNotifyCollectionChanged(notifyEventHandler, e);
            }
            catch (TaskCanceledException)
            {
                // Operation has canceled by the system
            }
        }
    }
}
