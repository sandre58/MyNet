// -----------------------------------------------------------------------
// <copyright file="SynchronizationContextCollectionEventDispatcher.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;

namespace MyNet.Collections;

/// <summary>
/// Dispatches events through a synchronization context, typically the UI context.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SynchronizationContextCollectionEventDispatcher"/> class.
/// </remarks>
/// <param name="context">The synchronization context to use.</param>
public sealed class SynchronizationContextCollectionEventDispatcher(SynchronizationContext? context) : ICollectionEventDispatcher
{
    /// <summary>
    /// Captures the current synchronization context or falls back to immediate dispatch.
    /// </summary>
    public static ICollectionEventDispatcher CaptureCurrentOrImmediate()
        => SynchronizationContext.Current is null
            ? ImmediateCollectionEventDispatcher.Default
            : new SynchronizationContextCollectionEventDispatcher(SynchronizationContext.Current);

    /// <summary>
    /// Checks if the current thread has access to the configured synchronization context, allowing for optimized dispatching when already on the correct thread.
    /// </summary>
    /// <returns>True if the current thread has access to the synchronization context; otherwise, false.</returns>
    public bool CheckAccess() => context is null || SynchronizationContext.Current == context;

    /// <inheritdoc />
    public void Dispatch(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (context is null)
        {
            action();
            return;
        }

        context.Post(_ => action(), null);
    }
}
