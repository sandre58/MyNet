// -----------------------------------------------------------------------
// <copyright file="DialogViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MyNet.UI.Commands;
using MyNet.UI.Dialogs;
using MyNet.UI.Dialogs.ContentDialogs;
using MyNet.UI.Loading;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Dialog;

/// <summary>
/// Provides a reusable base implementation for dialog view models.
/// Implements <see cref="IDialog"/> including the <see cref="IDialogAware"/> lifecycle callbacks.
/// </summary>
public class DialogViewModel : WorkspaceViewModel, IDialog
{
    /// <summary>
    /// Occurs when the dialog requests to be closed.
    /// </summary>
    public event EventHandler<CloseRequestedEventArgs>? CloseRequested;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogViewModel"/> class.
    /// </summary>
    /// <param name="busyService">Optional busy service used to manage loading state.</param>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    protected DialogViewModel(IBusyService? busyService = null, ICommandFactory? commandFactory = null)
        : base(busyService, commandFactory)
    {
        var commands = commandFactory ?? RelayCommandFactory.Default;
        CloseCommand = commands.Create<bool?>(close => RequestClose(close == true), CanClose);
    }

    /// <summary>
    /// Gets the command to close the dialog. The boolean parameter indicates whether to force close.
    /// </summary>
    public ICommand CloseCommand { get; }

    /// <inheritdoc />
    public virtual Task<bool> CanCloseAsync() => Task.FromResult(true);

    /// <summary>
    /// Called by the infrastructure after the dialog is opened. Override for initialization logic.
    /// </summary>
    public virtual Task OnOpenedAsync() => State is LoadState.NotLoaded ? LoadAsync() : Task.CompletedTask;

    /// <summary>
    /// Called by the infrastructure after the dialog is closed. Override for cleanup logic.
    /// </summary>
    public virtual Task OnClosedAsync() => Task.CompletedTask;

    /// <summary>Determines whether the close command can execute.</summary>
    protected virtual bool CanClose(bool? force) => true;

    /// <summary>Raises a close request for the current dialog.</summary>
    /// <param name="force">A value indicating whether the close request should bypass optional checks.</param>
    protected virtual void RequestClose(bool force = false) => CloseRequested?.Invoke(this, new() { Force = force });
}

/// <summary>
/// Provides a reusable base implementation for dialog view models that expose a typed result.
/// The internal <see cref="System.Threading.Tasks.TaskCompletionSource{TResult}"/> is reset each
/// time <see cref="OnOpenedAsync"/> is called, so the instance may be reused.
/// </summary>
/// <typeparam name="TResult">The type of the dialog result.</typeparam>
public abstract class DialogViewModel<TResult>(IBusyService? busyService = null, ICommandFactory? commandFactory = null) : DialogViewModel(busyService, commandFactory), IDialog<TResult>
{
    private TaskCompletionSource<DialogResult<TResult>> _tcs = new();

    /// <inheritdoc />
    public override Task OnOpenedAsync()
    {
        // Reset so the dialog can be shown more than once.
        _tcs = new();
        return base.OnOpenedAsync();
    }

    /// <inheritdoc />
    public override Task OnClosedAsync()
    {
        // Guarantee the TCS always completes — even on forced/unexpected close.
        _tcs.TrySetResult(DialogResult<TResult>.Dismissed());
        return base.OnClosedAsync();
    }

    /// <inheritdoc />
    public Task<DialogResult<TResult>> GetResultAsync() => _tcs.Task;

    /// <summary>Marks the dialog as successful and stores <paramref name="value"/> as the result.</summary>
    protected void SetResult(TResult value)
        => _tcs.TrySetResult(DialogResult<TResult>.Success(value));

    /// <summary>Marks the dialog as explicitly canceled (no result value).</summary>
    protected void SetCancelled()
        => _tcs.TrySetResult(DialogResult<TResult>.Cancelled());

    /// <summary>Stores the result and requests the dialog to close.</summary>
    /// <param name="result">The result returned by the dialog.</param>
    /// <param name="force">Whether the close request should be forced.</param>
    protected virtual void Close(TResult result, bool force = false)
    {
        SetResult(result);
        RequestClose(force);
    }
}
