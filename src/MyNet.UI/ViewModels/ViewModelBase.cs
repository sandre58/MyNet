// -----------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MyNet.Observable;
using MyNet.Observable.Attributes;
using MyNet.UI.Loading;
using MyNet.UI.Loading.Models;
using MyNet.UI.ViewModels.Workspace;
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
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed in Cleanup")]
    private readonly SemaphoreSlim _stateLock = new(1, 1);

    /// <summary>
    /// Gets the unique identifier for this view model instance.
    /// Useful for tracking, logging, and debugging.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Gets the current loading state of the workspace.
    /// </summary>
    public LoadState State { get; private set; } = LoadState.NotLoaded;

    /// <summary>
    /// Gets the busy service for indicating long-running operations.
    /// </summary>
    public IBusyService BusyService { get; } = busyService ?? BusyManager.Create();

    #region Loading

    /// <summary>
    /// Loads the view model asynchronously if it is not already loaded. If the view model is in the <see cref="LoadState.NotLoaded"/> state, this method will call <see cref="OnLoadAsync"/> to perform the actual loading logic. If the view model is already in the <see cref="LoadState.Loading"/>, <see cref="LoadState.Loaded"/>, or <see cref="LoadState.Error"/> state, this method will do nothing and return immediately. This ensures that the loading logic is only executed once and prevents redundant loading attempts.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected Task LoadAsync(CancellationToken cancellationToken = default) => ExecuteStateAsync<IndeterminateBusy>(async (_, ct) => await OnLoadAsync(ct).ConfigureAwait(false), cancellationToken);

    /// <summary>
    /// When overridden in a derived class, performs the actual loading logic for the view model. This method is called by <see cref="LoadAsync"/> when the view model is not already loaded. The default implementation does nothing. Override this method to implement custom loading behavior that should only run once, such as fetching data from a service or initializing state.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task OnLoadAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    #endregion

    #region Async Execution

    /// <summary>
    /// Executes an asynchronous action with a busy indicator of the specified type, and manages the loading state of the view model. The view model state is set to <see cref="LoadState.Loading"/> before executing the action, and then set to <see cref="LoadState.Loaded"/> if the action completes successfully, or <see cref="LoadState.Error"/> if an exception occurs. If the operation is canceled, the state is reset to <see cref="LoadState.NotLoaded"/>. This method ensures that only one state transition can occur at a time by using a semaphore lock.
    /// </summary>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    protected async Task ExecuteStateAsync<TBusy>(Func<TBusy, CancellationToken, Task> action, CancellationToken cancellationToken = default)
    where TBusy : class, IBusy, new()
    {
        await _stateLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            State = LoadState.Loading;

            await BusyService.RunAsync(action, cancellationToken).ConfigureAwait(false);

            State = LoadState.Loaded;
        }
        catch (OperationCanceledException)
        {
            State = LoadState.NotLoaded;
            throw;
        }
        catch (Exception ex)
        {
            State = LoadState.Error;
            OnExecutionError(ex);
            throw;
        }
        finally
        {
            _stateLock.Release();
        }
    }

    /// <summary>
    /// Marks the workspace as loaded without performing any loading logic. This can be used when the loading process is handled externally and the workspace just needs to update its state.
    /// </summary>
    /// <param name="action">The asynchronous action to execute during the state transition.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    protected async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        try
        {
            await BusyService.RunAsync<IndeterminateBusy>(async (_, ct) => await action(ct).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            OnExecutionError(ex);
        }
    }

    /// <summary>
    /// Executes an asynchronous function that returns a result with automatic error handling.
    /// Exceptions are caught and passed to <see cref="OnExecutionError"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <param name="defaultValue">The default value to return if an error occurs.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation with the result, or the default value if an error occurs.</returns>
    protected async Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> func, TResult defaultValue = default!, CancellationToken cancellationToken = default)
    {
        try
        {
            TResult? result = default;
            await BusyService.RunAsync<IndeterminateBusy>(async (_, ct) => result = await func(ct).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);

            return result!;
        }
        catch (Exception ex)
        {
            OnExecutionError(ex);
            return defaultValue;
        }
    }

    /// <summary>
    /// Executes an asynchronous function with progress tracking and cancellation support.
    /// Use this for async operations where progress can be reported.
    /// </summary>
    /// <param name="action">The asynchronous function to execute. Receives a <see cref="ProgressionBusy"/> instance for progress reporting.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected async Task ExecuteWithProgressAsync(Func<ProgressionBusy, CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        try
        {
            await BusyService.RunAsync<ProgressionBusy>(async (x, ct) => await action(x, ct).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            OnExecutionError(ex);
        }
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
    protected virtual void OnExecutionError(Exception exception) => throw exception;

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

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. Disposes the state lock and calls the base cleanup method.
    /// </summary>
    protected override void Cleanup()
    {
        _stateLock.Dispose();
        base.Cleanup();
    }
}
