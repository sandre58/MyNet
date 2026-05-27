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
using MyNet.Primitives;
using MyNet.UI.Loading;
using MyNet.UI.Loading.Models;

namespace MyNet.UI.ViewModels;

/// <summary>
/// Base class for all view models in the application.
/// Provides common functionality like busy indication for async operations, error handling, and state management.
/// </summary>
/// <remarks>
/// <see cref="BusyService"/> is scoped to this view model (bind <see cref="BusyService"/> in the view).
/// For application-wide busy, inject <see cref="IBusyService"/> on the operation or service that needs it.
/// </remarks>
[SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Fields is not disposable and does not require cleanup.")]
public abstract class ViewModelBase : ObservableObject, IIdentifiable<Guid>
{
    private readonly SemaphoreSlim _stateLock = new(1, 1);

    /// <summary>
    /// Gets the unique identifier for this view model instance.
    /// Useful for tracking, logging, and debugging.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Gets the current loading state of the workspace.
    /// </summary>
    public LoadState State { get; private set => SetProperty(ref field, value); } = LoadState.NotLoaded;

    /// <summary>
    /// Gets the local busy service for operations scoped to this view model.
    /// </summary>
    public IBusyService BusyService { get; } = new BusyService();

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
        where TBusy : IBusy, new()
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
    /// Executes an asynchronous action with the local busy service.
    /// </summary>
    /// <param name="action">The asynchronous action to execute during the state transition.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    protected async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        try
        {
            await BusyService.RunIndeterminateAsync(action, cancellationToken).ConfigureAwait(false);
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
    /// Executes an asynchronous function with progress tracking and cancellation support on the local busy service.
    /// </summary>
    /// <param name="action">The asynchronous function to execute. Receives a <see cref="ProgressionBusy"/> instance for progress reporting.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected async Task ExecuteWithProgressAsync(Func<ProgressionBusy, CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        try
        {
            await BusyService.RunProgressionAsync(action, cancellationToken).ConfigureAwait(false);
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
    protected virtual void OnExecutionError(Exception exception) { }

    #endregion

    /// <inheritdoc />
    public override int GetHashCode() => Id.GetHashCode();

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ViewModelBase viewModel && Id == viewModel.Id;

    /// <inheritdoc />
    protected override void DisposeManagedResources()
    {
        _stateLock.Dispose();
        base.DisposeManagedResources();
    }
}
