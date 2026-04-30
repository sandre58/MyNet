// -----------------------------------------------------------------------
// <copyright file="Busy.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

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
    public Busy() => CancelCommand = CommandsManager.Create(Cancel, () => IsCancellable && CanCancel && !IsCancelling);

    /// <summary>
    /// Gets the cancellation token associated with this busy operation.
    /// </summary>
    public CancellationToken CancellationToken => _cts?.Token ?? CancellationToken.None;

    /// <summary>
    /// Gets a value indicating whether cancellation is possible.
    /// </summary>
    public bool IsCancellable => _cts?.IsCancellationRequested == false;

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
    public bool CanCancel { get; set; } = true;

    /// <summary>
    /// Internal: attach a CTS from BusyService.
    /// </summary>
    internal void Attach(CancellationTokenSource cts)
    {
        _cts = cts;
        OnPropertyChanged(nameof(IsCancellable));
    }

    /// <summary>
    /// Requests cancellation of the busy operation. Sets <see cref="IsCancelling"/> to true and invokes <see cref="CancelAction"/> if set.
    /// </summary>
    public void Cancel()
    {
        if (!IsCancellable)
            return;

        IsCancelling = true;
        OnPropertyChanged(nameof(IsCancelling));

        _cts?.Cancel();
    }
}
