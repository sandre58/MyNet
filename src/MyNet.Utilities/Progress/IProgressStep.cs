// -----------------------------------------------------------------------
// <copyright file="IProgressStep.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Progress;

/// <summary>
/// Represents a typed progress step that carries an optional message of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the message associated with this step.</typeparam>
public interface IProgressStep<T> : IProgressStep
{
    /// <summary>Gets the message associated with this step, or <see langword="null"/> if none was provided.</summary>
    T? Message { get; }

    /// <summary>Updates the message associated with this step and notifies subscribers.</summary>
    /// <param name="message">The new message value.</param>
    void UpdateMessage(T? message);
}

/// <summary>
/// Represents a single unit of progress within a hierarchical progress-tracking session.
/// Disposing the step marks it as completed and pops it from the active stack.
/// </summary>
public interface IProgressStep : IDisposable
{
    /// <summary>Gets the current progress value, between 0.0 (not started) and 1.0 (complete).</summary>
    double Progress { get; }

    /// <summary>Gets a value indicating whether this step has been completed (i.e. disposed).</summary>
    bool IsCompleted { get; }

    /// <summary>Gets a value indicating whether this step supports cancellation.</summary>
    bool CanCancel { get; }

    /// <summary>Gets the action to invoke to cancel the running operation, or <see langword="null"/> if none.</summary>
    Action? CancelAction { get; }

    /// <summary>
    /// Manually advances the progress to <paramref name="value"/>.
    /// Values outside [0.0, 1.0] are clamped automatically.
    /// </summary>
    /// <param name="value">The new progress value.</param>
    void UpdateProgress(double value);
}
