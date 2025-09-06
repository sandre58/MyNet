// -----------------------------------------------------------------------
// <copyright file="Deferrer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;

namespace MyNet.Utilities.Deferring;

/// <summary>
/// Provides a simple mechanism to defer execution of an action until deferral scopes are ended.
/// </summary>
/// <remarks>
/// Use <see cref="Defer"/> to create a scope that postpones execution. When all scopes are disposed, the bound action is executed.
/// </remarks>
public class Deferrer : IDeferrer
{
    private readonly ConcurrentStack<DeferScope> _trackingScopes = new();
    private Action? _action;

    /// <summary>
    /// Initializes a new instance of the <see cref="Deferrer"/> class and binds the provided action.
    /// </summary>
    /// <param name="action">The action to execute when deferral ends.</param>
    public Deferrer(Action action) => Bind(action);

    /// <summary>
    /// Initializes a new instance of the <see cref="Deferrer"/> class without a bound action.
    /// </summary>
    public Deferrer() { }

    /// <summary>
    /// Gets a value indicating whether execution is currently deferred (one or more active defer scopes exist).
    /// </summary>
    public bool IsDeferred => !_trackingScopes.IsEmpty;

    /// <summary>
    /// Binds the action that will be executed when deferral ends.
    /// </summary>
    /// <param name="action">The action to bind.</param>
    public void Bind(Action action) => _action = action;

    /// <summary>
    /// Creates a new deferral scope. While the scope is active, execution is deferred.
    /// Dispose the returned <see cref="IDisposable"/> to end the scope.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> representing the deferral scope.</returns>
    public IDisposable Defer() => new DeferScope(this);

    /// <summary>
    /// Executes the bound action immediately.
    /// </summary>
    public void Execute() => _action?.Invoke();

    /// <summary>
    /// Executes the bound action immediately if execution is not currently deferred; otherwise does nothing.
    /// </summary>
    public void DeferOrExecute()
    {
        if (IsDeferred) return;
        _action?.Invoke();
    }

    internal void Pop() => _trackingScopes.TryPop(out _);

    internal void Push(DeferScope trackingScope) => _trackingScopes.Push(trackingScope);

    internal void EndDefer() => DeferOrExecute();
}
