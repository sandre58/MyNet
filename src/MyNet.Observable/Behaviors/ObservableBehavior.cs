// -----------------------------------------------------------------------
// <copyright file="ObservableBehavior.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Represents a base behavior attached to an <see cref="ObservableObject"/>.
/// </summary>
/// <param name="owner">The owner object.</param>
/// <exception cref="ArgumentNullException">Thrown when owner is null.</exception>
public abstract class ObservableBehavior(ObservableObject owner) : IObservableBehavior, IDisposable
{
    /// <summary>
    /// Gets the owner object.
    /// </summary>
    protected ObservableObject Owner { get; } = owner ?? throw new ArgumentNullException(nameof(owner));

    /// <summary>
    /// Gets a value indicating whether the behavior is disposed.
    /// </summary>
    protected bool IsDisposed { get; private set; }

    #region IDisposable Support

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    protected virtual void DisposeManagedResources()
    {
    }

    /// <summary>
    /// Disposes the object and releases all resources.
    /// </summary>
    /// <param name="disposing">Indicates whether the method is called from Dispose (true) or from a finalizer (false).</param>
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
            return;

        if (disposing)
            DisposeManagedResources();

        IsDisposed = true;
    }

    /// <summary>
    /// Disposes the object and releases all resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion IDisposable Support
}
