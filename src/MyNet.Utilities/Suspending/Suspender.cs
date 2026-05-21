// -----------------------------------------------------------------------
// <copyright file="Suspender.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.Utilities.Suspending;

/// <summary>
/// Provides an implementation of the <see cref="ISuspender"/> interface, allowing operations to be suspended and resumed in a nested manner. Each call to Suspend or Resume creates a new scope that can be disposed to revert to the previous state, ensuring that suspend and resume operations are properly balanced and can be used in a stack-like fashion.
/// </summary>
public class Suspender : ISuspender
{
    private readonly Stack<SuspendScope> _scopes = new();

    /// <inheritdoc/>
    public bool IsSuspended => _scopes.TryPeek(out var scope) && scope.IsSuspended;

    /// <inheritdoc/>
    public IDisposable Suspend() => new SuspendScope(this, true);

    /// <inheritdoc/>
    public IDisposable Resume() => new SuspendScope(this, false);

    /// <summary>
    /// Pushes a new suspend scope onto the stack. This method is called by the <see cref="SuspendScope"/> constructor to manage the suspension state. The scope is added to the stack, and its state will be used to determine the overall suspension status of the <see cref="Suspender"/>. This method should only be called by the <see cref="SuspendScope"/> class to ensure proper management of the suspension state.
    /// </summary>
    /// <param name="scope">The suspend scope to push onto the stack.</param>
    internal void Push(SuspendScope scope) => _scopes.Push(scope);

    /// <summary>
    /// Pops the specified suspend scope from the stack. This method is called by the <see cref="SuspendScope"/> Dispose method to restore the previous suspension state. The method checks that the scope being popped is the same as the one on top of the stack to ensure that suspend and resume operations are properly balanced. If the scopes are not in the correct order, an InvalidOperationException is thrown to indicate a misuse of the suspension mechanism.
    /// </summary>
    /// <param name="scope">The suspend scope to pop from the stack.</param>
    /// <exception cref="InvalidOperationException">Thrown if the scopes are not disposed in the correct order.</exception>
    internal void Pop(SuspendScope scope)
    {
        if (!_scopes.TryPeek(out var current) || !ReferenceEquals(current, scope))
            throw new InvalidOperationException("Suspend scopes must be disposed in reverse order.");

        _scopes.Pop();
    }
}
