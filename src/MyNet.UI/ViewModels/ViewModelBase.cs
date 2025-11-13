// -----------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using MyNet.Observable;
using MyNet.Observable.Attributes;
using MyNet.UI.Loading;
using MyNet.UI.Loading.Models;
using MyNet.Utilities;

namespace MyNet.UI.ViewModels;

/// <summary>
/// Base class for all view models in the application.
/// Provides common functionality like busy indication for async operations, error handling, and state management.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ViewModelBase"/> class.
/// </remarks>
/// <param name="busyService">Optional busy service. If null, a new instance is created.</param>
[CanBeValidatedForDeclaredClassOnly(false)]
[CanSetIsModifiedAttributeForDeclaredClassOnly(false)]
public abstract class ViewModelBase(IBusyService? busyService = null) : EditableObject, IIdentifiable<Guid>
{
    /// <summary>
    /// Gets the unique identifier for this view model instance.
    /// Useful for tracking, logging, and debugging.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Gets a value indicating whether this view model has been loaded.
    /// Set to true after the first successful load/refresh operation.
    /// </summary>
    public bool IsLoaded { get; private set; }

    /// <summary>
    /// Gets the busy service for indicating long-running operations.
    /// </summary>
    public IBusyService BusyService { get; } = busyService ?? BusyManager.Create();

    /// <summary>
    /// Marks the view model as loaded.
    /// Call this after successful initialization or refresh operations.
    /// </summary>
    protected void MarkAsLoaded() => IsLoaded = true;

    #region Async Execution

    /// <summary>
    /// Executes an action asynchronously with indeterminate busy indication.
    /// Use this for operations where progress cannot be tracked.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual async Task ExecuteAsync(Action action)
    => await BusyService.WaitAsync<IndeterminateBusy>(_ => action()).ConfigureAwait(false);

    /// <summary>
    /// Executes an asynchronous function with indeterminate busy indication.
    /// Use this for async operations where progress cannot be tracked.
    /// </summary>
    /// <param name="action">The asynchronous function to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual async Task ExecuteAsync(Func<Task> action)
        => await BusyService.WaitAsync<IndeterminateBusy>(async _ => await action().ConfigureAwait(false)).ConfigureAwait(false);

    /// <summary>
    /// Executes an action asynchronously with progress tracking and cancellation support.
    /// Use this for operations where progress can be reported.
    /// </summary>
    /// <param name="action">The action to execute. Receives a <see cref="ProgressionBusy"/> instance for progress reporting.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual async Task ExecuteWithProgressAsync(Action<ProgressionBusy> action)
        => await BusyService.WaitAsync(action).ConfigureAwait(false);

    /// <summary>
    /// Executes an asynchronous function with progress tracking and cancellation support.
    /// Use this for async operations where progress can be reported.
    /// </summary>
    /// <param name="action">The asynchronous function to execute. Receives a <see cref="ProgressionBusy"/> instance for progress reporting.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual async Task ExecuteWithProgressAsync(Func<ProgressionBusy, Task> action)
  => await BusyService.WaitAsync(action).ConfigureAwait(false);

    /// <summary>
    /// Executes a function that returns a result asynchronously with indeterminate busy indication.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>A task representing the asynchronous operation with the result.</returns>
    protected virtual async Task<TResult> ExecuteAsync<TResult>(Func<TResult> func)
    {
        TResult? result = default;
        await BusyService.WaitAsync<IndeterminateBusy>(_ => result = func()).ConfigureAwait(false);
        return result!;
    }

    /// <summary>
    /// Executes an asynchronous function that returns a result with indeterminate busy indication.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>A task representing the asynchronous operation with the result.</returns>
    protected virtual async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> func)
    {
        TResult? result = default;
        await BusyService.WaitAsync<IndeterminateBusy>(async _ => result = await func().ConfigureAwait(false)).ConfigureAwait(false);
        return result!;
    }

    #endregion

    #region Error Handling

    /// <summary>
    /// Called when an exception occurs during an async operation.
    /// Override this method to provide custom error handling (logging, user notification, etc.).
    /// The default implementation does nothing, allowing exceptions to propagate.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    /// <remarks>
    /// Example override:
    /// <code>
    /// protected override void OnExecutionError(Exception exception)
    /// {
    ///  Logger.Error(exception, "Error in {ViewModelType}", GetType().Name);
    ///     ToasterManager.ShowError($"An error occurred: {exception.Message}");
    /// }
    /// </code>
    /// </remarks>
    protected virtual void OnExecutionError(Exception exception)
    {
        // Default: do nothing, let exception propagate
        // Derived classes should override to provide custom error handling
    }

    /// <summary>
    /// Executes an action asynchronously with automatic error handling.
    /// Exceptions are caught and passed to <see cref="OnExecutionError"/>.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual async Task ExecuteSafeAsync(Action action)
    {
        try
        {
            await ExecuteAsync(action).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            OnExecutionError(ex);
        }
    }

    /// <summary>
    /// Executes an asynchronous function with automatic error handling.
    /// Exceptions are caught and passed to <see cref="OnExecutionError"/>.
    /// </summary>
    /// <param name="action">The asynchronous function to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual async Task ExecuteSafeAsync(Func<Task> action)
    {
        try
        {
            await ExecuteAsync(action).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            OnExecutionError(ex);
        }
    }

    /// <summary>
    /// Executes a function that returns a result asynchronously with automatic error handling.
    /// Exceptions are caught and passed to <see cref="OnExecutionError"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <param name="defaultValue">The default value to return if an error occurs.</param>
    /// <returns>A task representing the asynchronous operation with the result, or the default value if an error occurs.</returns>
    protected virtual async Task<TResult> ExecuteSafeAsync<TResult>(Func<TResult> func, TResult defaultValue = default!)
    {
        try
        {
            return await ExecuteAsync(func).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            OnExecutionError(ex);
            return defaultValue;
        }
    }

    /// <summary>
    /// Executes an asynchronous function that returns a result with automatic error handling.
    /// Exceptions are caught and passed to <see cref="OnExecutionError"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <param name="defaultValue">The default value to return if an error occurs.</param>
    /// <returns>A task representing the asynchronous operation with the result, or the default value if an error occurs.</returns>
    protected virtual async Task<TResult> ExecuteSafeAsync<TResult>(Func<Task<TResult>> func, TResult defaultValue = default!)
    {
        try
        {
            return await ExecuteAsync(func).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            OnExecutionError(ex);
            return defaultValue;
        }
    }

    #endregion

    /// <summary>
    /// Returns a hash code for this instance based on the unique identifier.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// Determines whether the specified object is equal to this instance by comparing unique identifiers.
    /// </summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns>true if the specified object is a ViewModelBase with the same Id; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is ViewModelBase viewModel && Id == viewModel.Id;
}
