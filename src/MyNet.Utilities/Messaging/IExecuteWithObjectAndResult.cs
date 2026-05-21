// -----------------------------------------------------------------------
// <copyright file="IExecuteWithObjectAndResult.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Messaging;

/// <summary>
/// This interface is meant for the <see cref="WeakFunc{T, TResult}" /> class and can be
/// useful if you store multiple WeakFunc{T, TResult} instances but don't know in advance
/// what type T represents.
/// </summary>
public interface IExecuteWithObjectAndResult
{
    /// <summary>
    /// Gets the target of the WeakFunc.
    /// </summary>
    object? Target { get; }

    /// <summary>
    /// Gets a value indicating whether the target is still alive.
    /// </summary>
    bool IsAlive { get; }

    /// <summary>
    /// Gets the name of the method that this weak function represents.
    /// </summary>
    string? MethodName { get; }

    /// <summary>
    /// Executes a Func and returns the result.
    /// </summary>
    /// <param name="parameter">A parameter passed as an object,
    /// to be casted to the appropriate type.</param>
    /// <returns>The result of the operation.</returns>
    object? ExecuteWithObject(object? parameter);

    /// <summary>
    /// Deletes all references, which notifies the cleanup method
    /// that this entry must be deleted.
    /// </summary>
    void MarkForDeletion();
}
