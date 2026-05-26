// -----------------------------------------------------------------------
// <copyright file="IExecuteWithObject.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Messaging;

/// <summary>
/// This interface is meant for the <see cref="WeakAction{T}" /> class and can be
/// useful if you store multiple WeakAction{T} instances but don't know in advance
/// what type T represents.
/// </summary>
public interface IExecuteWithObject
{
    /// <summary>
    /// Gets the target of the WeakAction.
    /// </summary>
    object? Target { get; }

    /// <summary>
    /// Gets a value indicating whether the target is still alive.
    /// </summary>
    bool IsAlive { get; }

    /// <summary>
    /// Gets the name of the method that this weak action represents.
    /// </summary>
    string? MethodName { get; }

    /// <summary>
    /// Executes an action.
    /// </summary>
    /// <param name="parameter">A parameter passed as an object,
    /// to be casted to the appropriate type.</param>
    void ExecuteWithObject(object? parameter);

    /// <summary>
    /// Deletes all references, which notifies the cleanup method
    /// that this entry must be deleted.
    /// </summary>
    void MarkForDeletion();
}
