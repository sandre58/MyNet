// -----------------------------------------------------------------------
// <copyright file="ImmediateCollectionEventDispatcher.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Collections;

/// <summary>
/// Dispatches events immediately on the current thread.
/// </summary>
public sealed class ImmediateCollectionEventDispatcher : ICollectionEventDispatcher
{
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static ImmediateCollectionEventDispatcher Default { get; } = new();

    private ImmediateCollectionEventDispatcher() { }

    /// <summary>
    /// Always returns true since this dispatcher executes actions immediately on the current thread, meaning it always has access to the configured context.
    /// </summary>
    /// <returns>True, indicating that the current thread has access to the dispatcher.</returns>
    public bool CheckAccess() => true;

    /// <inheritdoc />
    public void Dispatch(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        action();
    }
}
