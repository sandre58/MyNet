// -----------------------------------------------------------------------
// <copyright file="DelegateDisposable.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// A simple implementation of <see cref="IDisposable"/> that executes a provided action when disposed. This class is useful for scenarios where you want to ensure that a specific cleanup action is performed when an object goes out of scope, such as releasing resources, resetting state, or performing any necessary finalization logic. By passing the desired cleanup action to the constructor, you can easily manage resource lifetimes and ensure proper disposal without needing to create a full class for each disposable resource.
/// </summary>
/// <param name="disposeAction">The action to execute when the object is disposed.</param>
public sealed class DelegateDisposable(Action disposeAction) : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// Disposes the object by executing the provided action. If the object has already been disposed, this method does nothing. This ensures that the cleanup action is only executed once, preventing potential issues from multiple disposals. After the action is executed, the internal state is updated to indicate that the object has been disposed, allowing for safe and idempotent disposal behavior.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        disposeAction();

        _disposed = true;
    }
}
