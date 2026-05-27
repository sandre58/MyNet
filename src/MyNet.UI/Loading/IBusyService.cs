// -----------------------------------------------------------------------
// <copyright file="IBusyService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Loading.Models;

namespace MyNet.UI.Loading;

/// <summary>
/// Manages busy operation scopes and exposes observable state for UI binding.
/// Each <see cref="ViewModels.ViewModelBase"/> owns a local <see cref="BusyService"/> instance;
/// the client application may register a singleton for global operations (overlay, shell).
/// </summary>
public interface IBusyService : INotifyPropertyChanged
{
    /// <summary>
    /// Gets a value indicating whether at least one busy operation is active.
    /// </summary>
    bool IsBusy { get; }

    /// <summary>
    /// Gets the top-most busy indicator on the stack, if any.
    /// </summary>
    IBusy? CurrentBusy { get; }

    /// <summary>
    /// Executes an asynchronous action while a busy scope of type <typeparamref name="TBusy"/> is active.
    /// </summary>
    /// <typeparam name="TBusy">The busy indicator type.</typeparam>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RunAsync<TBusy>(Func<TBusy, CancellationToken, Task> action, CancellationToken cancellationToken = default)
        where TBusy : IBusy, new();

    /// <summary>
    /// Executes an asynchronous action while a busy scope of type <typeparamref name="TBusy"/> is active.
    /// </summary>
    /// <typeparam name="TBusy">The busy indicator type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<TResult> RunAsync<TBusy, TResult>(Func<TBusy, CancellationToken, Task<TResult>> action, CancellationToken cancellationToken = default)
        where TBusy : IBusy, new();

    /// <summary>
    /// Opens a busy scope that remains active until the returned <see cref="IBusyScope"/> is disposed.
    /// </summary>
    /// <typeparam name="TBusy">The busy indicator type.</typeparam>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A disposable busy scope.</returns>
    IBusyScope Begin<TBusy>(CancellationToken cancellationToken = default)
        where TBusy : IBusy, new();

    /// <summary>
    /// Gets the current top-most busy indicator of type <typeparamref name="TBusy"/>, if any.
    /// </summary>
    /// <typeparam name="TBusy">The busy indicator type.</typeparam>
    /// <returns>The current indicator, or <c>null</c>.</returns>
    TBusy? GetCurrent<TBusy>()
        where TBusy : class, IBusy;
}
