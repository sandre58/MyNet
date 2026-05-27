// -----------------------------------------------------------------------
// <copyright file="Busy.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Windows.Input;
using MyNet.Observable;
using MyNet.UI.Commands;

namespace MyNet.UI.Loading.Models;

/// <summary>
/// Represents a busy indicator that supports cancellation.
/// </summary>
public class Busy : ObservableObject, IBusy
{
    private CancellationTokenSource? _cts;

    /// <summary>
    /// Initializes a new instance of the <see cref="Busy"/> class.
    /// </summary>
    public Busy()
        : this(RelayCommandFactory.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Busy"/> class.
    /// </summary>
    /// <param name="commandFactory">Command factory used to create the cancellation command.</param>
    public Busy(ICommandFactory commandFactory)
    {
        ArgumentNullException.ThrowIfNull(commandFactory);
        CancelCommand = commandFactory.Create(Cancel, () => IsCancellable && CanCancel && !IsCancelling);
    }

    /// <summary>
    /// Gets the cancellation token associated with this busy operation.
    /// </summary>
    public CancellationToken CancellationToken => _cts?.Token ?? CancellationToken.None;

    /// <summary>
    /// Gets a value indicating whether cancellation is possible.
    /// </summary>
    public bool IsCancellable => _cts is { IsCancellationRequested: false };

    /// <summary>
    /// Gets the command used to trigger cancellation.
    /// </summary>
    public ICommand CancelCommand { get; }

    /// <summary>
    /// Gets a value indicating whether cancellation is in progress.
    /// </summary>
    public bool IsCancelling { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether cancellation can be triggered by the user.
    /// </summary>
    public bool CanCancel { get => field; set => SetProperty(ref field, value); } = true;

    /// <summary>
    /// Binds a cancellation source to this busy indicator.
    /// Called by <see cref="Loading.BusyService"/> when a scope starts.
    /// </summary>
    /// <param name="cancellationTokenSource">The source that drives <see cref="CancellationToken"/> and user-initiated <see cref="Cancel"/>.</param>
    public void BindCancellation(CancellationTokenSource cancellationTokenSource)
    {
        ArgumentNullException.ThrowIfNull(cancellationTokenSource);
        _cts = cancellationTokenSource;
        IsCancelling = false;
        NotifyPropertyChanged(nameof(IsCancellable));
        NotifyPropertyChanged(nameof(IsCancelling));
    }

    /// <summary>
    /// Requests cancellation of the busy operation.
    /// </summary>
    public void Cancel()
    {
        if (!IsCancellable)
            return;

        IsCancelling = true;
        NotifyPropertyChanged(nameof(IsCancelling));

        _cts?.Cancel();
    }
}
