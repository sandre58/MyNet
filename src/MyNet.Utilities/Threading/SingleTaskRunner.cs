// -----------------------------------------------------------------------
// <copyright file="SingleTaskRunner.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyNet.Utilities.Logging;

namespace MyNet.Utilities.Threading;

/// <summary>
/// Provides a mechanism to run a single instance of an asynchronous task at a time, with support for cancellation and notifications when the running state changes or when the task is cancelled.
/// </summary>
/// <param name="action">The asynchronous action to execute.</param>
/// <param name="onRunningChanged">An optional callback invoked when the running state changes.</param>
/// <param name="onCancelled">An optional callback invoked when the task is canceled.</param>
/// <param name="logger">An optional logger for logging errors.</param>
public sealed class SingleTaskRunner(
    Func<CancellationToken, Task> action,
    Action<bool>? onRunningChanged = null,
    Action? onCancelled = null,
    ILogger? logger = null)
    : IDisposable
{
    private readonly Lock _lock = new();
    private readonly Func<CancellationToken, Task> _action = action ?? throw new ArgumentNullException(nameof(action));
    private CancellationTokenSource? _cts;
    private Task? _currentTask;
    private bool _isRunning;
    private bool _disposed;

    /// <summary>
    /// Gets a value indicating whether a task is currently running.
    /// </summary>
    public bool IsRunning
    {
        get
        {
            lock (_lock)
            {
                return _isRunning;
            }
        }
    }

    /// <summary>
    /// Gets the currently running task if any.
    /// </summary>
    public Task? CurrentTask
    {
        get
        {
            lock (_lock)
            {
                return _currentTask;
            }
        }
    }

    /// <summary>
    /// Starts execution if no task is already running.
    /// </summary>
    /// <returns>
    /// True if the task was started; otherwise false.
    /// </returns>
    public bool Run()
    {
        lock (_lock)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (_isRunning)
                return false;

            _cts?.Dispose();

            _cts = new();
            var token = _cts.Token;

            _isRunning = true;

            _currentTask = ExecuteAsync(token);
        }

        NotifyRunningChanged(true);

        return true;
    }

    /// <summary>
    /// Starts execution if no task is already running and returns the running task.
    /// </summary>
    /// <returns>
    /// The running task, or null if another execution is already in progress.
    /// </returns>
    public Task? RunAsync() => Run() ? CurrentTask : null;

    /// <summary>
    /// Cancels the current execution.
    /// </summary>
    public void Cancel()
    {
        lock (_lock)
        {
            if (_disposed)
                return;

            _cts?.Cancel();
        }
    }

    /// <summary>
    /// Executes the provided asynchronous action, handling cancellation and exceptions, and ensuring that the running state is updated appropriately when the task completes or is cancelled.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    private async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _action(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            if (cancellationToken.IsCancellationRequested)
                OnCancelledSafe();
        }
        catch (Exception ex)
        {
            logger?.LogException(ex);
        }
        finally
        {
            lock (_lock)
            {
                _isRunning = false;
                _currentTask = null;
            }

            NotifyRunningChanged(false);
        }
    }

    /// <summary>
    /// Notifies subscribers that the running state has changed, invoking the onRunningChanged callback if provided, and logging any exceptions that occur during invocation.
    /// </summary>
    /// <param name="isRunning">A value indicating whether a task is currently running.</param>
    private void NotifyRunningChanged(bool isRunning)
    {
        try
        {
            onRunningChanged?.Invoke(isRunning);
        }
        catch (Exception ex)
        {
            logger?.LogException(ex);
        }
    }

    /// <summary>
    /// Notifies subscribers that the task has been canceled, invoking the onCancelled callback if provided, and logging any exceptions that occur during invocation.
    /// </summary>
    private void OnCancelledSafe()
    {
        try
        {
            onCancelled?.Invoke();
        }
        catch (Exception ex)
        {
            logger?.LogException(ex);
        }
    }

    /// <summary>
    /// Disposes the SingleTaskRunner, ensuring that any running task is canceled and that resources are released appropriately. After disposal, the SingleTaskRunner will no longer accept new tasks or allow cancellation, and any attempts to run a task will throw an ObjectDisposedException.
    /// </summary>
    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed)
                return;

            _disposed = true;

            try
            {
                _cts?.Cancel();
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }
        }
    }
}
