// -----------------------------------------------------------------------
// <copyright file="ProgressReport.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.Utilities.Progress;

/// <summary>
/// Represents a snapshot of the current progress state reported to subscribers.
/// </summary>
/// <typeparam name="T">The type of progress messages.</typeparam>
public sealed record ProgressReport<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressReport{T}"/> class.
    /// </summary>
    /// <param name="progress">Overall progress value between 0.0 and 1.0.</param>
    /// <param name="messages">Breadcrumb of active messages from root to innermost step.</param>
    /// <param name="cancelAction">Action to invoke in order to cancel the operation, or <see langword="null"/> if none was provided.</param>
    /// <param name="canCancel">Whether the current operation can be cancelled.</param>
    public ProgressReport(double progress, IReadOnlyList<T> messages, Action? cancelAction, bool canCancel)
    {
        Progress = progress;
        Messages = messages;
        CancelAction = cancelAction;
        CanCancel = canCancel;
    }

    /// <summary>Gets the overall progress value, between 0.0 (not started) and 1.0 (complete).</summary>
    public double Progress { get; }

    /// <summary>Gets the ordered list of active messages from the root step down to the innermost step.</summary>
    public IReadOnlyList<T> Messages { get; }

    /// <summary>Gets the action to invoke to cancel the running operation, or <see langword="null"/> if not provided.</summary>
    public Action? CancelAction { get; }

    /// <summary>Gets a value indicating whether every active step supports cancellation.</summary>
    public bool CanCancel { get; }
}
