// -----------------------------------------------------------------------
// <copyright file="ImmediateCollectionSynchronizer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Collections;

/// <summary>
/// Current collection synchronizer that performs no synchronization, for use when thread safety is not required.
/// </summary>
public sealed class ImmediateCollectionSynchronizer : ICollectionSynchronizer
{
    /// <summary>
    /// Gets the default instance of the collection synchronizer that performs no synchronization.
    /// </summary>
    public static ImmediateCollectionSynchronizer Default { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ImmediateCollectionSynchronizer"/> class.
    /// </summary>
    private ImmediateCollectionSynchronizer() { }

    /// <summary>
    /// Executes the specified action without any synchronization, as this implementation does not provide thread safety.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public void Write(Action action) => action();

    /// <summary>
    /// Executes the specified function without any synchronization, as this implementation does not provide thread safety.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>The result of the function.</returns>
    public TResult Write<TResult>(Func<TResult> func) => func();

    /// <summary>
    /// Executes the specified action without any synchronization, as this implementation does not provide thread safety.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public void Read(Action action) => action();

    /// <summary>
    /// Executes the specified function without any synchronization, as this implementation does not provide thread safety.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>The result of the function.</returns>
    public TResult Read<TResult>(Func<TResult> func) => func();
}
