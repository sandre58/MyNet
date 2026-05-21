// -----------------------------------------------------------------------
// <copyright file="ICollectionEventDispatcher.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Collections;

/// <summary>
/// Dispatches collection events to the desired execution context (UI thread, immediate, etc.).
/// </summary>
public interface ICollectionEventDispatcher
{
    /// <summary>
    /// Checks if the current thread has access to the configured context, allowing for optimized dispatching when already on the correct thread.
    /// </summary>
    /// <returns><c>true</c> if the current thread has access; otherwise, <c>false</c>.</returns>
    bool CheckAccess();

    /// <summary>
    /// Dispatches an action to the configured context.
    /// </summary>
    /// <param name="action">The action to dispatch.</param>
    void Dispatch(Action action);
}
