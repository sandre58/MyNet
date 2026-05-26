// -----------------------------------------------------------------------
// <copyright file="IAutoSaveEngine.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyNet.IO.AutoSave;

/// <summary>
/// Defines the contract for a service that periodically auto-saves data on a configurable timer.
/// The service can be enabled, disabled, suspended (in a scoped manner), or cancelled at any time.
/// </summary>
public interface IAutoSaveEngine
{
    /// <summary>Gets the current timer interval.</summary>
    TimeSpan Interval { get; }

    /// <summary>
    /// Gets a value indicating whether the auto-save timer is currently running. This is true when the timer is active and counting down to the next save operation, and false when the timer is stopped or paused.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>Gets a value indicating whether a save operation is currently in progress.</summary>
    bool IsSaving { get; }

    /// <summary>
    /// Sets the interval for the auto-save timer. The interval must be greater than zero.
    /// </summary>
    /// <param name="interval">The new interval for the auto-save timer.</param>
    void SetInterval(TimeSpan interval);

    /// <summary>
    /// Triggers an immediate save operation, bypassing the timer. If a save is already in progress, this method will wait for it to complete before starting a new save operation. The provided cancellation token can be used to cancel the triggered save operation if needed.
    /// </summary>
    /// <param name="ct">A cancellation token that can be used to cancel the save operation.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task TriggerSaveAsync(CancellationToken ct);

    /// <summary>
    /// Starts the timer (only effective when the service is enabled).
    /// Replaces any existing cancellation token to allow a clean new save cycle.
    /// </summary>
    void Start();

    /// <summary>Stops the timer and cancels any in-flight save operation.</summary>
    void Stop();

    /// <summary>Cancels the current in-flight save operation without stopping the timer.</summary>
    void Cancel();

    /// <summary>
    /// Suspends auto-saving for the duration of the returned scope.
    /// Any in-flight save is cancelled immediately. Auto-saving resumes when the returned
    /// <see cref="IDisposable"/> is disposed.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> whose disposal resumes auto-saving.</returns>
    IDisposable Suspend();
}
