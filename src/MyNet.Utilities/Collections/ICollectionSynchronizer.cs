// -----------------------------------------------------------------------
// <copyright file="ICollectionSynchronizer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Collections;

/// <summary>
/// Defines synchronization behavior for collection operations, allowing for thread-safe access when needed.
/// </summary>
public interface ICollectionSynchronizer
{
    /// <summary>
    /// Executes the specified action within a write lock, ensuring exclusive access for modifications.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    void Write(Action action);

    /// <summary>
    /// Executes the specified function within a write lock, ensuring exclusive access for modifications.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>The result of the function.</returns>
    TResult Write<TResult>(Func<TResult> func);

    /// <summary>
    /// Executes the specified action within a read lock, allowing concurrent read access.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    void Read(Action action);

    /// <summary>
    /// Executes the specified function within a read lock, allowing concurrent read access.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>The result of the function.</returns>
    TResult Read<TResult>(Func<TResult> func);
}
