// -----------------------------------------------------------------------
// <copyright file="LockCollectionSynchronizer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;

namespace MyNet.Utilities.Collections;

/// <summary>
/// Collection synchronizer using a reader-writer lock for thread-safe access to collections.
/// </summary>
public sealed class LockCollectionSynchronizer : ICollectionSynchronizer, IDisposable
{
    private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.SupportsRecursion);

    /// <summary>
    /// Initializes a new instance of the <see cref="LockCollectionSynchronizer"/> class.
    /// </summary>
    public LockCollectionSynchronizer() { }

    /// <summary>
    /// Executes the specified action within a write lock, ensuring exclusive access for modifications.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public void Write(Action action)
    {
        _lock.EnterWriteLock();

        try
        {
            action();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Executes the specified function within a write lock, ensuring exclusive access for modifications.
    /// </summary>
    /// <param name="func">The function to execute.</param>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <returns>The result of the function.</returns>
    public TResult Write<TResult>(Func<TResult> func)
    {
        _lock.EnterWriteLock();

        try
        {
            return func();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Executes the specified action within a read lock, allowing concurrent read access.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public void Read(Action action)
    {
        _lock.EnterReadLock();

        try
        {
            action();
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// Executes the specified function within a read lock, allowing concurrent read access.
    /// </summary>
    /// <param name="func">The function to execute.</param>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <returns>The result of the function.</returns>
    public TResult Read<TResult>(Func<TResult> func)
    {
        _lock.EnterReadLock();

        try
        {
            return func();
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// Releases all resources used by the <see cref="LockCollectionSynchronizer"/> instance, including the underlying lock.
    /// </summary>
    public void Dispose() => _lock.Dispose();
}
