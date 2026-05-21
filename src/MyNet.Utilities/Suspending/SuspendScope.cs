// -----------------------------------------------------------------------
// <copyright file="SuspendScope.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Suspending;

/// <summary>
/// Represents a scope for suspending or allowing operations. When disposed, it will automatically pop itself from the owning <see cref="Suspender"/> to restore the previous state.
/// </summary>
internal sealed class SuspendScope : IDisposable
{
    private readonly Suspender _owner;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="SuspendScope"/> class with the specified owner and suspension state. The scope will automatically push itself onto the owner's stack to manage the suspension state.
    /// </summary>
    /// <param name="owner">The owner <see cref="Suspender"/> that manages the suspension state.</param>
    /// <param name="suspended">A value indicating whether the scope should start in a suspended state.</param>
    public SuspendScope(Suspender owner, bool suspended)
    {
        _owner = owner;
        IsSuspended = suspended;

        _owner.Push(this);
    }

    /// <summary>
    /// Gets a value indicating whether operations are currently suspended within this scope. This property reflects the state of the scope and is used by the owning <see cref="Suspender"/> to determine the overall suspension state.
    /// </summary>
    public bool IsSuspended { get; }

    /// <summary>
    /// Disposes the scope, which will automatically pop it from the owning <see cref="Suspender"/> to restore the previous suspension state. If the scope has already been disposed, this method will have no effect.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        _owner.Pop(this);

        _disposed = true;
    }
}
