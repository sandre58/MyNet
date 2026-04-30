// -----------------------------------------------------------------------
// <copyright file="DialogViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using MyNet.UI.ViewModels.Common;

namespace MyNet.UI.ViewModels.Dialog;

/// <summary>
/// Provides a reusable base implementation for dialog view models.
/// </summary>
public abstract class DialogViewModel : ViewModelBase, IDialogViewModel
{
    /// <summary>
    /// Occurs when the dialog requests to be closed.
    /// </summary>
    public event EventHandler<CloseRequestedEventArgs>? CloseRequested;

    /// <summary>
    /// Gets or sets the dialog title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Determines whether the dialog can be closed.
    /// </summary>
    /// <returns><see langword="true"/> when the dialog can be closed; otherwise <see langword="false"/>.</returns>
    public virtual Task<bool> CanCloseAsync() => Task.FromResult(true);

    /// <summary>
    /// Raises a close request for the current dialog.
    /// </summary>
    /// <param name="force">A value indicating whether the close request should bypass optional checks performed by the host.</param>
    protected virtual void RequestClose(bool force = false) => CloseRequested?.Invoke(this, new() { Force = force });
}

/// <summary>
/// Provides a reusable base implementation for dialog view models that expose a typed result.
/// </summary>
/// <typeparam name="TResult">The type of the dialog result.</typeparam>
public abstract class DialogViewModel<TResult> : DialogViewModel, IDialogViewModel<TResult>
{
    /// <summary>
    /// Gets the result produced by the dialog.
    /// </summary>
    public TResult? Result { get; private set; }

    /// <summary>
    /// Sets the current result value without requesting closure.
    /// </summary>
    /// <param name="result">The result value to store.</param>
    protected void SetResult(TResult? result) => Result = result;

    /// <summary>
    /// Stores the specified result and requests the dialog to close.
    /// </summary>
    /// <param name="result">The result returned by the dialog.</param>
    /// <param name="force">A value indicating whether the close request should be forced.</param>
    protected virtual void Close(TResult? result = default, bool force = false)
    {
        Result = result;
        RequestClose(force);
    }
}
