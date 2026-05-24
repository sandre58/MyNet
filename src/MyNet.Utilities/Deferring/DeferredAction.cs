// -----------------------------------------------------------------------
// <copyright file="DeferredAction.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Deferring;

/// <summary>
/// Provides a simple mechanism to defer execution of an action until deferral scopes are ended.
/// </summary>
/// <remarks>
/// Use <see cref="Defer"/> to create a scope that postpones execution. When all scopes are disposed, the bound action is executed.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="DeferredAction"/> class and binds the provided action.
/// </remarks>
/// <param name="action">The action to execute when deferral ends.</param>
public class DeferredAction(Action action) : IDeferrable
{
    private int _deferCount;
    private bool _pending;

    /// <summary>
    /// Gets a value indicating whether execution is currently deferred. Returns true if there are active deferral scopes; otherwise, false.
    /// </summary>
    public bool IsDeferred => _deferCount > 0;

    /// <summary>
    /// Creates a new deferral scope. While the scope is active, execution is deferred.
    /// Dispose the returned <see cref="IDisposable"/> to end the scope.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> representing the deferral scope.</returns>
    public IDisposable Defer()
    {
        _deferCount++;
        return new Scope(this);
    }

    /// <summary>
    /// Executes the bound action immediately, regardless of the current deferral state. This allows bypassing deferral when necessary.
    /// </summary>
    public void ExecuteNow() => action();

    /// <summary>
    /// Requests execution of the bound action. If deferral is active, marks the action as pending and defers execution until all scopes are ended. If no deferral is active, executes the action immediately.
    /// </summary>
    public void Request()
    {
        if (IsDeferred)
        {
            _pending = true;
            return;
        }

        action.Invoke();
    }

    /// <summary>
    /// Ends a deferral scope and executes the bound action if no other scopes remain active.
    /// </summary>
    private void EndDefer()
    {
        _deferCount--;

        if (_deferCount == 0 && _pending)
        {
            _pending = false;
            action.Invoke();
        }
    }

    /// <summary>
    /// Internal scope type representing a deferral. Disposing this instance ends the scope.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="Scope"/> class and registers it with the provided <see cref="DeferredAction"/>.
    /// </remarks>
    /// <param name="owner">The deferrer that created this scope.</param>
    private sealed class Scope(DeferredAction owner) : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Ends the deferral scope and triggers deferred execution if no other scopes remain.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            owner.EndDefer();
        }
    }
}
