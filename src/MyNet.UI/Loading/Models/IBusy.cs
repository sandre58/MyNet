// -----------------------------------------------------------------------
// <copyright file="IBusy.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;

namespace MyNet.UI.Loading.Models;

/// <summary>
/// Defines the contract for a busy indicator, including cancellation support.
/// </summary>
public interface IBusy
{
    /// <summary>
    /// Gets a value indicating whether the busy operation can be canceled.
    /// </summary>
    bool IsCancellable { get; }

    /// <summary>
    /// Gets a value indicating whether a cancellation request is currently in progress.
    /// </summary>
    bool IsCancelling { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is allowed to trigger cancellation.
    /// </summary>
    bool CanCancel { get; set; }

    /// <summary>
    /// Gets the cancellation token associated with the current operation.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Requests cancellation of the current operation.
    /// </summary>
    void Cancel();
}
