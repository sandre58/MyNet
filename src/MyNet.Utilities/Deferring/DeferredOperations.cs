// -----------------------------------------------------------------------
// <copyright file="DeferredOperations.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.Utilities.Deferring;

/// <summary>
/// Provides a simple mechanism to defer execution of actions until deferral scopes are ended. Use <see cref="Defer"/> to create a scope that postpones execution. When all scopes are disposed, the bound actions are executed.
/// </summary>
public sealed class DeferredOperations
{
    private readonly List<Action> _operations = [];
    private int _deferCount;

    /// <summary>
    /// Gets a value indicating whether execution is currently deferred. Returns true if there are active deferral scopes; otherwise, false.
    /// </summary>
    public bool IsDeferred => _deferCount > 0;

    /// <summary>
    /// Creates a new deferral scope. While the scope is active, execution is deferred. Dispose the returned <see cref="IDisposable"/> to end the scope.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> representing the deferral scope.</returns>
    public IDisposable Defer()
    {
        _deferCount++;
        return new Scope(this);
    }

    /// <summary>
    /// Executes the provided action immediately if deferral is not active; otherwise, defers execution until deferral scopes are ended.
    /// </summary>
    /// <param name="action">The action to execute or defer.</param>
    public void Execute(Action action)
    {
        if (IsDeferred)
        {
            _operations.Add(action);
            return;
        }

        action();
    }

    /// <summary>
    /// Ends a deferral scope and executes all deferred actions if no other scopes remain active.
    /// </summary>
    private void EndDefer()
    {
        _deferCount--;

        if (_deferCount == 0)
        {
            var operations = _operations.ToArray();
            _operations.Clear();

            foreach (var operation in operations)
                operation();
        }
    }

    /// <summary>
    /// Internal scope type representing a deferral. Disposing this instance ends the scope.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="Scope"/> class and registers it with the provided <see cref="DeferredAction"/>.
    /// </remarks>
    /// <param name="owner">The deferrer that created this scope.</param>
    private sealed class Scope(DeferredOperations owner) : IDisposable
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
